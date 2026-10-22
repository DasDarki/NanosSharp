#if NANOSSHARP_WASM
using System.Runtime.InteropServices;
using NanosSharp.Interop.Arguments;

namespace NanosSharp.Interop.Wasm;

/// <summary>
/// Host-called microbenchmark entry points. Each runs its inner loop inside the
/// sandbox so the native host can time a single export call and divide by the
/// iteration count to get the per-operation cost of a real seam crossing.
/// </summary>
internal static unsafe class WasmBench
{
    private static volatile int _sink;

    [UnmanagedCallersOnly(EntryPoint = "bench_import")]
    public static void BenchImport(int iterations)
    {
        var total = 0;
        for (var i = 0; i < iterations; i++)
        {
            total += NativeRuntime.Api.EngineIsAttached();
        }

        _sink = total;
    }

    [UnmanagedCallersOnly(EntryPoint = "bench_value")]
    public static void BenchValue(int iterations)
    {
        long total = 0;
        for (var i = 0; i < iterations; i++)
        {
            var handle = NativeRuntime.Api.ValueCreateInteger(i);
            total += NativeRuntime.Api.ValueGetInteger(handle);
            NativeRuntime.Api.ValueFree(handle);
        }

        _sink = (int)total;
    }

    [UnmanagedCallersOnly(EntryPoint = "bench_funccall")]
    public static void BenchFunctionCall(int iterations)
    {
        NativeArgument result = default;
        byte* error = null;
        var total = 0;
        for (var i = 0; i < iterations; i++)
        {
            total += NativeRuntime.Api.FunctionCall((nint)1, 0, null, 0, &result, &error);
        }

        _sink = total;
    }

    [UnmanagedCallersOnly(EntryPoint = "bench_compute")]
    public static long BenchCompute(int iterations)
    {
        long accumulator = 0;
        for (var i = 0; i < iterations; i++)
        {
            accumulator += (long)i * 3 + (accumulator & 7);
        }

        return accumulator;
    }
}
#endif
