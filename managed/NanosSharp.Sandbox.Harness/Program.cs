using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NanosSharp.Interop;
using NanosSharp.Interop.Arguments;
using NanosSharp.Sandbox;

namespace NanosSharp.Sandbox.Harness;

internal static unsafe class Program
{
    private const string PackageName = "smoke";

    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("usage: harness <core-module.wasm> <package-dir> [bench]");
            return 2;
        }

        var module = args[0];
        var sourcePackage = args[1];
        var bench = args.Length >= 3 && args[2] == "bench";

        var api = (NativeApi*)NativeMemory.Alloc((nuint)sizeof(NativeApi));
        *api = MockEngine.Build();

        try
        {
            if (bench)
            {
                using var benchHost = new WasmHost(api, module, sourcePackage);
                benchHost.Boot();
                return RunBenchmarks(benchHost);
            }

            var staging = Directory.CreateTempSubdirectory("nanossharp-harness-").FullName;
            try
            {
                StagePackage(staging, sourcePackage, 1);
                using var host = new WasmHost(api, module, staging);
                host.Boot();

                Console.WriteLine(">> engine attached");
                host.EngineAttached();

                Console.WriteLine(">> sync: load");
                host.SyncPackages();

                Console.WriteLine(">> invoke callback (simulated engine event)");
                InvokeClientTick(host, 0.5f);

                Console.WriteLine(">> sync: unload");
                Directory.Delete(Path.Combine(staging, PackageName), recursive: true);
                host.SyncPackages();

                Console.WriteLine(">> sync: reload");
                StagePackage(staging, sourcePackage, 2);
                host.SyncPackages();

                for (var i = 0; i < 3; i++)
                {
                    host.Tick(0.016f);
                }

                host.EngineDetaching();
                Console.WriteLine("wasm-host-test: host-driven callbacks completed");
                return 0;
            }
            finally
            {
                TryDelete(staging);
            }
        }
        finally
        {
            NativeMemory.Free(api);
        }
    }

    private static void InvokeClientTick(WasmHost host, float delta)
    {
        if (MockEngine.CreatedFunctions.Count == 0)
        {
            Console.WriteLine("(no subscribed function to invoke)");
            return;
        }

        var id = MockEngine.CreatedFunctions[^1];
        var argument = new NativeArgument { Type = (byte)2, Number = delta };
        host.InvokeFunction(id, &argument, 1);
    }

    private static void StagePackage(string staging, string source, int generation)
    {
        var directory = Path.Combine(staging, PackageName);
        Directory.CreateDirectory(directory);
        foreach (var dll in Directory.GetFiles(source, "*.dll"))
        {
            File.Copy(dll, Path.Combine(directory, Path.GetFileName(dll)), overwrite: true);
        }

        File.WriteAllText(Path.Combine(directory, ".gen"), generation.ToString());
    }

    private static void TryDelete(string directory)
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch
        {
        }
    }

    private static int RunBenchmarks(WasmHost host)
    {
        var benches = new (string Name, string Label, int Iterations, int PerOp, bool Result)[]
        {
            ("bench_compute", "compute loop (interpreter)", 20_000_000, 1, true),
            ("bench_import", "import crossing (engine-is-attached)", 5_000_000, 1, false),
            ("bench_value", "value round trip (create+get+free)", 2_000_000, 3, false),
            ("bench_funccall", "function-call (0 args, int result)", 2_000_000, 1, false),
        };

        foreach (var bench in benches)
        {
            Invoke(host, bench.Name, 10_000);
        }

        foreach (var bench in benches)
        {
            var start = Stopwatch.GetTimestamp();
            Invoke(host, bench.Name, bench.Iterations);
            var elapsed = Stopwatch.GetElapsedTime(start);
            var nanoseconds = elapsed.TotalNanoseconds / bench.Iterations / bench.PerOp;
            Console.WriteLine($"[bench] {bench.Label,-40} {nanoseconds,8:F2} ns/op");
        }

        const int calls = 500_000;
        var hostStart = Stopwatch.GetTimestamp();
        for (var i = 0; i < calls; i++)
        {
            Invoke(host, "bench_compute", 0);
        }

        var hostElapsed = Stopwatch.GetElapsedTime(hostStart);
        var perCall = hostElapsed.TotalNanoseconds / calls;
        Console.WriteLine($"[bench] {"host->guest export call",-40} {perCall,8:F2} ns/op");
        Console.WriteLine("wasm-host-test: benchmarks completed");
        return 0;
    }

    private static void Invoke(WasmHost host, string name, int iterations)
    {
        var function = host.GetExport(name)
            ?? throw new InvalidOperationException($"reactor does not export {name}");
        function.Invoke(iterations);
    }
}
