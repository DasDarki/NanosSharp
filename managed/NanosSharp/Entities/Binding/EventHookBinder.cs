using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using NanosSharp.Scripting;

namespace NanosSharp.Entities.Binding;

internal static class EventHookBinder
{
    private const BindingFlags InstanceMethods = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly ConditionalWeakTable<Type, EventHook[]> Hooks = [];

    public static List<ScriptSubscription>? Bind(Entity entity)
    {
        var hooks = Hooks.GetValue(entity.GetType(), FindHooks);
        if (hooks.Length == 0)
        {
            return null;
        }

        var subscriptions = new List<ScriptSubscription>(hooks.Length);
        foreach (var hook in hooks)
        {
            subscriptions.Add(entity.Subscribe(hook.EventName, hook.Handler));
        }

        return subscriptions;
    }

    private static EventHook[] FindHooks(Type type)
    {
        if (type.Assembly == typeof(Entity).Assembly)
        {
            return [];
        }

        return type.GetMethods(InstanceMethods)
            .Where(method => method.IsVirtual && method.DeclaringType != method.GetBaseDefinition().DeclaringType)
            .Select(method => method.GetBaseDefinition())
            .Select(definition => (Definition: definition, Attribute: definition.GetCustomAttribute<EventHookAttribute>()))
            .Where(entry => entry.Attribute is not null)
            .OrderBy(entry => entry.Attribute!.EventName, StringComparer.Ordinal)
            .Select(entry => new EventHook(entry.Attribute!.EventName, CreateHandler(entry.Definition)))
            .ToArray();
    }

    private static Delegate CreateHandler(MethodInfo hook)
    {
        var self = Expression.Parameter(hook.DeclaringType!, "self");
        var arguments = hook.GetParameters().Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray();
        var call = Expression.Call(self, hook, arguments);
        var delegateType = Expression.GetDelegateType([self.Type, .. arguments.Select(argument => argument.Type), hook.ReturnType]);
        return Expression.Lambda(delegateType, call, [self, .. arguments]).Compile();
    }

    private sealed record EventHook(string EventName, Delegate Handler);
}
