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

    public object? Invoke(nint environment, ReadOnlySpan<object?> arguments)
    {
        var result = Call(environment, null, arguments);
        return ResultReader.Read(ref result);
    }

    public T? Invoke<T>(nint environment, ReadOnlySpan<object?> arguments)
    {
        var result = Call(environment, null, arguments);
        return ResultReader.Read<T>(ref result);
    }

    public object? InvokeMember(nint environment, Entity self, ReadOnlySpan<object?> arguments)
    {
        var result = Call(environment, self, arguments);
        return ResultReader.Read(ref result);
    }

    public T? InvokeMember<T>(nint environment, Entity self, ReadOnlySpan<object?> arguments)
    {
        var result = Call(environment, self, arguments);
        return ResultReader.Read<T>(ref result);
    }

    private NativeArgument Call(nint environment, Entity? self, ReadOnlySpan<object?> arguments)
    {
        MainThread.Ensure();
        var count = arguments.Length + (self is null ? 0 : 1);
        var buffer = count <= StackArgumentLimit ? stackalloc NativeArgument[count] : new NativeArgument[count];
        Span<byte> strings = stackalloc byte[StackStringBytes];
        var packer = new ArgumentPacker(buffer, arguments, strings);
        try
        {
            if (self is not null)
            {
                packer.AddSelf(self);
            }

            foreach (var argument in arguments)
            {
                packer.Add(argument);
            }

            NativeArgument result;
            byte* error = null;
            fixed (NativeArgument* pointer = buffer)
            {
                var succeeded = NativeRuntime.Api.FunctionCall(handle, environment, pointer, packer.Count, &result, &error);
                NativeRuntime.ThrowIfFailed(succeeded, error);
            }

            return result;
        }
        finally
        {
            packer.Dispose();
        }
    }
}
