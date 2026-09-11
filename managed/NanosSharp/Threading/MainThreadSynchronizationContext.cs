namespace NanosSharp.Threading;

internal sealed class MainThreadSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback callback, object? state) => MainThread.Enqueue(callback, state);

    public override void Send(SendOrPostCallback callback, object? state)
    {
        if (MainThread.IsCurrent)
        {
            callback(state);
            return;
        }

        MainThread.InvokeAsync(() => callback(state)).GetAwaiter().GetResult();
    }

    public override SynchronizationContext CreateCopy() => this;
}
