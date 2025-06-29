using NanosSharp.API;
using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Package class in C#. In the C# context this class is also an abstract
/// class for all C# packages to inherit from. Each main class must inherit from this class to be recognized as one
/// and there can only be one main class per package.
/// </summary>
/// <remarks>
/// Some static functions are not fully implemented yet or need native implementation through calling Lua Stack API
/// functions due to C#/Lua interop limitations.
/// </remarks>
public partial class Package
{
    /// <summary>
    /// Exports a variable to the Lua environment.
    /// </summary>
    /// <param name="variable_name">The name of the variable to export.</param>
    /// <param name="value">The value to export.</param>
    /// <exception cref="NotImplementedException">If the type of value is not supported for export (see remarks).</exception>
    public static void Export(string variable_name, object value)
    {
        switch (value)
        {
            case null:
            case Enum:
            case LuaRef:
            case ILuaUnit:
            case ILuaVM.CFunction:
            case bool:
            case double:
            case int:
            case long:
            case string:
            case float:
                break;
            default:
                throw new NotImplementedException("Exporting this type is not supported yet: " + value.GetType());
                
        }
        
        ServerModule.EnsureMainThread();
        
        BPackage.Export(ServerModule.VM, variable_name, value);
    }
    
    /// <summary>
    /// Requires a Lua script file and returns the result of the script execution.
    /// </summary>
    /// <param name="script_file">The path to the Lua script file to require.</param>
    /// <param name="force_load">Whether to force the script to be loaded again, even if it has already been loaded.</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// This is like the remarks section not fully C# implemented yet. It can be called but it might return raw Lua objects.
    /// </remarks>
    public static object Require(string script_file, bool? force_load = null)
    {
        ServerModule.EnsureMainThread();
        
        return BPackage.Require(ServerModule.VM, script_file, force_load);
    }
}