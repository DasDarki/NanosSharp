using System.Collections.Concurrent;
using NanosSharp.API;

namespace NanosSharp.Server.Pool;

/// <summary>
/// The entity pool is a collection of entities that can be reused to avoid frequent allocations and deallocations.
/// </summary>
internal sealed class EntityPool<T>(IEntityFactory<T> factory) where T : NanosUnit
{
    private readonly ConcurrentDictionary<LuaRef, T> _pool = new();
    
    /// <summary>
    /// Returns an entity from the pool or creates a new one if it does not exist in the pool.
    /// </summary>
    public T Get(LuaRef handle)
    {
        if (_pool.TryGetValue(handle, out var entity))
        {
            return entity;
        }

        entity = factory.Create(handle);
        _pool[handle] = entity;
        return entity;
    }

    /// <summary>
    /// Returns the raw <see cref="NanosUnit"/> from the pool without type casting it to a specific entity type.
    /// </summary>
    public NanosUnit GetRaw(LuaRef handle) => Get(handle);
}