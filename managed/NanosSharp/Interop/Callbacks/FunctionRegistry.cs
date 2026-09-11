using System.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Interop.Callbacks;

internal static unsafe class FunctionRegistry
{
    private static readonly Dictionary<long, Registration> Registrations = [];
    private static long _nextId;

    public static int Count => Registrations.Count;

    public static nint CreateNative(Delegate callback)
    {
        MainThread.Ensure();
        var scope = PackageContext.Current;
        var id = ++_nextId;
        Registrations[id] = new Registration(callback, scope, callback.Method.GetParameters());
        scope?.Functions.Add(id);
        return NativeRuntime.Api.ValueCreateFunction(id, PackageContext.Environment);
    }

    public static nint Invoke(long id, nint arguments, int count)
    {
        if (!Registrations.TryGetValue(id, out var registration) || registration.Scope is { IsActive: false })
        {
            return 0;
        }

        using var _ = PackageContext.Enter(registration.Scope);
        try
        {
            var values = ValueMarshaller.ReadArguments(arguments, count);
            var result = registration.Callback.DynamicInvoke(registration.Bind(values));
            return result is null ? 0 : ValueMarshaller.ToNative(result);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            Log.Exception(exception.InnerException, "Unhandled exception in script callback");
        }
        catch (Exception exception)
        {
            Log.Exception(exception, "Unhandled exception in script callback");
        }

        return 0;
    }

    public static void Release(long id)
    {
        if (Registrations.Remove(id, out var registration))
        {
            registration.Scope?.Functions.Remove(id);
        }
    }

    public static void ReleaseScope(PackageScope scope)
    {
        foreach (var id in scope.Functions)
        {
            Registrations.Remove(id);
        }

        scope.Functions.Clear();
    }

    private sealed record Registration(Delegate Callback, PackageScope? Scope, ParameterInfo[] Parameters)
    {
        public object?[] Bind(object?[] values)
        {
            if (Parameters is [{ ParameterType: var type }] && type == typeof(object?[]))
            {
                return [values];
            }

            var arguments = new object?[Parameters.Length];
            for (var index = 0; index < Parameters.Length; index++)
            {
                var parameter = Parameters[index];
                arguments[index] = index < values.Length
                    ? ValueConverter.Convert(values[index], parameter.ParameterType)
                    : parameter.HasDefaultValue
                        ? parameter.DefaultValue
                        : ValueConverter.Convert(null, parameter.ParameterType);
            }

            return arguments;
        }
    }
}
