using NanosSharp.Entities.Binding;
using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Entities;

/// <summary>Replaces the classes used to wrap entities, so they can carry own state and behaviour.</summary>
public static class EntityFactory
{
    /// <summary>Wraps every entity of the base scripting class with <typeparamref name="TEntity"/>.</summary>
    public static void Register<TEntity>() where TEntity : Entity => Register(typeof(TEntity));

    /// <summary>Wraps every entity of the base scripting class with instances created by the factory.</summary>
    public static void Register<TEntity>(Func<TEntity> factory) where TEntity : Entity
    {
        ArgumentNullException.ThrowIfNull(factory);
        MainThread.Ensure();
        EntityRegistry.AddFactory(new FactoryRegistration(typeof(TEntity), factory, PackageContext.Current));
    }

    /// <summary>Removes the factory of <typeparamref name="TEntity"/>, existing entities fall back to the previous class.</summary>
    public static void Unregister<TEntity>() where TEntity : Entity
    {
        MainThread.Ensure();
        EntityRegistry.RemoveFactories(registration => registration.EntityType == typeof(TEntity));
    }

    internal static void Register(Type entityType)
    {
        MainThread.Ensure();
        if (entityType.IsAbstract || !entityType.IsSubclassOf(typeof(Entity)))
        {
            throw new ArgumentException($"{entityType} must be a non abstract entity class.");
        }

        Func<Entity> create = () => (Entity)Activator.CreateInstance(entityType, nonPublic: true)!;
        EntityRegistry.AddFactory(new FactoryRegistration(entityType, create, PackageContext.Current));
    }
}
