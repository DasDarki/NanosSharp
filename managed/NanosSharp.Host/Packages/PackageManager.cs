using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Core;
using NanosSharp.Host.Configuration;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Host.Packages.Dependencies;
using NanosSharp.Host.Packages.Loading;
using NanosSharp.Interop;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Scripting;

namespace NanosSharp.Host.Packages;

internal static unsafe class PackageManager
{
    private const string ApiDumpEnvironmentVariable = "NANOSSHARP_DUMP_API";
    private const string SettingsFileName = "NanosSharp.toml";
    private static readonly TimeSpan TransientRetryDelay = TimeSpan.FromSeconds(1);
    private static readonly Dictionary<nint, ManagedPackage> Packages = [];
    private static readonly Dictionary<string, PackageBuild> PendingBuilds = new(StringComparer.Ordinal);
    private static readonly UnloadTracker UnloadTracker = new();
    private static HostSettings _settings = null!;
    private static PackageBuildOptions _buildOptions = null!;

    public static void Initialize(string runtimeDirectory)
    {
        var serverDirectory = Path.GetFullPath(Path.Combine(runtimeDirectory, "..", ".."));
        _settings = HostSettings.Load(Path.Combine(serverDirectory, SettingsFileName));
        var nugetCache = Path.Combine(serverDirectory, "NanosSharp", "cache", "nuget");
        _buildOptions = new PackageBuildOptions(new NuGetRestorer(_settings.NuGet.Sources, nugetCache), _settings.Projects.Generate,
            _settings.Compilation.Optimize);
        NativeCallbacks.EngineAttached += OnEngineAttached;
        NativeCallbacks.EngineDetaching += OnEngineDetaching;
        NativeCallbacks.PackageLoaded += OnPackageLoaded;
        NativeCallbacks.PackageUnloading += OnPackageUnloading;
        NativeCallbacks.Tick += _ => UnloadTracker.Tick();
#if !CLIENT
        NativeCallbacks.ClientFilesStoring += PackageMarker.Write;
        NativeCallbacks.ClientFilesStored += PackageMarker.Remove;
#endif
    }

    private static void OnEngineAttached()
    {
        StartHandshake();
        if (Environment.GetEnvironmentVariable(ApiDumpEnvironmentVariable) is { Length: > 0 } path)
        {
            ScriptApi.DumpApi(path);
            Log.Info($"Scripting API dumped to {path}.");
        }
    }

    private static void OnEngineDetaching()
    {
#if CLIENT
        ServerHandshake.Stop();
#else
        NanosSharpClients.Stop();
#endif
        PendingBuilds.Clear();
        foreach (var package in Packages.Values.ToArray())
        {
            Unregister(package);
        }
    }

    private static void StartHandshake()
    {
        try
        {
#if CLIENT
            ServerHandshake.Start();
#else
            var clients = _settings.Clients;
            NanosSharpClients.Start(new ClientPolicy(clients.Required, TimeSpan.FromSeconds(clients.HandshakeTimeoutSeconds),
                clients.KickMessage));
#endif
        }
        catch (Exception exception)
        {
            Log.Exception(exception, "Failed to start the NanosSharp client handshake");
        }
    }

    private static void OnPackageLoaded(NativePackage nativePackage)
    {
#if CLIENT
        SetPackageActive(nativePackage.Name, false);
#endif
        if (!PackageBuilder.HasManagedCode(nativePackage.Directory))
        {
            return;
        }

#if CLIENT
        if (PackageMarker.FindIncompatibility(nativePackage.Directory) is { } incompatibility)
        {
            Log.Warning($"The C# code of {nativePackage.Name} is not started because {incompatibility}, its Lua fallback is used instead.");
            return;
        }
#endif

        if (Packages.TryGetValue(nativePackage.Handle, out var existing))
        {
            Unregister(existing);
        }

        var package = new ManagedPackage(nativePackage, PackageManifest.Read(nativePackage.Directory));
        Packages[package.Handle] = package;
        using (PackageContext.Enter(package.Scope))
        {
            try
            {
                var build = PendingBuilds.Remove(package.Name, out var pending) ? pending : BuildPackage(package);
                package.Start(build, assemblyName => ResolveShared(package.Manifest, assemblyName));
#if CLIENT
                SetPackageActive(package.Name, true);
#endif
            }
            catch (Exception exception)
            {
                Log.Error(exception is PackageBuildException
                    ? exception.Message
                    : $"Failed to start the C# package: {exception}");
                TrackUnload(package, package.Stop());
            }
        }

        if (_settings.HotReload.Enabled && !PackageBuilder.IsSandboxed)
        {
            package.SourcesChanged += OnSourcesChanged;
            package.WatchSources(TimeSpan.FromMilliseconds(_settings.HotReload.DelayMilliseconds));
        }
    }

#if CLIENT
    private static void SetPackageActive(string packageName, bool isActive) =>
        ScriptApi.Invoke("Client", "SetValue", Handshake.PackageValuePrefix + packageName, isActive);
#endif

    private static void OnPackageUnloading(nint handle)
    {
        if (Packages.TryGetValue(handle, out var package))
        {
            Unregister(package);
        }
    }

    private static void OnSourcesChanged(ManagedPackage package)
    {
        if (!Packages.ContainsKey(package.Handle))
        {
            return;
        }

        using (PackageContext.Enter(package.Scope))
        {
            try
            {
                PendingBuilds[package.Name] = BuildPackage(package);
            }
            catch (PackageBuildException exception) when (exception.IsTransient)
            {
                package.ScheduleSourcesChanged(TransientRetryDelay);
                return;
            }
            catch (PackageBuildException exception)
            {
                Log.Error($"{exception.Message} The running version stays active.");
                return;
            }

            Log.Info("Changes detected, reloading.");
        }

        var dependents = FindDependents(package.Name);
        Reload(package);
        foreach (var dependent in dependents)
        {
            Reload(dependent);
        }
    }

    private static PackageBuild BuildPackage(ManagedPackage package) =>
        PackageBuilder.Build(package.Name, package.Scope.Directory, CollectRequiredImages(package.Manifest), _buildOptions);

    private static void Reload(ManagedPackage package)
    {
        byte* error = null;
        if (NativeRuntime.Api.PackageReload(package.Handle, &error) == 0)
        {
            PendingBuilds.Remove(package.Name);
            NativeRuntime.Log(LogLevel.Error, $"Reloading package {package.Name} failed: {NativeRuntime.TakeString(error)}");
        }
    }

    private static void Unregister(ManagedPackage package)
    {
        Packages.Remove(package.Handle);
        package.SourcesChanged -= OnSourcesChanged;
        package.StopWatching();
        try
        {
            TrackUnload(package, package.Stop());
        }
        catch (Exception exception)
        {
            NativeRuntime.Log(LogLevel.Error, $"Failed to stop C# package {package.Name}: {exception}");
        }
    }

    private static void TrackUnload(ManagedPackage package, AssemblyLoadContext? loadContext)
    {
        if (loadContext is not null && _settings.HotReload.VerifyUnload)
        {
            UnloadTracker.Track(package.Name, loadContext);
        }
    }

    private static ManagedPackage? FindPackage(string name) =>
        Packages.Values.FirstOrDefault(package => package.Name == name);

    private static IEnumerable<ManagedPackage> EnumerateRequired(PackageManifest manifest, HashSet<string>? visited = null)
    {
        visited ??= new HashSet<string>(StringComparer.Ordinal);
        foreach (var requirement in manifest.Requirements)
        {
            if (!visited.Add(requirement) || FindPackage(requirement) is not { } required)
            {
                continue;
            }

            yield return required;
            foreach (var transitive in EnumerateRequired(required.Manifest, visited))
            {
                yield return transitive;
            }
        }
    }

    private static IEnumerable<AssemblyImage> CollectRequiredImages(PackageManifest manifest) =>
        EnumerateRequired(manifest).SelectMany(package => package.Build?.Assemblies.Values ?? []);

    private static Assembly? ResolveShared(PackageManifest manifest, AssemblyName assemblyName) =>
        EnumerateRequired(manifest).Select(package => package.FindAssembly(assemblyName)).FirstOrDefault(assembly => assembly is not null);

    private static List<ManagedPackage> FindDependents(string packageName)
    {
        var dependents = new List<ManagedPackage>();
        var pending = new Queue<string>([packageName]);
        while (pending.TryDequeue(out var name))
        {
            var direct = Packages.Values.Where(package =>
                package.Name != packageName && !dependents.Contains(package) && package.Manifest.Requirements.Contains(name));
            foreach (var package in direct.ToArray())
            {
                dependents.Add(package);
                pending.Enqueue(package.Name);
            }
        }

        return dependents;
    }
}
