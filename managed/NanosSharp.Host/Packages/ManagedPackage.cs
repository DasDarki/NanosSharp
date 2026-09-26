using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Eventing;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Host.Packages.Loading;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Host.Packages;

internal sealed class ManagedPackage
{
    public string Name => Scope.Name;

    public nint Handle { get; }

    public PackageScope Scope { get; }

    public PackageManifest Manifest { get; }

    public PackageBuild? Build { get; private set; }

    public event Action<ManagedPackage>? SourcesChanged;

    private readonly List<Package> _entries = [];
    private PackageLoadContext? _loadContext;
    private FileSystemWatcher? _watcher;
    private Timer? _changeTimer;

    public ManagedPackage(NativePackage package, PackageManifest manifest)
    {
        Handle = package.Handle;
        Manifest = manifest;
        Scope = new PackageScope(package.Name, Path.GetFullPath(package.Directory))
        {
            Environment = package.Environment,
            Handle = package.Handle,
        };
    }

    public void Start(PackageBuild build, Func<AssemblyName, Assembly?> resolveShared)
    {
        Build = build;
        _loadContext = new PackageLoadContext(Name, build, resolveShared);
        Scope.LoadContext = _loadContext;
        Scope.IsActive = true;
        using var _ = PackageContext.Enter(Scope);

        var loadedTypes = _loadContext.LoadBuild().SelectMany(GetLoadableTypes).ToArray();
        foreach (var type in loadedTypes.Where(IsEntryType))
        {
            var entry = (Package)Activator.CreateInstance(type, nonPublic: true)!;
            entry.Scope = Scope;
            _entries.Add(entry);
        }

        foreach (var factoryType in loadedTypes.Where(IsEntityFactory))
        {
            EntityFactory.Register(factoryType);
        }

        foreach (var entry in _entries)
        {
            EventBus.Register(entry);
        }

        foreach (var entry in _entries)
        {
            entry.OnStart();
        }

        Log.Info($"C# package started with {_entries.Count} entry point(s).");
    }

    public AssemblyLoadContext? Stop()
    {
        using (PackageContext.Enter(Scope))
        {
            foreach (var entry in Enumerable.Reverse(_entries))
            {
                StopEntry(entry);
            }
        }

        foreach (var subscription in Scope.Subscriptions.ToArray())
        {
            subscription.Dispose();
        }

        Scope.Events.Clear();
        TimerScheduler.ReleaseScope(Scope);
        FunctionRegistry.ReleaseScope(Scope);
        EntityRegistry.ReleaseScope(Scope);
        Scope.IsActive = false;
        Scope.LoadContext = null;
        _entries.Clear();

        var loadContext = _loadContext;
        _loadContext = null;
        loadContext?.Unload();
        return loadContext;
    }

    public Assembly? FindAssembly(AssemblyName assemblyName) =>
        _loadContext?.Assemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName.Name);

    public void WatchSources(TimeSpan delay)
    {
        var serverDirectory = Path.Combine(Scope.Directory, PackageBuilder.ServerDirectoryName);
        _changeTimer = new Timer(_ => MainThread.Post(() => SourcesChanged?.Invoke(this)));
        _watcher = new FileSystemWatcher(serverDirectory)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
        };
        _watcher.Changed += (_, arguments) => OnFileChanged(arguments.FullPath, delay);
        _watcher.Created += (_, arguments) => OnFileChanged(arguments.FullPath, delay);
        _watcher.Deleted += (_, arguments) => OnFileChanged(arguments.FullPath, delay);
        _watcher.Renamed += (_, arguments) => OnFileChanged(arguments.FullPath, delay);
        _watcher.EnableRaisingEvents = true;
    }

    public void ScheduleSourcesChanged(TimeSpan delay) => _changeTimer?.Change(delay, Timeout.InfiniteTimeSpan);

    public void StopWatching()
    {
        _watcher?.Dispose();
        _watcher = null;
        _changeTimer?.Dispose();
        _changeTimer = null;
    }

    private void OnFileChanged(string path, TimeSpan delay)
    {
        if (PackageBuilder.IsRelevantFile(path))
        {
            ScheduleSourcesChanged(delay);
        }
    }

    private static void StopEntry(Package entry)
    {
        try
        {
            entry.OnStop();
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"{entry.GetType().Name}.OnStop failed");
        }
    }

    private static bool IsEntityFactory(Type type) => type.GetCustomAttribute<EntityFactoryAttribute>() is not null;

    private static bool IsEntryType(Type type) =>
        type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(Package));

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
