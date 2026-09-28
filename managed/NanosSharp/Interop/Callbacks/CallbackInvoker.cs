using System.Linq.Expressions;
using System.Reflection;
using NanosSharp.Interop.Values;

namespace NanosSharp.Interop.Callbacks;

internal static class CallbackInvoker
{
    private static readonly Dictionary<Type, Func<Delegate, object?[], object?>> Invokers = [];

    private static readonly MethodInfo ConvertMethod = typeof(ValueConverter).GetMethods()
        .Single(method => method is { Name: nameof(ValueConverter.Convert), IsGenericMethodDefinition: true });

    public static object? Invoke(Delegate callback, object?[] arguments) => GetInvoker(callback.GetType())(callback, arguments);

    private static Func<Delegate, object?[], object?> GetInvoker(Type delegateType)
    {
        if (Invokers.TryGetValue(delegateType, out var invoker))
        {
            return invoker;
        }

        invoker = Compile(delegateType);
        Invokers[delegateType] = invoker;
        return invoker;
    }

    private static Func<Delegate, object?[], object?> Compile(Type delegateType)
    {
        var invokeMethod = delegateType.GetMethod("Invoke")!;
        var callback = Expression.Parameter(typeof(Delegate), "callback");
        var arguments = Expression.Parameter(typeof(object?[]), "arguments");
        var parameters = invokeMethod.GetParameters();

        IEnumerable<Expression> values = parameters is [{ ParameterType: var single }] && single == typeof(object?[])
            ? [arguments]
            : parameters.Select((parameter, index) => BindParameter(parameter, index, arguments));

        Expression call = Expression.Invoke(Expression.Convert(callback, delegateType), values);
        Expression body = invokeMethod.ReturnType == typeof(void)
            ? Expression.Block(call, Expression.Constant(null, typeof(object)))
            : Expression.Convert(call, typeof(object));

        return Expression.Lambda<Func<Delegate, object?[], object?>>(body, callback, arguments).Compile();
    }

    private static Expression BindParameter(ParameterInfo parameter, int index, ParameterExpression arguments)
    {
        var fallback = parameter.HasDefaultValue
            ? Expression.Constant(parameter.DefaultValue, typeof(object))
            : Expression.Constant(null, typeof(object));
        var value = Expression.Condition(
            Expression.LessThan(Expression.Constant(index), Expression.ArrayLength(arguments)),
            Expression.ArrayIndex(arguments, Expression.Constant(index)),
            fallback);
        Expression converted = Expression.Call(ConvertMethod.MakeGenericMethod(parameter.ParameterType), value);
        return converted.Type == parameter.ParameterType ? converted : Expression.Convert(converted, parameter.ParameterType);
    }
}
