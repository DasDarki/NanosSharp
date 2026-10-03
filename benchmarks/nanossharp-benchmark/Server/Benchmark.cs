using System.Diagnostics;

public sealed class Benchmark : Package
{
    private static readonly int Iterations =
        int.TryParse(Environment.GetEnvironmentVariable("NANOSSHARP_BENCH_ITERATIONS"), out var iterations) ? iterations : 20000;
    private const int WarmupIterations = 1000;

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
        Log.Info("[TEST] SUMMARY passed=0 failed=0");
        MainThread.Post(Server.Stop);
    }

    private static void Measure(string name, Action operation)
    {
        if (Environment.GetEnvironmentVariable("NANOSSHARP_BENCH_ONLY") is { Length: > 0 } only && only != name)
        {
            return;
        }

        for (var index = 0; index < WarmupIterations; index++)
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
