using NanosSharp.Core;

namespace NanosSharp.Entities;

public partial class Player
{
    /// <summary>Sends a chat message only this player sees.</summary>
    public void SendMessage(string message) => Chat.SendMessage(this, message);
}
