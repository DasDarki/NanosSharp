using NanosSharp.Entities.Binding;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class of every nanos world scripting entity.</summary>
public abstract partial class Entity
{
    private const string NetworkIdMethod = "GetID";

    /// <summary>Whether the entity still exists.</summary>
    public bool IsValid => _instance != 0;

    /// <summary>Name of the scripting class, for example Player or Character.</summary>
    public string ClassName => NativeClass.Name;

    /// <summary>Raised on the main thread when the entity was destroyed.</summary>
    public event Action<Entity>? Destroyed;

    internal ulong ClassId => NativeClass.Id;

    internal NativeClass NativeClass { get; private set; } = null!;

    internal uint NetworkId { get; private set; }

    private nint _instance;
    private List<ScriptSubscription>? _hooks;

    /// <summary>Calls a scripting method of this entity by name.</summary>
    public object? Invoke(string method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).Invoke(PackageContext.Environment, this, arguments);

    /// <summary>Calls a scripting method of this entity by name and converts its result.</summary>
    public T? Invoke<T>(string method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).Invoke<T>(PackageContext.Environment, this, arguments);

    /// <summary>Reads a value stored on the entity and converts it.</summary>
    public T? GetValue<T>(string key, T? fallback = default) => Invoke<T>("GetValue", key, fallback);

    /// <summary>Subscribes to an event of this entity, the handler parameters are converted automatically.</summary>
    public ScriptSubscription Subscribe(string eventName, Delegate handler) =>
        ScriptSubscription.Create(handler, function => Invoke("Subscribe", eventName, function),
            function => UnsubscribeIfValid(eventName, function));

    /// <inheritdoc />
    public override string ToString() => IsValid ? $"{ClassName}({_instance:X})" : $"{ClassName}(destroyed)";

    internal void Attach(NativeClass nativeClass, nint instance)
    {
        NativeClass = nativeClass;
        _instance = instance;
        _hooks = EventHookBinder.Bind(this);
    }

    internal void RemoveHandlers(PackageScope scope)
    {
        foreach (var handler in Destroyed?.GetInvocationList() ?? [])
        {
            if (scope.Owns(handler))
            {
                Destroyed -= (Action<Entity>)handler;
            }
        }
    }

    internal void Unbind()
    {
        ReleaseHooks();
        _instance = 0;
    }

    internal void RememberNetworkId(uint networkId)
    {
        if (networkId != 0)
        {
            NetworkId = networkId;
        }
    }

    internal uint ResolveNetworkId()
    {
        if (NetworkId == 0 && NativeClass.Methods.ContainsKey(NetworkIdMethod))
        {
            NetworkId = Invoke<uint>(NetworkIdMethod);
        }

        return NetworkId;
    }

    internal nint EnsureValid() =>
        _instance != 0 ? _instance : throw new ObjectDisposedException(ClassName, "The entity was destroyed.");

    internal void Invalidate()
    {
        if (_instance == 0)
        {
            return;
        }

        _instance = 0;
        ReleaseHooks();
        try
        {
            Destroyed?.Invoke(this);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"Unhandled exception in {ClassName}.Destroyed");
        }
    }

    private protected void Invoke(MethodSlot method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).Invoke(PackageContext.Environment, this, arguments);

    private protected T? Invoke<T>(MethodSlot method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).Invoke<T>(PackageContext.Environment, this, arguments);

    private protected void InvokeVariadic(MethodSlot method, ReadOnlySpan<object?> arguments, ReadOnlySpan<object?> variadic) =>
        NativeClass.GetMethod(method).Invoke(PackageContext.Environment, this, arguments, variadic);

    private protected T? InvokeVariadic<T>(MethodSlot method, ReadOnlySpan<object?> arguments, ReadOnlySpan<object?> variadic) =>
        NativeClass.GetMethod(method).Invoke<T>(PackageContext.Environment, this, arguments, variadic);

    private void ReleaseHooks()
    {
        var hooks = _hooks;
        _hooks = null;
        foreach (var subscription in hooks ?? [])
        {
            subscription.Dispose();
        }
    }

    private void UnsubscribeIfValid(string eventName, ScriptFunction function)
    {
        if (IsValid)
        {
            Invoke("Unsubscribe", eventName, function);
        }
    }

    private protected void SpawnSelf(string className, ReadOnlySpan<object?> arguments)
    {
        var nativeClass = NativeClassRegistry.Get(className);
        var constructor = nativeClass.Constructor ?? throw new ScriptException($"{className} cannot be spawned.");
        using (EntityRegistry.ExpectSpawn(this))
        {
            constructor.Invoke(PackageContext.Environment, null, arguments);
        }

        if (!IsValid)
        {
            throw new ScriptException($"{className} did not spawn an entity.");
        }
    }
}
