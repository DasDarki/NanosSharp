using System.Linq.Expressions;
using System.Reflection;
using NanosSharp.Interop.Arguments;

namespace NanosSharp.Interop.Callbacks;

internal static class CallbackInvoker
{
    private static readonly Dictionary<Type, Func<Delegate, nint, int, object?>> Invokers = [];

    private static readonly MethodInfo GetMethod = typeof(ArgumentAccess).GetMethod(nameof(ArgumentAccess.Get))!;

    private static readonly MethodInfo GetAllMethod = typeof(ArgumentAccess).GetMethod(nameof(ArgumentAccess.GetAll))!;

    public static object? Invoke(Delegate callback, nint arguments, int count) => GetInvoker(callback.GetType())(callback, arguments, count);

    private static Func<Delegate, nint, int, object?> GetInvoker(Type delegateType)
    {
        if (Invokers.TryGetValue(delegateType, out var invoker))
        {
            return invoker;
        }

        invoker = Compile(delegateType);
        Invokers[delegateType] = invoker;
        return invoker;
    }

    private static Func<Delegate, nint, int, object?> Compile(Type delegateType)
    {
        var invokeMethod = delegateType.GetMethod("Invoke")!;
        var callback = Expression.Parameter(typeof(Delegate), "callback");
        var arguments = Expression.Parameter(typeof(nint), "arguments");
        var count = Expression.Parameter(typeof(int), "count");
        var parameters = invokeMethod.GetParameters();

        IEnumerable<Expression> values = parameters is [{ ParameterType: var single }] && single == typeof(object?[])
            ? [Expression.Call(GetAllMethod, arguments, count)]
            : parameters.Select((parameter, index) => BindParameter(parameter, index, arguments, count));

        Expression call = Expression.Invoke(Expression.Convert(callback, delegateType), values);
        Expression body = invokeMethod.ReturnType == typeof(void)
            ? Expression.Block(call, Expression.Constant(null, typeof(object)))
            : Expression.Convert(call, typeof(object));

        return Expression.Lambda<Func<Delegate, nint, int, object?>>(body, callback, arguments, count).Compile();
    }

    private static Expression BindParameter(ParameterInfo parameter, int index, ParameterExpression arguments,
        ParameterExpression count)
    {
        var type = parameter.ParameterType;
        Expression value = Expression.Call(GetMethod.MakeGenericMethod(type), arguments, count, Expression.Constant(index));
        if (!parameter.HasDefaultValue)
        {
            return value;
        }

        var isPresent = Expression.LessThan(Expression.Constant(index), count);
        return Expression.Condition(isPresent, value, Expression.Constant(parameter.DefaultValue, type));
    }
}
