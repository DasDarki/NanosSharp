#if CLIENT
using NanosSharp.Host.Packages.Building;
using NanosSharp.Interop;
using NanosSharp.Interop.Arguments;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Sandbox;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Runs untrusted client C# packages inside the WebAssembly sandbox instead of
/// in process. The trusted host still compiles the package with Roslyn outside the
/// sandbox, then stages the compiled assemblies into a per package subdirectory of
/// a directory the reactor reads read only. A single reactor runs for the whole
/// session; staging or unstaging a package writes or removes its subdirectory and
/// asks the reactor to reconcile, so packages are loaded, unloaded and reloaded
/// without a reboot. The reactor module is compiled once and cached so the only
/// reboot path - recovery from a failed reactor call - stays cheap.
/// </summary>
internal sealed unsafe class ClientSandbox : IDisposable
{
    private const string ReactorEnvironmentVariable = "NANOSSHARP_REACTOR";
    private const string ReactorFileName = "nanossharp-reactor.wasm";
    private const string StagingDirectoryName = "wasm-sandbox";
    private const string GenerationFileName = ".gen";

    private readonly string _reactorModule;
    private readonly string _stagingDirectory;
    private readonly Dictionary<string, int> _generations = new(StringComparer.Ordinal);

    private ReactorModule? _reactor;
    private WasmHost? _host;
    private bool _engineAttached;
    private bool _syncPending;
    private bool _reactorMissingAnnounced;

    public ClientSandbox(string runtimeDirectory)
    {
        _reactorModule = Environment.GetEnvironmentVariable(ReactorEnvironmentVariable) is { Length: > 0 } overridePath
            ? overridePath
            : Path.Combine(runtimeDirectory, ReactorFileName);
        _stagingDirectory = Path.Combine(runtimeDirectory, StagingDirectoryName);
        ResetStagingDirectory();

        NativeCallbacks.FunctionInvoker = (id, arguments, count) => _host?.InvokeFunction(id, (NativeArgument*)arguments, count) ?? 0;
        NativeCallbacks.FunctionReleaser = id => _host?.ReleaseFunction(id);
        NativeCallbacks.EntityCreatedHandler = (classId, instance, networkId) => _host?.EntityCreated(classId, instance, networkId);
        NativeCallbacks.EntityDestroyedHandler = (classId, instance) => _host?.EntityDestroyed(classId, instance);
    }

    public void Stage(string packageName, PackageBuild build)
    {
        WritePackage(packageName, build);
        _syncPending = true;
    }

    public void Unstage(string packageName)
    {
        if (!_generations.Remove(packageName))
        {
            return;
        }

        TryDeleteDirectory(Path.Combine(_stagingDirectory, packageName));
        _syncPending = true;
    }

    public void EngineAttached() => _engineAttached = true;

    public void EngineDetaching()
    {
        _engineAttached = false;
        if (_host is not null)
        {
            try
            {
                _host.EngineDetaching();
            }
            catch (Exception exception)
            {
                Log.Exception(exception, "The client sandbox failed to detach from the engine.");
            }
        }

        DisposeHost();
    }

    public void Tick(float deltaTime)
    {
        if (!_engineAttached)
        {
            return;
        }

        if (_host is null)
        {
            if (_generations.Count > 0)
            {
                EnsureHost();
            }

            return;
        }

        if (_syncPending)
        {
            _syncPending = false;
            Guard(_host.SyncPackages, "reconcile packages in");
            return;
        }

        Guard(() => _host.Tick(deltaTime), "tick");
    }

    public void Dispose()
    {
        NativeCallbacks.FunctionInvoker = null;
        NativeCallbacks.FunctionReleaser = null;
        NativeCallbacks.EntityCreatedHandler = null;
        NativeCallbacks.EntityDestroyedHandler = null;
        DisposeHost();
        _reactor?.Dispose();
        _reactor = null;
        ResetStagingDirectory();
    }

    private bool EnsureHost()
    {
        if (_host is not null)
        {
            return true;
        }

        if (!_engineAttached || !EnsureReactor())
        {
            return false;
        }

        try
        {
            var host = new WasmHost(NativeRuntime.ApiPointer, _reactor!, _stagingDirectory);
            host.Boot();
            host.EngineAttached();
            host.SyncPackages();
            _host = host;
            _syncPending = false;
            return true;
        }
        catch (Exception exception)
        {
            Log.Exception(exception, "The client sandbox failed to boot the reactor.");
            DisposeHost();
            return false;
        }
    }

    private bool EnsureReactor()
    {
        if (_reactor is not null)
        {
            return true;
        }

        if (_reactorMissingAnnounced)
        {
            return false;
        }

        if (!File.Exists(_reactorModule))
        {
            _reactorMissingAnnounced = true;
            Log.Error($"The client sandbox reactor module was not found at {_reactorModule}; C# client packages will not run.");
            return false;
        }

        try
        {
            _reactor = WasmHost.Compile(_reactorModule);
            return true;
        }
        catch (Exception exception)
        {
            _reactorMissingAnnounced = true;
            Log.Exception(exception, "The client sandbox failed to compile the reactor module.");
            return false;
        }
    }

    private void Guard(Action action, string what)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"The client sandbox failed to {what} the reactor.");
            DisposeHost();
        }
    }

    private void WritePackage(string packageName, PackageBuild build)
    {
        var directory = Path.Combine(_stagingDirectory, packageName);
        TryDeleteDirectory(directory);
        Directory.CreateDirectory(directory);
        foreach (var image in build.Assemblies.Values)
        {
            File.WriteAllBytes(Path.Combine(directory, image.Name + ".dll"), image.Image);
        }

        var generation = _generations.TryGetValue(packageName, out var previous) ? previous + 1 : 1;
        _generations[packageName] = generation;
        File.WriteAllText(Path.Combine(directory, GenerationFileName), generation.ToString());
    }

    private void DisposeHost()
    {
        _host?.Dispose();
        _host = null;
    }

    private void ResetStagingDirectory()
    {
        _generations.Clear();
        TryDeleteDirectory(_stagingDirectory);
        Directory.CreateDirectory(_stagingDirectory);
    }

    private static void TryDeleteDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"The client sandbox failed to remove {directory}.");
        }
    }
}
#endif
