using NanosSharp.Interop;
using NanosSharp.Interop.Callbacks;
using NanosSharp.Packages;

namespace NanosSharp.Scripting;

/// <summary>An active event subscription, dispose it to unsubscribe.</summary>
public sealed class ScriptSubscription : IDisposable
{
    /// <summary>Whether the subscription is still active.</summary>
    public bool IsActive => _function?.IsValid == true;

    internal event Action<ScriptSubscription>? Disposed;

    private readonly Action<ScriptFunction> _unsubscribe;
    private readonly PackageScope? _scope;
    private ScriptFunction? _function;

    private ScriptSubscription(ScriptFunction function, Action<ScriptFunction> unsubscribe, PackageScope? scope)
    {
        _function = function;
        _unsubscribe = unsubscribe;
        _scope = scope;
        _scope?.Subscriptions.Add(this);
    }

    /// <summary>Removes the handler from the event.</summary>
    public void Dispose()
    {
        var function = Interlocked.Exchange(ref _function, null);
        if (function is null)
        {
            return;
        }

        _scope?.Subscriptions.Remove(this);
        if (function.IsValid && NativeRuntime.IsEngineAttached)
        {
            _unsubscribe(function);
        }

        function.Dispose();
        Disposed?.Invoke(this);
    }

    internal static ScriptSubscription Create(Delegate handler, Action<ScriptFunction> subscribe,
        Action<ScriptFunction> unsubscribe)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var function = new ScriptFunction(FunctionRegistry.CreateNative(handler));
        try
        {
            subscribe(function);
        }
        catch
        {
            function.Dispose();
            throw;
        }

        return new ScriptSubscription(function, unsubscribe, PackageContext.Current);
    }
}
