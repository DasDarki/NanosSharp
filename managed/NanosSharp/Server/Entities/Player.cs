using NanosSharp.Core;

namespace NanosSharp.Entities;

public partial class Player
{
    /// <summary>Whether this player runs a compatible NanosSharp client runtime.</summary>
    public bool HasNanosSharp => NanosSharpClients.Get(this) is not null;

    /// <summary>The NanosSharp version of this player, null when it has no compatible client or did not report it yet.</summary>
    public string? NanosSharpVersion => NanosSharpClients.Get(this)?.Version;

    /// <summary>Sends a chat message only this player sees.</summary>
    public void SendMessage(string message) => Chat.SendMessage(this, message);
}
