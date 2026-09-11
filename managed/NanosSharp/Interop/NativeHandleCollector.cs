using System.Collections.Concurrent;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Interop;

internal static unsafe class NativeHandleCollector
{
    private static readonly ConcurrentQueue<nint> PendingReleases = new();
    private static readonly ConcurrentDictionary<nint, WeakReference<ScriptHandle>> LiveHandles = new();

    public static void Track(ScriptHandle owner, nint handle) => LiveHandles[handle] = new WeakReference<ScriptHandle>(owner);

    public static void Enqueue(nint handle) => PendingReleases.Enqueue(handle);

    public static void Release(nint handle)
    {
        if (!MainThread.IsCurrent)
        {
            Enqueue(handle);
            return;
        }

        if (LiveHandles.TryRemove(handle, out _))
        {
            NativeRuntime.Api.ValueFree(handle);
        }
    }

    public static void Collect()
    {
        while (PendingReleases.TryDequeue(out var handle))
        {
            Release(handle);
        }
    }

    public static void ReleaseAll()
    {
        Collect();
        foreach (var (handle, owner) in LiveHandles)
        {
            if (owner.TryGetTarget(out var target))
            {
                target.Detach();
            }

            Release(handle);
        }
    }
}
