using System.Reflection;
using NanosSharp.Interop.Reflection;

namespace NanosSharp.Entities.Binding;

internal static class EntityRegistry
{
    private static readonly Dictionary<nint, Entity> Entities = [];
    private static readonly Dictionary<string, Type> EntityTypes = typeof(Entity).Assembly.GetTypes()
        .Where(type => type.IsSubclassOf(typeof(Entity)) && !type.IsAbstract)
        .Select(type => (Type: type, Attribute: type.GetCustomAttribute<ScriptClassAttribute>()))
        .Where(entry => entry.Attribute is not null)
        .ToDictionary(entry => entry.Attribute!.Name, entry => entry.Type, StringComparer.Ordinal);

    public static IEnumerable<Entity> All => Entities.Values;

    public static string GetClassName(Type type) =>
        type.GetCustomAttribute<ScriptClassAttribute>()?.Name
        ?? throw new ArgumentException($"{type} is not bound to a scripting class.");

    public static Entity? Resolve(ulong classId, nint instance)
    {
        if (instance == 0)
        {
            return null;
        }

        return Entities.TryGetValue(instance, out var entity) ? entity : Register(classId, instance);
    }

    public static Entity? ResolveDeferred(uint networkId) =>
        Entities.Values.FirstOrDefault(entity => entity.IsValid && entity.Invoke<uint>("GetID") == networkId);

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

        var type = EntityTypes.GetValueOrDefault(nativeClass.Name, typeof(GenericEntity));
        var entity = (Entity)Activator.CreateInstance(type, nonPublic: true)!;
        entity.Attach(nativeClass, instance);
        Entities[instance] = entity;
        return entity;
    }
}
