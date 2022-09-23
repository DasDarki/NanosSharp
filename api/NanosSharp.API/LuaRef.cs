namespace NanosSharp.API;

public struct LuaRef
{
    private readonly int _index;
    
    public LuaRef(int index)
    {
        _index = index;
    }

    public static implicit operator int(LuaRef luaRef) => luaRef._index;
    public static implicit operator LuaRef(int index) => new(index);
}