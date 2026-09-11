using NanosSharp.Entities.Binding;
using NanosSharp.Interop.Reflection;
using NanosSharp.Interop.Values;
using NanosSharp.Logging;
using NanosSharp.Packages;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class of every nanos world scripting entity.</summary>
public abstract class Entity
{
    /// <summary>Whether the entity still exists.</summary>
    public bool IsValid => _instance != 0;

    /// <summary>Name of the scripting class, for example Player or Character.</summary>
    public string ClassName => NativeClass.Name;

    /// <summary>Network id of the entity.</summary>
    public uint Id => Invoke<uint>("GetID");

    /// <summary>Dimension the entity lives in.</summary>
    public uint Dimension
    {
        get => Invoke<uint>("GetDimension");
        set => Invoke("SetDimension", value);
    }

    /// <summary>Raised on the main thread when the entity was destroyed.</summary>
    public event Action<Entity>? Destroyed;

    internal ulong ClassId => NativeClass.Id;

    internal NativeClass NativeClass { get; private set; } = null!;

    private nint _instance;

    /// <summary>Calls a scripting method of this entity by name.</summary>
    public object? Invoke(string method, params ReadOnlySpan<object?> arguments)
    {
        var values = new object?[arguments.Length + 1];
        values[0] = this;
        arguments.CopyTo(values.AsSpan(1));
        return NativeClass.GetMethod(method).Invoke(PackageContext.Environment, values);
    }

    /// <summary>Calls a scripting method of this entity by name and converts its result.</summary>
    public T? Invoke<T>(string method, params ReadOnlySpan<object?> arguments) =>
        ValueConverter.Convert<T>(Invoke(method, arguments));

    /// <summary>Destroys the entity.</summary>
    public void Destroy() => Invoke("Destroy");

    /// <summary>Reads a value stored on the entity.</summary>
    public T? GetValue<T>(string key, T? fallback = default) => Invoke<T>("GetValue", key, fallback);

    /// <summary>Stores a value on the entity, optionally synchronized to clients.</summary>
    public void SetValue(string key, object? value, bool synchronize = false) => Invoke("SetValue", key, value, synchronize);

    /// <summary>Subscribes to an event of this entity, the handler parameters are converted automatically.</summary>
    public ScriptSubscription Subscribe(string eventName, Delegate handler) =>
        ScriptSubscription.Create(handler, function => Invoke("Subscribe", eventName, function),
            function => Invoke("Unsubscribe", eventName, function));

    /// <inheritdoc />
    public override string ToString() => IsValid ? $"{ClassName}({_instance:X})" : $"{ClassName}(destroyed)";

    internal void Attach(NativeClass nativeClass, nint instance)
    {
        NativeClass = nativeClass;
        _instance = instance;
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

    private protected static T Spawn<T>(params ReadOnlySpan<object?> arguments) where T : Entity
    {
        var nativeClass = NativeClassRegistry.Get(EntityRegistry.GetClassName(typeof(T)));
        var constructor = nativeClass.Constructor ??
                          throw new ScriptException($"{nativeClass.Name} cannot be spawned.");
        return constructor.Invoke(PackageContext.Environment, arguments) as T ??
               throw new ScriptException($"{nativeClass.Name} did not return a valid entity.");
    }

    private protected static object? InvokeStatic(Type type, string function, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.Invoke(EntityRegistry.GetClassName(type), function, arguments);
}
