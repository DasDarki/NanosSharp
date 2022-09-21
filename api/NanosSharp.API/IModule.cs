namespace NanosSharp.API;

/// <summary>
/// The interface represents a managed C# module which can be load into any luaVM offering the possibility to extend
/// the luaVM with C# code or interact with lua code directly from C#.
/// </summary>
public interface IModule
{
    /// <summary>
    /// The name of this module.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The version of this module.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets called when the module is being initialized for a given <see cref="ILuaVM"/>.
    /// </summary>
    /// <param name="vm">The interface representing the managed part of the target luaVM.</param>
    void Initialize(ILuaVM vm);
}