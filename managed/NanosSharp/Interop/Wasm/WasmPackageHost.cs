#if NANOSSHARP_WASM
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Eventing;
using NanosSharp.Injection;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Wasm;

/// <summary>
/// Loads, unloads and reloads the delivered client packages inside the sandbox
/// without rebooting the reactor. The host stages each package into its own
/// subdirectory of <c>/package</c> with a generation marker; a single
/// <c>nanos_sync_packages</c> call diffs the staged set against the loaded set and
/// loads, unloads or reloads packages to match. Each package gets its own
/// collectible load context and package scope and runs the full package lifecycle
/// (entry points, dependency injection, entity factories, exports and events), so
/// unloading one tears down just that package and leaves the others running.
/// </summary>
internal static class WasmPackageHost
{
    private const string PackageDirectory = "/package";
    private const string GenerationFileName = ".gen";

    private sealed class LoadedPackage
    {
        public required string Generation { get; init; }

        public required PackageScope Scope { get; init; }

        public required AssemblyLoadContext Context { get; init; }

        public List<Package> Entries { get; } = [];
    }

    private static readonly Dictionary<string, LoadedPackage> Loaded = new(StringComparer.Ordinal);

    /// <summary>
    /// Host-called entry that reconciles the loaded packages with the staged set.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "nanos_sync_packages")]
    public static void SyncExport() => Sync();

    [RequiresUnreferencedCode("Loads package assemblies at runtime; only used on the runtime-loading (Mono) client.")]
    private static void Sync()
    {
        if (!RuntimeFeature.IsDynamicCodeSupported || !Directory.Exists(PackageDirectory))
        {
            return;
        }

        var staged = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var directory in Directory.GetDirectories(PackageDirectory))
        {
            staged[Path.GetFileName(directory)] = ReadGeneration(directory);
        }

        foreach (var name in Loaded.Keys.ToArray())
        {
            if (!staged.TryGetValue(name, out var generation) || generation != Loaded[name].Generation)
            {
                Unload(name);
            }
        }

        foreach (var (name, generation) in staged)
        {
            if (!Loaded.ContainsKey(name))
            {
                Load(Path.Combine(PackageDirectory, name), name, generation);
            }
        }
    }

    private static string ReadGeneration(string directory)
    {
        var path = Path.Combine(directory, GenerationFileName);
        return File.Exists(path) ? File.ReadAllText(path).Trim() : "0";
    }

    [RequiresUnreferencedCode("Loads package assemblies at runtime; only used on the runtime-loading (Mono) client.")]
    private static void Load(string directory, string name, string generation)
    {
        var context = new AssemblyLoadContext($"nanos:{name}", isCollectible: true);
        context.Resolving += ResolveAcrossPackages;
        var scope = new PackageScope(name, directory)
        {
            IsActive = true,
            LoadContext = context,
        };
        var package = new LoadedPackage { Generation = generation, Scope = scope, Context = context };

        using (PackageContext.Enter(scope))
        {
            try
            {
                var types = new List<Type>();
                foreach (var path in Directory.GetFiles(directory, "*.dll"))
                {
                    var assembly = context.LoadFromStream(new MemoryStream(File.ReadAllBytes(path)));
                    types.AddRange(GetLoadableTypes(assembly));
                }

                Start(package, scope, types);
            }
            catch (Exception exception)
            {
                NativeRuntime.Log(LogLevel.Error, $"failed to load package {name}: {exception}");
            }
        }

        Loaded[name] = package;
    }

    [RequiresUnreferencedCode("Instantiates package types at runtime; only used on the runtime-loading (Mono) client.")]
    private static void Start(LoadedPackage package, PackageScope scope, List<Type> types)
    {
        foreach (var type in types.Where(IsEntryType))
        {
            var entry = (Package)Activator.CreateInstance(type, nonPublic: true)!;
            entry.Scope = scope;
            package.Entries.Add(entry);
        }

        foreach (var type in types.Where(IsEntityFactory))
        {
            EntityFactory.Register(type);
        }

        foreach (var type in types)
        {
            Exports.Register(type);
        }

        var container = new ServiceContainer(scope);
        scope.Container = container;

        foreach (var type in types.Where(IsServiceType))
        {
            container.AddDiscovered(type);
        }

        foreach (var entry in package.Entries)
        {
            container.Add(entry);
        }

        container.ResolveInjections();

        foreach (var entry in package.Entries)
        {
            EventBus.Register(entry);
        }

        foreach (var service in container.AutoRegisters)
        {
            EventBus.Register(service);
        }

        _ = container.StartAsync(package.Entries);

        NativeRuntime.Log(LogLevel.Info, $"package {scope.Name} started with {package.Entries.Count} entry point(s)");
    }

    private static void Unload(string name)
    {
        if (!Loaded.Remove(name, out var package))
        {
            return;
        }

        NativeRuntime.Log(LogLevel.Info, $"unloading package {name}");

        using (PackageContext.Enter(package.Scope))
        {
            foreach (var entry in Enumerable.Reverse(package.Entries))
            {
                StopEntry(entry);
            }

            package.Scope.Container?.Stop();
        }

        package.Scope.Container = null;
        ReleaseScope(package.Scope);
        package.Scope.IsActive = false;
        package.Scope.LoadContext = null;
        package.Entries.Clear();
        package.Context.Unload();
    }

    private static void StopEntry(Package entry)
    {
        try
        {
            entry.OnStop();
        }
        catch (Exception exception)
        {
            NativeRuntime.Log(LogLevel.Error, $"{entry.GetType().Name}.OnStop failed: {exception}");
        }
    }

    private static void ReleaseScope(PackageScope scope)
    {
        foreach (var subscription in scope.Subscriptions.ToArray())
        {
            subscription.Dispose();
        }

        scope.Events.Clear();
        Exports.ReleaseScope(scope);
        TimerScheduler.ReleaseScope(scope);
        FunctionRegistry.ReleaseScope(scope);
        EntityRegistry.ReleaseScope(scope);
    }

    private static Assembly? ResolveAcrossPackages(AssemblyLoadContext context, AssemblyName name)
    {
        foreach (var package in Loaded.Values)
        {
            var assembly = package.Context.Assemblies.FirstOrDefault(candidate => candidate.GetName().Name == name.Name);
            if (assembly is not null)
            {
                return assembly;
            }
        }

        return null;
    }

    private static bool IsEntryType(Type type) =>
        type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(Package));

    private static bool IsEntityFactory(Type type) => type.GetCustomAttribute<EntityFactoryAttribute>() is not null;

    private static bool IsServiceType(Type type) =>
        type is { IsClass: true, IsAbstract: false } && !type.IsSubclassOf(typeof(Package)) &&
        (typeof(IService).IsAssignableFrom(type) || typeof(IAutoRegister).IsAssignableFrom(type));

    [RequiresUnreferencedCode("Enumerates package types at runtime; only used on the runtime-loading (Mono) client.")]
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}
#endif
