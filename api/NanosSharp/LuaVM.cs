using NanosSharp.API;

namespace NanosSharp;

/// <summary>
/// The implementation of <see cref="ILuaVM"/>.
/// </summary>
internal class LuaVM : ILuaVM
{
    /// <summary>
    /// The pointer to the native luaVM.
    /// </summary>
    internal IntPtr Handle { get; }

    internal LuaVM(IntPtr handle)
    {
        Handle = handle;
    }
}