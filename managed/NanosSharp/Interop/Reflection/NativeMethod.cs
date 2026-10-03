using NanosSharp.Entities;
using NanosSharp.Interop.Arguments;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Reflection;

internal sealed unsafe class NativeMethod(nint handle)
{
    private const int StackArgumentLimit = 16;
    private const int StackStringBytes = 512;

    public string Name { get; } = NativeRuntime.ReadString(NativeRuntime.Api.FunctionGetName(handle));

    public string Signature => NativeRuntime.TakeString(NativeRuntime.Api.FunctionGetSignature(handle));

    public object? Invoke(nint environment, Entity? self, ReadOnlySpan<object?> head, ReadOnlySpan<object?> tail = default)
    {
        var result = Call(environment, self, head, tail);
        return ResultReader.Read(ref result);
    }

    public T? Invoke<T>(nint environment, Entity? self, ReadOnlySpan<object?> head, ReadOnlySpan<object?> tail = default)
    {
        var result = Call(environment, self, head, tail);
        return ResultReader.Read<T>(ref result);
    }

    private NativeArgument Call(nint environment, Entity? self, ReadOnlySpan<object?> head, ReadOnlySpan<object?> tail)
    {
        MainThread.Ensure();
        var count = head.Length + tail.Length + (self is null ? 0 : 1);
        var buffer = count <= StackArgumentLimit ? stackalloc NativeArgument[count] : new NativeArgument[count];
        var packer = new ArgumentPacker(buffer, head, tail, stackalloc byte[StackStringBytes]);
        try
        {
            if (self is not null)
            {
                packer.AddSelf(self);
            }

            AddAll(ref packer, head);
            AddAll(ref packer, tail);
            return Execute(environment, buffer, packer.Count);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private NativeArgument Execute(nint environment, Span<NativeArgument> arguments, int count)
    {
        NativeArgument result;
        byte* error = null;
        fixed (NativeArgument* pointer = arguments)
        {
            var succeeded = NativeRuntime.Api.FunctionCall(handle, environment, pointer, count, &result, &error);
            NativeRuntime.ThrowIfFailed(succeeded, error);
        }

        return result;
    }

    private static void AddAll(ref ArgumentPacker packer, ReadOnlySpan<object?> values)
    {
        foreach (var value in values)
        {
            packer.Add(value);
        }
    }
}
