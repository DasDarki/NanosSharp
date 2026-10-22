#if NANOSSHARP_WASM
using System.Runtime.InteropServices;
using NanosSharp.Interop.Arguments;

namespace NanosSharp.Interop.Callbacks;

/// <summary>
/// Host-callable entry points for the engine to managed callback direction.
/// Inside the sandbox the engine cannot hold managed function pointers, so each
/// callback is a named wasm export the native host invokes instead. Handles cross
/// as i32 table indices and callback arguments are read from the guest linear
/// memory offset the host writes them to.
/// </summary>
internal static unsafe class WasmExports
{
    [UnmanagedCallersOnly(EntryPoint = "nanos_engine_attached")]
    public static void EngineAttached() => NativeCallbacks.DispatchEngineAttached();

    [UnmanagedCallersOnly(EntryPoint = "nanos_engine_detaching")]
    public static void EngineDetaching() => NativeCallbacks.DispatchEngineDetaching();

    [UnmanagedCallersOnly(EntryPoint = "nanos_tick")]
    public static void Tick(float deltaTime) => NativeCallbacks.DispatchTick(deltaTime);

    [UnmanagedCallersOnly(EntryPoint = "nanos_invoke_function")]
    public static int InvokeFunction(long id, int argumentsOffset, int count) =>
        (int)NativeCallbacks.DispatchInvokeFunction(id, (NativeArgument*)(nint)argumentsOffset, count);

    [UnmanagedCallersOnly(EntryPoint = "nanos_release_function")]
    public static void ReleaseFunction(long id) => NativeCallbacks.DispatchReleaseFunction(id);

    [UnmanagedCallersOnly(EntryPoint = "nanos_entity_created")]
    public static void EntityCreated(long classId, int instance, int networkId) =>
        NativeCallbacks.DispatchEntityCreated((ulong)classId, (nint)instance, (uint)networkId);

    [UnmanagedCallersOnly(EntryPoint = "nanos_entity_destroyed")]
    public static void EntityDestroyed(long classId, int instance) =>
        NativeCallbacks.DispatchEntityDestroyed((ulong)classId, (nint)instance);
}
#endif
