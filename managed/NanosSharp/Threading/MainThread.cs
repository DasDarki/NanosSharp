using System.Collections.Concurrent;
using NanosSharp.Logging;

namespace NanosSharp.Threading;

/// <summary>Access to the server main thread, the only thread allowed to talk to nanos world.</summary>
public static class MainThread
{
    private static readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State, ExecutionContext? Context)> PendingWork = new();
    private static readonly MainThreadSynchronizationContext Context = new();
    private static int _threadId = -1;

    /// <summary>Whether the calling thread is the server main thread.</summary>
    public static bool IsCurrent => Environment.CurrentManagedThreadId == _threadId;

    /// <summary>Queues an action to run on the main thread during the next server tick.</summary>
    public static void Post(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        PendingWork.Enqueue((static state => ((Action)state!)(), action, ExecutionContext.Capture()));
    }

    /// <summary>Runs a function on the main thread and returns its result.</summary>
    public static Task<T> InvokeAsync<T>(Func<T> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        if (IsCurrent)
        {
            return Task.FromResult(function());
        }

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Post(() =>
        {
            try
            {
                completion.SetResult(function());
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        return completion.Task;
    }

    /// <summary>Runs an action on the main thread and completes when it finished.</summary>
    public static Task InvokeAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return InvokeAsync(() =>
        {
            action();
            return true;
        });
    }

    internal static void Capture()
    {
        _threadId = Environment.CurrentManagedThreadId;
        SynchronizationContext.SetSynchronizationContext(Context);
    }

    internal static void Ensure()
    {
        if (!IsCurrent)
        {
            throw new InvalidOperationException(
                "nanos world can only be accessed from the main thread, use MainThread.Post or MainThread.InvokeAsync.");
        }
    }

    internal static void Enqueue(SendOrPostCallback callback, object? state) => PendingWork.Enqueue((callback, state, null));

    internal static void Drain()
    {
        SynchronizationContext.SetSynchronizationContext(Context);
        var budget = PendingWork.Count;
        while (budget-- > 0 && PendingWork.TryDequeue(out var work))
        {
            Execute(work.Callback, work.State, work.Context);
        }
    }

    private static void Execute(SendOrPostCallback callback, object? state, ExecutionContext? context)
    {
        try
        {
            if (context is null)
            {
                callback(state);
                return;
            }

            ExecutionContext.Run(context, new ContextCallback(callback), state);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            Log.Exception(exception);
        }
    }
}
