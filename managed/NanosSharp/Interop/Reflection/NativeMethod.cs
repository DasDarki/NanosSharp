using NanosSharp.Interop.Values;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Reflection;

internal sealed unsafe class NativeMethod(nint handle)
{
    public string Name { get; } = NativeRuntime.ReadString(NativeRuntime.Api.FunctionGetName(handle));

    public string Signature => NativeRuntime.TakeString(NativeRuntime.Api.FunctionGetSignature(handle));

    public object? Invoke(nint environment, ReadOnlySpan<object?> arguments)
    {
        MainThread.Ensure();
        using var values = new NativeValueList(arguments);
        nint result;
        byte* error = null;
        var succeeded = NativeRuntime.Api.FunctionInvoke(handle, environment, values.Pointer, values.Count, &result, &error);
        NativeRuntime.ThrowIfFailed(succeeded, error);
        return ValueMarshaller.TakeFromNative(result);
    }
}
