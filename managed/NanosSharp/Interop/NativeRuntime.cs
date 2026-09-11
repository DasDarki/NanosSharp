using System.Runtime.InteropServices;
using System.Text;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Logging;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Interop;

internal static unsafe class NativeRuntime
{
    private static NativeApi* _api;

    public static ref readonly NativeApi Api
    {
        get
        {
            if (_api == null)
            {
                throw new InvalidOperationException("The NanosSharp native runtime is not initialized.");
            }

            return ref *_api;
        }
    }

    public static bool IsInitialized => _api != null;

    public static bool IsEngineAttached => _api != null && _api->EngineIsAttached() != 0;

    public static void Initialize(NativeApi* api, ManagedCallbacks* callbacks)
    {
        if (api->Version != NativeApi.SupportedVersion)
        {
            throw new NotSupportedException(
                $"Native API version {api->Version} is not supported, expected {NativeApi.SupportedVersion}.");
        }

        _api = api;
        MainThread.Capture();
        NativeCallbacks.Register(callbacks);
    }

    public static void Log(LogLevel level, string message)
    {
        if (_api == null)
        {
            System.Console.Error.WriteLine($"[NanosSharp] {message}");
            return;
        }

        var length = Encoding.UTF8.GetByteCount(message);
        Span<byte> buffer = length <= 1024 ? stackalloc byte[length] : new byte[length];
        Encoding.UTF8.GetBytes(message, buffer);
        fixed (byte* pointer = buffer)
        {
            _api->Log((int)level, pointer, length);
        }
    }

    public static string ReadString(byte* value) => Marshal.PtrToStringUTF8((nint)value) ?? string.Empty;

    public static string TakeString(byte* value)
    {
        try
        {
            return ReadString(value);
        }
        finally
        {
            Api.FreeString(value);
        }
    }

    public static void ThrowIfFailed(byte succeeded, byte* error)
    {
        if (succeeded != 0)
        {
            return;
        }

        throw new ScriptException(error == null ? "Unknown native error." : TakeString(error));
    }
}
