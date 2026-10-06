using NanosSharp.Entities;
using NanosSharp.Interop.Arguments;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Reflection;

internal sealed unsafe class NativeMethod(nint handle)
{
    public const int StackArgumentLimit = 16;
    public const int StackStringBytes = 512;

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

    public NativeArgument Execute(nint environment, ref ArgumentPacker packer, Span<NativeArgument> arguments)
    {
        NativeArgument result;
        byte* error = null;
        fixed (NativeArgument* pointer = arguments)
        {
            var succeeded = NativeRuntime.Api.FunctionCall(handle, environment, pointer, packer.Count, &result, &error);
            NativeRuntime.ThrowIfFailed(succeeded, error);
        }

        return result;
    }

    private NativeArgument Call(nint environment, Entity? self, ReadOnlySpan<object?> head, ReadOnlySpan<object?> tail)
    {
        MainThread.Ensure();
        var count = head.Length + tail.Length + (self is null ? 0 : 1);
        var buffer = count <= StackArgumentLimit ? stackalloc NativeArgument[count] : new NativeArgument[count];
        var packer = new ArgumentPacker(buffer, stackalloc byte[StackStringBytes]);
        try
        {
            if (self is not null)
            {
                packer.AddSelf(self);
            }

            AddAll(ref packer, head);
            AddAll(ref packer, tail);
            return Execute(environment, ref packer, buffer);
        }
        finally
        {
            packer.Dispose();
        }
    }

    private static void AddAll(ref ArgumentPacker packer, ReadOnlySpan<object?> values)
    {
        foreach (var value in values)
        {
            packer.Add(value);
        }
    }
}
