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

    /// <summary>Calls a scripting method of this entity by name.</summary>
    public object? Invoke(string method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).InvokeMember(PackageContext.Environment, this, arguments);

    /// <summary>Calls a scripting method of this entity by name and converts its result.</summary>
    public T? Invoke<T>(string method, params ReadOnlySpan<object?> arguments) =>
        NativeClass.GetMethod(method).InvokeMember<T>(PackageContext.Environment, this, arguments);

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

    internal void Unbind() => _instance = 0;

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
        try
        {
            Destroyed?.Invoke(this);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"Unhandled exception in {ClassName}.Destroyed");
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
            constructor.Invoke(PackageContext.Environment, arguments);
        }

        if (!IsValid)
        {
            throw new ScriptException($"{className} did not spawn an entity.");
        }
    }
}
