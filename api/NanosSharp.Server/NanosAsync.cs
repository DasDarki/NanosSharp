namespace NanosSharp.Server;

/// <summary>
/// A utility class for async operations.
/// </summary>
public static class NanosAsync
{
    /// <summary>
    /// Runs the given action on the main thread. If the current thread is the main thread, the action will be executed immediately.
    /// Otherwise, the action will be enqueued to run on the main thread in the next tick.
    /// </summary>
    /// <param name="action"></param>
    public static void RunOnMainThread(Action action)
    {
        if (ServerModule.IsMainThread)
        {
            action();
        }
        else
        {
            ServerModule.RunOnMainThread(action);
        }
    }
    
    /// <summary>
    /// Runs the given function on the main thread and returns a task with the result.
    /// </summary>
    /// <param name="func">The function to run on the main thread.</param>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <returns>A task with the result of the function.</returns>
    public static Task<T> DoOnMainThreadAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();
        
        RunOnMainThread(() =>
        {
            try
            {
                tcs.SetResult(func());
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });
        
        return tcs.Task;
    }
}