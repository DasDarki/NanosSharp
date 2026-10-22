#if NANOSSHARP_WASM
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NanosSharp.Interop.Wasm;

/// <summary>
/// Registers a persistent buffer the native host writes string results into.
/// In the sandbox a <see cref="Arguments.NativeArgument"/> string result cannot
/// point at host memory, so the host copies the bytes into this guest-owned
/// scratch and sets the offset; the managed reader consumes it synchronously
/// before the next call reuses it.
/// </summary>
internal static unsafe partial class WasmResultScratch
{
    private const int Capacity = 64 * 1024;

    [LibraryImport("nanos", EntryPoint = "set-result-scratch")]
    [WasmImportLinkage]
    private static partial void SetResultScratch(byte* buffer, int capacity);

    public static void Register()
    {
        var buffer = (byte*)NativeMemory.Alloc(Capacity);
        SetResultScratch(buffer, Capacity);
    }
}
#endif
