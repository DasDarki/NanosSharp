using System.Reflection;
using NanosSharp.Interop.Reflection;
using NanosSharp.Packages;

namespace NanosSharp.Entities.Binding;

internal static class EntityRegistry
{
    private static readonly Dictionary<nint, Entity> Entities = [];
    private static readonly Dictionary<string, Type> EntityTypes = typeof(Entity).Assembly.GetTypes()
        .Where(type => type.IsSubclassOf(typeof(Entity)) && !type.IsAbstract)
        .Select(type => (Type: type, Attribute: type.GetCustomAttribute<ScriptClassAttribute>()))
        .Where(entry => entry.Attribute is not null)
        .ToDictionary(entry => entry.Attribute!.Name, entry => entry.Type, StringComparer.Ordinal);

    private static Entity? _pendingSpawn;

    public static IEnumerable<Entity> All => Entities.Values;

    public static string GetClassName(Type type) =>
        type.GetCustomAttribute<ScriptClassAttribute>(inherit: true)?.Name
        ?? throw new ArgumentException($"{type} is not bound to a scripting class.");

    public static PendingSpawn ExpectSpawn(Entity entity)
    {
        var previous = _pendingSpawn;
        _pendingSpawn = entity;
        return new PendingSpawn(previous);
    }

    public static Entity? Resolve(ulong classId, nint instance)
    {
        if (instance == 0)
        {
            return null;
        }

        return Entities.TryGetValue(instance, out var entity) ? entity : Register(classId, instance);
    }

    public static void OnCreated(ulong classId, nint instance)
    {
        if (!Entities.ContainsKey(instance))
        {
            Register(classId, instance);
        }
    }

    public static void OnDestroyed(nint instance)
    {
        if (Entities.Remove(instance, out var entity))
        {
            entity.Invalidate();
        }
    }

    public static void ReleaseHandlers(PackageScope scope)
    {
        foreach (var entity in Entities.Values)
        {
            entity.RemoveHandlers(scope);
        }
    }

    public static void Clear()
    {
        foreach (var entity in Entities.Values)
        {
            entity.Invalidate();
        }

        Entities.Clear();
    }

    private static Entity? Register(ulong classId, nint instance)
    {
        var nativeClass = NativeClassRegistry.Find(classId);
        if (nativeClass is null)
        {
            return null;
        }

        var entity = TakePendingSpawn(nativeClass.Name) ?? CreateWrapper(nativeClass.Name);
        entity.Attach(nativeClass, instance);
        Entities[instance] = entity;
        return entity;
    }

    private static Entity? TakePendingSpawn(string className)
    {
        if (_pendingSpawn is null || GetClassName(_pendingSpawn.GetType()) != className)
        {
            return null;
        }

        var entity = _pendingSpawn;
        _pendingSpawn = null;
        return entity;
    }

    private static Entity CreateWrapper(string className)
    {
        var type = EntityTypes.GetValueOrDefault(className, typeof(GenericEntity));
        return (Entity)Activator.CreateInstance(type, nonPublic: true)!;
    }

    public readonly struct PendingSpawn(Entity? previous) : IDisposable
    {
        public void Dispose() => _pendingSpawn = previous;
    }
}
