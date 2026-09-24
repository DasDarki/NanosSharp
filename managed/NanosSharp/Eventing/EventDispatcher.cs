using System.Reflection;

namespace NanosSharp.Eventing;

internal sealed class EventDispatcher
{
    private const BindingFlags HandlerFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private readonly Dictionary<Type, EventRoute> _routes = [];

    public int Register(object listener)
    {
        var handlers = FindHandlers(listener.GetType()).ToArray();
        foreach (var (method, options, eventType) in handlers)
        {
            var target = method.IsStatic ? null : listener;
            GetRoute(eventType).Add(new EventHandlerRegistration(target, method, options));
        }

        return handlers.Length;
    }

    public void Unregister(object listener)
    {
        foreach (var route in _routes.Values)
        {
            route.RemoveListener(listener);
        }

        var emptyRoutes = _routes.Where(entry => entry.Value.IsEmpty).ToArray();
        foreach (var (eventType, route) in emptyRoutes)
        {
            route.Dispose();
            _routes.Remove(eventType);
        }
    }

    public void Clear()
    {
        foreach (var route in _routes.Values)
        {
            route.Dispose();
        }

        _routes.Clear();
    }

    private EventRoute GetRoute(Type eventType)
    {
        if (_routes.TryGetValue(eventType, out var route))
        {
            return route;
        }

        var descriptor = ScriptEventCatalog.Find(eventType) ??
                         throw new ArgumentException($"{eventType.Name} is not a nanos world event.");
        route = new EventRoute(descriptor);
        _routes[eventType] = route;
        return route;
    }

    private static IEnumerable<(MethodInfo Method, ServerEventAttribute Options, Type EventType)> FindHandlers(Type type)
    {
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            foreach (var method in current.GetMethods(HandlerFlags))
            {
                var options = method.GetCustomAttribute<ServerEventAttribute>();
                if (options is null)
                {
                    continue;
                }

                yield return (method, options, GetEventType(method));
            }
        }
    }

    private static Type GetEventType(MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length != 1 || !parameters[0].ParameterType.IsSubclassOf(typeof(ScriptEvent)))
        {
            throw new ArgumentException($"{method.DeclaringType?.Name}.{method.Name} must have exactly one event parameter.");
        }

        return parameters[0].ParameterType;
    }
}
