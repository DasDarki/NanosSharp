using NanosSharp.Interop.Arguments;
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
        Registrations[id] = new Registration(callback, scope);
        scope?.Functions.Add(id);
        return NativeRuntime.Api.ValueCreateFunction(id, PackageContext.Environment);
    }

    public static nint Invoke(long id, NativeArgument* arguments, int count)
    {
        if (!Registrations.TryGetValue(id, out var registration) || registration.Scope is { IsActive: false })
        {
            return 0;
        }

        using var _ = PackageContext.Enter(registration.Scope);
        try
        {
            var result = CallbackInvoker.Invoke(registration.Callback, (nint)arguments, count);
            return result is null ? 0 : ValueMarshaller.ToNative(result);
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

    private sealed record Registration(Delegate Callback, PackageScope? Scope);
}
