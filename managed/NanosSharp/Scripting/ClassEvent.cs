namespace NanosSharp.Scripting;

internal sealed class ClassEvent<THandler>(string className, string eventName) where THandler : Delegate
{
    private readonly Dictionary<THandler, ScriptSubscription> _subscriptions = [];

    public void Add(THandler? handler)
    {
        if (handler is null || _subscriptions.ContainsKey(handler))
        {
            return;
        }

        var subscription = ScriptApi.Subscribe(className, eventName, handler);
        subscription.Disposed += _ => _subscriptions.Remove(handler);
        _subscriptions[handler] = subscription;
    }

    public void Remove(THandler? handler)
    {
        if (handler is not null && _subscriptions.Remove(handler, out var subscription))
        {
            subscription.Dispose();
        }
    }
}
