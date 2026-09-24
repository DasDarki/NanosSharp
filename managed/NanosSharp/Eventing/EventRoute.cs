using System.Reflection;
using NanosSharp.Logging;
using NanosSharp.Scripting;

namespace NanosSharp.Eventing;

internal sealed class EventRoute : IDisposable
{
    public bool IsEmpty => _handlers.Count == 0;

    private readonly ScriptEventDescriptor _descriptor;
    private readonly List<EventHandlerRegistration> _handlers = [];
    private readonly ScriptSubscription _subscription;

    public EventRoute(ScriptEventDescriptor descriptor)
    {
        _descriptor = descriptor;
        _subscription = ScriptApi.Subscribe(descriptor.ClassName, descriptor.EventName, (Func<object?[], object?>)Dispatch);
    }

    public void Add(EventHandlerRegistration handler)
    {
        _handlers.Add(handler);
        _handlers.Sort((left, right) => left.Options.Priority.CompareTo(right.Options.Priority));
    }

    public void RemoveListener(object listener) => _handlers.RemoveAll(handler => ReferenceEquals(handler.Listener, listener));

    public void Dispose() => _subscription.Dispose();

    private object? Dispatch(object?[] arguments)
    {
        var scriptEvent = _descriptor.Create(arguments);
        foreach (var handler in _handlers.ToArray())
        {
            if (scriptEvent is CancellableEvent { IsCancelled: true } && !handler.Options.ReceiveCancelled)
            {
                continue;
            }

            Invoke(handler, scriptEvent);
        }

        return scriptEvent is CancellableEvent { IsCancelled: true } ? false : null;
    }

    private void Invoke(EventHandlerRegistration handler, ScriptEvent scriptEvent)
    {
        try
        {
            handler.Method.Invoke(handler.Listener, [scriptEvent]);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            Log.Exception(exception.InnerException, $"Unhandled exception in {handler.Method.DeclaringType?.Name}.{handler.Method.Name} for {_descriptor.ClassName}.{_descriptor.EventName}");
        }
    }
}
