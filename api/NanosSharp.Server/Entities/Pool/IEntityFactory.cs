using NanosSharp.API;

namespace NanosSharp.Server.Pool;

/// <summary>
/// The IEntityFactory interface is used to create instances of nanos world entities.
/// </summary>
/// <typeparam name="T">The type of the entity to create, which must inherit from <see cref="NanosUnit"/>.</typeparam>
public interface IEntityFactory<out T> where T : NanosUnit
{
    /// <summary>
    /// Creates an instance of the entity using the provided LuaRef handle.
    /// </summary>
    /// <param name="handle">The handle to the Lua object that represents the entity.</param>
    /// <returns>The created entity instance.</returns>
    T Create(LuaRef handle);
}