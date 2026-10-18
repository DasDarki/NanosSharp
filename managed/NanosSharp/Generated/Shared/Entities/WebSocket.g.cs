#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A WebSocket client connection to an external server.</summary>
[ScriptClass("WebSocket")]
public partial class WebSocket : Entity
{
    private static readonly MethodSlot CloseSlot = new("Close");
    private static readonly MethodSlot SendSlot = new("Send");

    /// <summary>Spawns a new WebSocket.</summary>
    public WebSocket(string url, IDictionary<string, string>? headers = null)
    {
        SpawnSelf("WebSocket", ScriptArguments.Trim([url, headers], 1));
    }

    protected WebSocket()
    {
    }

    /// <summary>Closes the connection and destroys the WebSocket</summary>
    public void Close() => Call(CloseSlot, 0);

    /// <summary>Sends a text message, returns false if the connection is not open</summary>
    public bool Send(string message) => CallResult<bool, string>(SendSlot, 1, message)!;
}
