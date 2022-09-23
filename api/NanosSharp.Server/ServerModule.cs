using NanosSharp.API;

namespace NanosSharp.Server;

/// <summary>
/// The server module loads the managed C# packages for the server side and provides the server side API.
/// </summary>
public class ServerModule : IModule
{
    /// <summary>
    /// Gets called by NanosSharp runtime when the module is loaded.
    /// </summary>
    public void Initialize(ILuaVM vm)
    {
    }
}