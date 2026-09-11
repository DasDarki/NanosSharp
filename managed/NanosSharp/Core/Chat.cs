using NanosSharp.Entities;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>The in game chat.</summary>
public static class Chat
{
    /// <summary>Raised when a player submits a message, return false to block it.</summary>
    public static event Func<string, Player, bool>? PlayerSubmit
    {
        add => PlayerSubmitEvent.Add(value);
        remove => PlayerSubmitEvent.Remove(value);
    }

    private const string ClassName = "Chat";
    private static readonly ClassEvent<Func<string, Player, bool>> PlayerSubmitEvent = new(ClassName, "PlayerSubmit");

    /// <summary>Sends a message to every player.</summary>
    public static void Broadcast(string message) => ScriptApi.Invoke(ClassName, "BroadcastMessage", message);

    /// <summary>Sends a message to one player.</summary>
    public static void SendMessage(Player player, string message) =>
        ScriptApi.Invoke(ClassName, "SendMessage", player, message);
}
