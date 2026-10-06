using System.Diagnostics;

public sealed class Benchmark : Package
{
    private static readonly int Iterations =
        int.TryParse(Environment.GetEnvironmentVariable("NANOSSHARP_BENCH_ITERATIONS"), out var iterations) ? iterations : 1000000;

    private static readonly TimeSpan WarmupDuration = TimeSpan.FromMilliseconds(500);

    protected override void OnStart() => Server.OnStart += Run;

    private static void Run()
    {
        var prop = new Prop(new Vector(0, 0, 100), Rotator.Zero, "nanos-world::SM_Cube");
        var location = new Vector(10, 20, 30);
        using var subscription = Events.Subscribe("csharp-benchmark", (long number, string text) => { });

        Measure("static getter", () => Server.GetTickRate());
        Measure("entity getter", () => prop.GetLocation());
        Measure("entity setter", () => prop.SetLocation(location));
        Measure("event roundtrip", () => Events.Call("csharp-benchmark", 1, "text"));

        prop.Destroy();
        MeasureCompute();
        Log.Info("[TEST] SUMMARY passed=0 failed=0");
        MainThread.Post(Server.Stop);
    }

    private static void MeasureCompute()
    {
        var stopwatch = Stopwatch.StartNew();
        const int limit = 2000000;
        var composite = new bool[limit + 1];
        var primes = 0;
        for (var number = 2; number <= limit; number++)
        {
            if (composite[number])
            {
                continue;
            }

            primes++;
            for (var multiple = (long)number * number; multiple <= limit; multiple += number)
            {
                composite[multiple] = true;
            }
        }

        Log.Info($"[BENCH] csharp compute prime sieve: {stopwatch.Elapsed.TotalMilliseconds:F1} ms ({primes} primes)");

        stopwatch.Restart();
        var values = new long[200000];
        long seed = 12345;
        for (var index = 0; index < values.Length; index++)
        {
            seed = (seed * 1103515245 + 12345) % 2147483648;
            values[index] = seed;
        }

        Array.Sort(values);
        Log.Info($"[BENCH] csharp compute sort: {stopwatch.Elapsed.TotalMilliseconds:F1} ms");
    }

    private static void Measure(string name, Action operation)
    {
        if (Environment.GetEnvironmentVariable("NANOSSHARP_BENCH_ONLY") is { Length: > 0 } only && only != name)
        {
            return;
        }

        var warmup = Stopwatch.StartNew();
        while (warmup.Elapsed < WarmupDuration)
        {
            operation();
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        for (var index = 0; index < Iterations; index++)
        {
            operation();
        }

        stopwatch.Stop();
        var nanoseconds = stopwatch.Elapsed.TotalNanoseconds / Iterations;
        var bytes = (GC.GetAllocatedBytesForCurrentThread() - allocated) / Iterations;
        Log.Info($"[BENCH] csharp {name}: {nanoseconds:F0} ns/op, {bytes} bytes/op");
    }
}
