using NanosSharp.Interop;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Scripting;

namespace NanosSharp.Host.Packages;

internal static unsafe class PackageManager
{
    private const string ApiDumpEnvironmentVariable = "NANOSSHARP_DUMP_API";
    private static readonly Dictionary<nint, ManagedPackage> Packages = [];

    public static string RuntimeDirectory { get; private set; } = string.Empty;

    public static void Initialize(string runtimeDirectory)
    {
        RuntimeDirectory = runtimeDirectory;
        NativeCallbacks.EngineAttached += OnEngineAttached;
        NativeCallbacks.EngineDetaching += OnEngineDetaching;
        NativeCallbacks.PackageLoaded += OnPackageLoaded;
        NativeCallbacks.PackageUnloading += OnPackageUnloading;
    }

    private static void OnEngineAttached()
    {
        if (Environment.GetEnvironmentVariable(ApiDumpEnvironmentVariable) is { Length: > 0 } path)
        {
            ScriptApi.DumpApi(path);
            Log.Info($"Scripting API dumped to {path}.");
        }
    }

    private static void OnEngineDetaching()
    {
        foreach (var package in Packages.Values.ToArray())
        {
            Stop(package);
        }
    }

    private static void OnPackageLoaded(NativePackage nativePackage)
    {
        if (!ManagedPackage.HasManagedCode(nativePackage.Directory))
        {
            return;
        }

        if (Packages.TryGetValue(nativePackage.Handle, out var existing))
        {
            Stop(existing);
        }

        var package = new ManagedPackage(nativePackage);
        package.SourcesChanged += OnSourcesChanged;
        Packages[package.Handle] = package;
        try
        {
            package.Start();
        }
        catch (Exception exception)
        {
            using (PackageContext.Enter(package.Scope))
            {
                Log.Exception(exception, "Failed to start the C# package");
            }

            Stop(package);
        }
    }

    private static void OnPackageUnloading(nint handle)
    {
        if (Packages.TryGetValue(handle, out var package))
        {
            Stop(package);
        }
    }

    private static void OnSourcesChanged(ManagedPackage package)
    {
        if (!Packages.ContainsKey(package.Handle))
        {
            return;
        }

        Log.Info($"Changes detected, reloading package {package.Name}...");
        byte* error = null;
        var succeeded = NativeRuntime.Api.PackageReload(package.Handle, &error);
        if (succeeded == 0)
        {
            Log.Error($"Reloading package {package.Name} failed: {NativeRuntime.TakeString(error)}");
        }
    }

    private static void Stop(ManagedPackage package)
    {
        Packages.Remove(package.Handle);
        package.SourcesChanged -= OnSourcesChanged;
        try
        {
            package.Stop();
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"Failed to stop C# package {package.Name}");
        }
    }
}
