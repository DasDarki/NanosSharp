using NanosSharp.API;

namespace NanosSharp.Server.Pool;

internal sealed class DefaultEntityFactory<T>(Func<LuaRef, T> create) : IEntityFactory<T> where T : NanosUnit
{
    public T Create(LuaRef handle) => create(handle);
}