using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Interop.Reflection;
using NanosSharp.Logging;
using NanosSharp.Packages;

namespace NanosSharp.Entities.Binding;

internal static class EntityRegistry
{
    private static readonly Dictionary<nint, Entity> Entities = [];
    private static readonly Dictionary<string, List<FactoryRegistration>> Factories = new(StringComparer.Ordinal);
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

    public static Entity? Resolve(ulong classId, nint instance, uint networkId = 0)
    {
        if (instance == 0)
        {
            return null;
        }

        var entity = Entities.TryGetValue(instance, out var existing) ? existing : Register(classId, instance);
        entity?.RememberNetworkId(networkId);
        return entity;
    }

    public static void OnCreated(ulong classId, nint instance, uint networkId) => Resolve(classId, instance, networkId);

    public static void OnDestroyed(nint instance)
    {
        if (Entities.Remove(instance, out var entity))
        {
            entity.Invalidate();
        }
    }

    public static void AddFactory(FactoryRegistration registration)
    {
        var className = GetClassName(registration.EntityType);
        if (!Factories.TryGetValue(className, out var registrations))
        {
            registrations = [];
            Factories[className] = registrations;
        }

        if (registrations.Count > 0)
        {
            Log.Warning($"{registration.EntityType.Name} replaces {registrations[^1].EntityType.Name} as wrapper for {className} entities.");
        }

        registrations.Add(registration);
        Rebind(className, entity => entity.GetType() == DefaultTypeOf(className) || IsFactoryType(entity.GetType()));
    }

    public static void RemoveFactories(Func<FactoryRegistration, bool> predicate)
    {
        foreach (var (className, registrations) in Factories.ToArray())
        {
            var removed = registrations.Where(predicate).Select(registration => registration.EntityType).ToHashSet();
            if (removed.Count == 0)
            {
                continue;
            }

            registrations.RemoveAll(registration => removed.Contains(registration.EntityType));
            Rebind(className, entity => removed.Contains(entity.GetType()));
        }
    }

    public static void ReleaseScope(PackageScope scope)
    {
        RemoveFactories(registration => registration.Scope == scope);
        var ownedClasses = Entities.Values
            .Where(entity => scope.LoadContext is not null && AssemblyLoadContext.GetLoadContext(entity.GetType().Assembly) == scope.LoadContext)
            .Select(entity => entity.ClassName)
            .Distinct()
            .ToArray();
        foreach (var className in ownedClasses)
        {
            Rebind(className, entity => AssemblyLoadContext.GetLoadContext(entity.GetType().Assembly) == scope.LoadContext);
        }

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
        if (Factories.TryGetValue(className, out var registrations) && registrations.Count > 0)
        {
            return registrations[^1].Create();
        }

        return (Entity)Activator.CreateInstance(DefaultTypeOf(className), nonPublic: true)!;
    }

    private static Type DefaultTypeOf(string className) => EntityTypes.GetValueOrDefault(className, typeof(GenericEntity));

    private static bool IsFactoryType(Type type) =>
        Factories.Values.Any(registrations => registrations.Any(registration => registration.EntityType == type));

    private static void Rebind(string className, Func<Entity, bool> shouldRebind)
    {
        var affected = Entities.Where(entry => entry.Value.ClassName == className && shouldRebind(entry.Value)).ToArray();
        foreach (var (instance, previous) in affected)
        {
            var replacement = CreateWrapper(className);
            replacement.Attach(previous.NativeClass, instance);
            replacement.RememberNetworkId(previous.NetworkId);
            previous.Unbind();
            Entities[instance] = replacement;
        }
    }

    public readonly struct PendingSpawn(Entity? previous) : IDisposable
    {
        public void Dispose() => _pendingSpawn = previous;
    }
}
