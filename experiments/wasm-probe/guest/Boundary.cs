using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace NanosSharp.Wasm.Probe.Guest;

internal static unsafe partial class Boundary
{
    private const byte TagBool = 0;
    private const byte TagInteger = 1;
    private const byte TagDouble = 2;
    private const byte TagString = 3;
    private const int RecordSize = 32;

    [LibraryImport("nanos", EntryPoint = "function-call")]
    [WasmImportLinkage]
    private static partial long HostFunctionCall(int method, byte* arguments, int length);

    [LibraryImport("nanos", EntryPoint = "next-event")]
    [WasmImportLinkage]
    private static partial int HostNextEvent();

    [LibraryImport("nanos", EntryPoint = "nop")]
    [WasmImportLinkage]
    private static partial int HostNop(int value);

    internal static (long Crossings, double ElapsedMilliseconds) BatchBench(int perBatch, long batches)
    {
        var values = new int[perBatch];
        for (var index = 0; index < perBatch; index++)
        {
            values[index] = index;
        }

        var buffer = PackIntegers(values);
        fixed (byte* pointer = buffer)
        {
            HostFunctionCall(1, pointer, buffer.Length);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (long batch = 0; batch < batches; batch++)
            {
                HostFunctionCall(1, pointer, buffer.Length);
            }

            stopwatch.Stop();
            return (batches, stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private static byte[] PackIntegers(int[] values)
    {
        var buffer = new byte[sizeof(int) + values.Length * RecordSize];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, values.Length);
        for (var index = 0; index < values.Length; index++)
        {
            var record = buffer.AsSpan(sizeof(int) + index * RecordSize, RecordSize);
            record[0] = TagInteger;
            BinaryPrimitives.WriteInt64LittleEndian(record[8..], values[index]);
        }

        return buffer;
    }

    internal static (int Result, double ElapsedMilliseconds) ChattyBench(long calls)
    {
        HostNop(0);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var accumulator = 0;
        for (long index = 0; index < calls; index++)
        {
            accumulator = HostNop(accumulator);
        }

        stopwatch.Stop();
        return (accumulator, stopwatch.Elapsed.TotalMilliseconds);
    }

    internal static int PumpEvents(Action<int> handle)
    {
        var handled = 0;
        int next;
        while ((next = HostNextEvent()) >= 0)
        {
            handle(next);
            handled++;
        }

        return handled;
    }

    internal static long Call(int method, params object[] arguments)
    {
        var buffer = Pack(arguments);
        fixed (byte* pointer = buffer)
        {
            return HostFunctionCall(method, pointer, buffer.Length);
        }
    }

    private static byte[] Pack(object[] arguments)
    {
        var records = new byte[arguments.Length * RecordSize];
        var scratchBase = sizeof(int) + records.Length;
        var scratch = new List<byte>();
        for (var index = 0; index < arguments.Length; index++)
        {
            var record = records.AsSpan(index * RecordSize, RecordSize);
            switch (arguments[index])
            {
                case bool value:
                    record[0] = TagBool;
                    record[8] = (byte)(value ? 1 : 0);
                    break;
                case int value:
                    record[0] = TagInteger;
                    BinaryPrimitives.WriteInt64LittleEndian(record[8..], value);
                    break;
                case long value:
                    record[0] = TagInteger;
                    BinaryPrimitives.WriteInt64LittleEndian(record[8..], value);
                    break;
                case double value:
                    record[0] = TagDouble;
                    BinaryPrimitives.WriteDoubleLittleEndian(record[8..], value);
                    break;
                case string value:
                    var bytes = Encoding.UTF8.GetBytes(value);
                    record[0] = TagString;
                    BinaryPrimitives.WriteInt32LittleEndian(record[8..], scratchBase + scratch.Count);
                    BinaryPrimitives.WriteInt32LittleEndian(record[12..], bytes.Length);
                    scratch.AddRange(bytes);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported argument type {arguments[index].GetType()}.");
            }
        }

        var buffer = new byte[scratchBase + scratch.Count];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, arguments.Length);
        records.CopyTo(buffer.AsSpan(sizeof(int)));
        scratch.CopyTo(buffer, scratchBase);
        return buffer;
    }
}
