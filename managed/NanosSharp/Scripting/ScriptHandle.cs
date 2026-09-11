using NanosSharp.Interop;

namespace NanosSharp.Scripting;

/// <summary>Owns a native script value, it becomes invalid when the scripting engine restarts.</summary>
public abstract class ScriptHandle : IDisposable
{
    /// <summary>Whether the native value is still alive.</summary>
    public bool IsValid => _handle != 0;

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(GetType().Name);

    private nint _handle;

    private protected ScriptHandle(nint handle)
    {
        _handle = handle;
        NativeHandleCollector.Track(this, handle);
    }

    ~ScriptHandle()
    {
        var handle = Interlocked.Exchange(ref _handle, 0);
        if (handle != 0)
        {
            NativeHandleCollector.Enqueue(handle);
        }
    }

    /// <summary>Releases the native value.</summary>
    public void Dispose()
    {
        var handle = Interlocked.Exchange(ref _handle, 0);
        if (handle != 0)
        {
            NativeHandleCollector.Release(handle);
        }

        GC.SuppressFinalize(this);
    }

    internal nint Detach() => Interlocked.Exchange(ref _handle, 0);
}
