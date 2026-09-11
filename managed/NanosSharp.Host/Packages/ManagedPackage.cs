using System.Reflection;
using NanosSharp.Host.Compilation;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Host.Packages;

internal sealed class ManagedPackage
{
    private const string ServerDirectoryName = "Server";
    private const string ApiAssemblyFileName = "NanosSharp.dll";
    private static readonly TimeSpan ReloadDelay = TimeSpan.FromMilliseconds(500);
    private static readonly string[] IgnoredDirectories = ["bin", "obj"];

    public string Name => Scope.Name;

    public nint Handle { get; }

    public PackageScope Scope { get; }

    public event Action<ManagedPackage>? SourcesChanged;

    private readonly string _serverDirectory;
    private readonly List<Package> _entries = [];
    private PackageLoadContext? _loadContext;
    private FileSystemWatcher? _watcher;
    private Timer? _reloadTimer;

    public ManagedPackage(NativePackage package)
    {
        Handle = package.Handle;
        Scope = new PackageScope(package.Name, Path.GetFullPath(package.Directory)) { Environment = package.Environment };
        _serverDirectory = Path.Combine(Scope.Directory, ServerDirectoryName);
    }

    public void Start()
    {
        _loadContext = new PackageLoadContext(Name, _serverDirectory);
        Scope.IsActive = true;
        using var _ = PackageContext.Enter(Scope);

        var assemblies = LoadAssemblies(_loadContext);
        foreach (var type in assemblies.SelectMany(GetLoadableTypes).Where(IsEntryType))
        {
            var entry = (Package)Activator.CreateInstance(type, nonPublic: true)!;
            entry.Scope = Scope;
            _entries.Add(entry);
        }

        foreach (var entry in _entries)
        {
            entry.OnStart();
        }

        WatchSources();
        Log.Info($"C# package started with {_entries.Count} entry point(s).");
    }

    public void Stop()
    {
        _watcher?.Dispose();
        _watcher = null;
        _reloadTimer?.Dispose();
        _reloadTimer = null;

        using (PackageContext.Enter(Scope))
        {
            foreach (var entry in Enumerable.Reverse(_entries))
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
        }

        foreach (var subscription in Scope.Subscriptions.ToArray())
        {
            subscription.Dispose();
        }

        FunctionRegistry.ReleaseScope(Scope);
        Scope.IsActive = false;
        _entries.Clear();
        _loadContext?.Unload();
        _loadContext = null;
    }

    private List<Assembly> LoadAssemblies(PackageLoadContext loadContext)
    {
        var assemblyPaths = FindAssemblies(_serverDirectory).ToArray();
        var assemblies = assemblyPaths
            .Select(path => loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path))))
            .ToList();

        var sources = FindSources(_serverDirectory).ToArray();
        if (sources.Length == 0)
        {
            return assemblies;
        }

        var result = ScriptCompiler.Compile($"{Name}.Scripts", sources, assemblyPaths);
        foreach (var diagnostic in result.Diagnostics)
        {
            Log.Warning(diagnostic);
        }

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Compilation of {sources.Length} C# file(s) failed.");
        }

        assemblies.Add(loadContext.LoadFromBytes(result.Assembly!, result.Symbols));
        return assemblies;
    }

    private void WatchSources()
    {
        _reloadTimer = new Timer(_ => MainThread.Post(() => SourcesChanged?.Invoke(this)));
        _watcher = new FileSystemWatcher(_serverDirectory)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
        };
        _watcher.Changed += OnSourceChanged;
        _watcher.Created += OnSourceChanged;
        _watcher.Deleted += OnSourceChanged;
        _watcher.Renamed += OnSourceChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnSourceChanged(object sender, FileSystemEventArgs arguments)
    {
        var extension = Path.GetExtension(arguments.FullPath);
        if (extension is ".cs" or ".dll" && !IsIgnored(arguments.FullPath))
        {
            _reloadTimer?.Change(ReloadDelay, Timeout.InfiniteTimeSpan);
        }
    }

    public static bool HasManagedCode(string packageDirectory)
    {
        var serverDirectory = Path.Combine(packageDirectory, ServerDirectoryName);
        return Directory.Exists(serverDirectory) &&
               (FindSources(serverDirectory).Any() || FindAssemblies(serverDirectory).Any());
    }

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

    private static IEnumerable<string> FindSources(string serverDirectory) =>
        Directory.EnumerateFiles(serverDirectory, "*.cs", SearchOption.AllDirectories).Where(path => !IsIgnored(path));

    private static IEnumerable<string> FindAssemblies(string serverDirectory) =>
        Directory.EnumerateFiles(serverDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(Path.GetFileName(path), ApiAssemblyFileName, StringComparison.OrdinalIgnoreCase));

    private static bool IsIgnored(string path) =>
        path.Split(Path.DirectorySeparatorChar).Any(segment => IgnoredDirectories.Contains(segment));
}
