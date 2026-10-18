using System.Runtime.InteropServices;
using NanosSharp.Interop.Arguments;
using NanosSharp.Entities.Binding;
using NanosSharp.Interop.Reflection;
using NanosSharp.Logging;
using NanosSharp.Packages;
#if CLIENT
using NanosSharp.Scripting;
#endif
using NanosSharp.Threading;

namespace NanosSharp.Interop.Callbacks;

internal static unsafe class NativeCallbacks
{
    private const string GlobalEnvironmentName = "nanossharp";

    public static event Action? EngineAttached;

    public static event Action? EngineDetaching;

    public static event Action<float>? Tick;

    public static event Action<NativePackage>? PackageLoaded;

    public static event Action<nint>? PackageUnloading;

#if CLIENT
    private static ScriptSubscription? _clientTick;
#endif

    public static void Register(ManagedCallbacks* callbacks)
    {
        callbacks->EngineAttached = &OnEngineAttached;
        callbacks->EngineDetaching = &OnEngineDetaching;
        callbacks->Tick = &OnTick;
        callbacks->EntityCreated = &OnEntityCreated;
        callbacks->EntityDestroyed = &OnEntityDestroyed;
        callbacks->InvokeFunction = &OnInvokeFunction;
        callbacks->ReleaseFunction = &OnReleaseFunction;
        callbacks->PackageLoaded = &OnPackageLoaded;
        callbacks->PackageUnloading = &OnPackageUnloading;
    }

    public static nint CreateEnvironment(string name)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(name);
        fixed (byte* pointer = bytes)
        {
            return NativeRuntime.Api.EnvironmentCreate(pointer, bytes.Length);
        }
    }

    public static void DestroyEnvironment(string name)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(name);
        fixed (byte* pointer = bytes)
        {
            NativeRuntime.Api.EnvironmentDestroy(pointer, bytes.Length);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnEngineAttached() => Guard(() =>
    {
        MainThread.Capture();
        NativeClassRegistry.Load();
        PackageContext.GlobalEnvironment = CreateEnvironment(GlobalEnvironmentName);
#if CLIENT
        _clientTick = ScriptApi.Subscribe("Client", "Tick", (Action<float>)(deltaTime => Guard(() => RunTick(deltaTime))));
#endif
        EngineAttached?.Invoke();
    });

    [UnmanagedCallersOnly]
    private static void OnEngineDetaching() => Guard(() =>
    {
        try
        {
            EngineDetaching?.Invoke();
        }
        finally
        {
#if CLIENT
            _clientTick?.Dispose();
            _clientTick = null;
#endif
            NativeHandleCollector.ReleaseAll();
            EntityRegistry.Clear();
            if (PackageContext.GlobalEnvironment != 0)
            {
                DestroyEnvironment(GlobalEnvironmentName);
                PackageContext.GlobalEnvironment = 0;
            }

            NativeClassRegistry.Clear();
        }
    });

    [UnmanagedCallersOnly]
    private static void OnTick(float deltaTime) => Guard(() => RunTick(deltaTime));

    [UnmanagedCallersOnly]
    private static void OnEntityCreated(ulong classId, nint instance, uint networkId) =>
        Guard(() => EntityRegistry.OnCreated(classId, instance, networkId));

    [UnmanagedCallersOnly]
    private static void OnEntityDestroyed(ulong classId, nint instance) =>
        Guard(() => EntityRegistry.OnDestroyed(instance));

    [UnmanagedCallersOnly]
    private static nint OnInvokeFunction(long id, NativeArgument* arguments, int count)
    {
        try
        {
            return FunctionRegistry.Invoke(id, arguments, count);
        }
        catch (Exception exception)
        {
            Log.Exception(exception);
            return 0;
        }
    }

    [UnmanagedCallersOnly]
    private static void OnReleaseFunction(long id) => Guard(() => FunctionRegistry.Release(id));

    [UnmanagedCallersOnly]
    private static void OnPackageLoaded(nint package, byte* name, byte* directory, nint environment) => Guard(() =>
        PackageLoaded?.Invoke(new NativePackage(package, NativeRuntime.ReadString(name),
            NativeRuntime.ReadString(directory), environment)));

    [UnmanagedCallersOnly]
    private static void OnPackageUnloading(nint package) => Guard(() => PackageUnloading?.Invoke(package));

    private static void RunTick(float deltaTime)
    {
        NativeHandleCollector.Collect();
        MainThread.Drain();
        TimerScheduler.Tick();
        Tick?.Invoke(deltaTime);
    }

    private static void Guard(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Log.Exception(exception);
        }
    }
}
