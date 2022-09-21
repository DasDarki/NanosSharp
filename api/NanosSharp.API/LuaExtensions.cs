namespace NanosSharp.API;

/// <summary>
/// Adds some extensions to the <see cref="ILuaVM"/> interface.
/// </summary>
public static class LuaExtensions
{
    /// <summary>
    /// Clears the current stack of this luaVM.
    /// </summary>
    public static void ClearStack(this ILuaVM lua)
    {
        int top = lua.GetTop();
        lua.Pop(top);
    }
}