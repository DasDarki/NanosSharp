using NanosSharp.Entities;
using NanosSharp.Interop.Values;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Reflection;

internal sealed unsafe class NativeProperty(nint handle)
{
    public string Name { get; } = NativeRuntime.ReadString(NativeRuntime.Api.PropertyGetName(handle));

    public string Signature => NativeRuntime.TakeString(NativeRuntime.Api.PropertyGetSignature(handle));

    public object? Get(Entity self)
    {
        MainThread.Ensure();
        using var values = new NativeValueList([self]);
        nint result;
        byte* error = null;
        NativeRuntime.ThrowIfFailed(NativeRuntime.Api.PropertyGet(handle, values.Pointer[0], &result, &error), error);
        return ValueMarshaller.TakeFromNative(result);
    }

    public void Set(Entity self, object? value)
    {
        MainThread.Ensure();
        using var values = new NativeValueList([self, value]);
        byte* error = null;
        var succeeded = NativeRuntime.Api.PropertySet(handle, values.Pointer[0], values.Pointer[1], &error);
        NativeRuntime.ThrowIfFailed(succeeded, error);
    }
}
