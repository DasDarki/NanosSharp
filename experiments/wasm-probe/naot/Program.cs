using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static unsafe partial class Program
{
    private const byte TagInteger = 1;
    private const int RecordSize = 32;

    [LibraryImport("nanos", EntryPoint = "nop")]
    [WasmImportLinkage]
    private static partial int HostNop(int value);

    [LibraryImport("nanos", EntryPoint = "function-call")]
    [WasmImportLinkage]
    private static partial long HostFunctionCall(int method, byte* arguments, int length);

    private static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "chatty")
        {
            Chatty(long.Parse(args[1]));
            return 0;
        }

        if (args.Length >= 3 && args[0] == "batch")
        {
            Batch(int.Parse(args[1]), long.Parse(args[2]));
            return 0;
        }

        Console.WriteLine("naot ready");
        return 0;
    }

    private static void Chatty(long calls)
    {
        HostNop(0);
        var stopwatch = Stopwatch.StartNew();
        var accumulator = 0;
        for (long index = 0; index < calls; index++)
        {
            accumulator = HostNop(accumulator);
        }

        stopwatch.Stop();
        Console.WriteLine($"result={accumulator} elapsed_ms={stopwatch.Elapsed.TotalMilliseconds:F1}");
    }

    private static void Batch(int perBatch, long batches)
    {
        var buffer = PackIntegers(perBatch);
        fixed (byte* pointer = buffer)
        {
            HostFunctionCall(1, pointer, buffer.Length);
            var stopwatch = Stopwatch.StartNew();
            for (long batch = 0; batch < batches; batch++)
            {
                HostFunctionCall(1, pointer, buffer.Length);
            }

            stopwatch.Stop();
            Console.WriteLine($"crossings={batches} values={batches * perBatch} elapsed_ms={stopwatch.Elapsed.TotalMilliseconds:F1}");
        }
    }

    private static byte[] PackIntegers(int perBatch)
    {
        var buffer = new byte[sizeof(int) + perBatch * RecordSize];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, perBatch);
        for (var index = 0; index < perBatch; index++)
        {
            var record = buffer.AsSpan(sizeof(int) + index * RecordSize, RecordSize);
            record[0] = TagInteger;
            BinaryPrimitives.WriteInt64LittleEndian(record[8..], index);
        }

        return buffer;
    }
}
