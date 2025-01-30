namespace NanosSharp.API;

public struct LuaRef(int index)
{
    private readonly int _index = index;

    public static implicit operator int(LuaRef luaRef) => luaRef._index;
    public static implicit operator long(LuaRef luaRef) => luaRef._index;
    public static implicit operator LuaRef(int index) => new(index);
}