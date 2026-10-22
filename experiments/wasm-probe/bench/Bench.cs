using System.Diagnostics;

namespace NanosSharp.Wasm.Probe.Bench;

internal static class Runner
{
    private static ulong Compute(long iterations)
    {
        ulong x = 0x9E3779B97F4A7C15UL;
        ulong acc = 0;
        for (long index = 0; index < iterations; index++)
        {
            x ^= x >> 12;
            x ^= x << 25;
            x ^= x >> 27;
            acc += x * 0x2545F4914F6CDD1DUL;
        }

        return acc;
    }

    public static int Run(string[] args)
    {
        var iterations = args.Length > 0 ? long.Parse(args[0]) : 20_000_000L;
        Compute(1000);
        var stopwatch = Stopwatch.StartNew();
        var result = Compute(iterations);
        stopwatch.Stop();
        Console.WriteLine($"result={result:x16} elapsed_ms={stopwatch.Elapsed.TotalMilliseconds:F1}");
        return 0;
    }
}
