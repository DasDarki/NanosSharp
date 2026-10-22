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

    public static event Action<nint, string>? ClientFilesStoring;

    public static event Action<nint>? ClientFilesStored;

#if CLIENT
    private static ScriptSubscription? _clientTick;
    internal static Func<long, nint, int, nint>? FunctionInvoker;
    internal static Action<long>? FunctionReleaser;
    internal static Action<ulong, nint, uint>? EntityCreatedHandler;
    internal static Action<ulong, nint>? EntityDestroyedHandler;
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
        callbacks->ClientFilesStoring = &OnClientFilesStoring;
        callbacks->ClientFilesStored = &OnClientFilesStored;
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
    private static void OnEngineAttached() => OnEngineAttachedCore();

    private static void OnEngineAttachedCore() => Guard(() =>
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
    private static void OnEngineDetaching() => OnEngineDetachingCore();

    private static void OnEngineDetachingCore() => Guard(() =>
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
    private static void OnTick(float deltaTime) => DispatchTick(deltaTime);

    internal static void DispatchEngineAttached() => OnEngineAttachedCore();

    internal static void DispatchEngineDetaching() => OnEngineDetachingCore();

    internal static void DispatchTick(float deltaTime) => Guard(() => RunTick(deltaTime));

    [UnmanagedCallersOnly]
    private static void OnEntityCreated(ulong classId, nint instance, uint networkId)
    {
#if CLIENT
        if (EntityCreatedHandler is { } handler)
        {
            Guard(() => handler(classId, instance, networkId));
            return;
        }
#endif
        Guard(() => EntityRegistry.OnCreated(classId, instance, networkId));
    }

    [UnmanagedCallersOnly]
    private static void OnEntityDestroyed(ulong classId, nint instance)
    {
#if CLIENT
        if (EntityDestroyedHandler is { } handler)
        {
            Guard(() => handler(classId, instance));
            return;
        }
#endif
        Guard(() => EntityRegistry.OnDestroyed(instance));
    }

    [UnmanagedCallersOnly]
    private static nint OnInvokeFunction(long id, NativeArgument* arguments, int count)
    {
#if CLIENT
        if (FunctionInvoker is { } invoker)
        {
            try
            {
                return invoker(id, (nint)arguments, count);
            }
            catch (Exception exception)
            {
                Log.Exception(exception);
                return 0;
            }
        }
#endif
        return DispatchInvokeFunction(id, arguments, count);
    }

    [UnmanagedCallersOnly]
    private static void OnReleaseFunction(long id)
    {
#if CLIENT
        if (FunctionReleaser is { } releaser)
        {
            Guard(() => releaser(id));
            return;
        }
#endif
        DispatchReleaseFunction(id);
    }

    internal static nint DispatchInvokeFunction(long id, NativeArgument* arguments, int count)
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

    internal static void DispatchReleaseFunction(long id) => Guard(() => FunctionRegistry.Release(id));

    internal static void DispatchEntityCreated(ulong classId, nint instance, uint networkId) =>
        Guard(() => EntityRegistry.OnCreated(classId, instance, networkId));

    internal static void DispatchEntityDestroyed(ulong classId, nint instance) =>
        Guard(() => EntityRegistry.OnDestroyed(instance));

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

    [UnmanagedCallersOnly]
    private static void OnClientFilesStoring(nint package, byte* directory) =>
        Guard(() => ClientFilesStoring?.Invoke(package, NativeRuntime.ReadString(directory)));

    [UnmanagedCallersOnly]
    private static void OnClientFilesStored(nint package) => Guard(() => ClientFilesStored?.Invoke(package));

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
