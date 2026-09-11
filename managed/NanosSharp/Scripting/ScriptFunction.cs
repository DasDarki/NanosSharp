using NanosSharp.Interop;
using NanosSharp.Interop.Values;
using NanosSharp.Threading;

namespace NanosSharp.Scripting;

/// <summary>A function created by a script, for example a Lua callback passed to an event.</summary>
public sealed unsafe class ScriptFunction : ScriptHandle
{
    internal ScriptFunction(nint handle) : base(handle)
    {
    }

    /// <summary>Invokes the function and returns its first result.</summary>
    public object? Invoke(params ReadOnlySpan<object?> arguments)
    {
        MainThread.Ensure();
        using var values = new NativeValueList(arguments);
        nint result;
        byte* error = null;
        var succeeded = NativeRuntime.Api.ValueInvoke(Handle, values.Pointer, values.Count, &result, &error);
        NativeRuntime.ThrowIfFailed(succeeded, error);
        return ValueMarshaller.TakeFromNative(result);
    }

    /// <summary>Invokes the function and converts its first result.</summary>
    public T? Invoke<T>(params ReadOnlySpan<object?> arguments) => ValueConverter.Convert<T>(Invoke(arguments));
}
