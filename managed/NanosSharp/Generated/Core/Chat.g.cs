#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Configure, send and intercept chat messages.</summary>
public static partial class Chat
{
    /// <summary>Called when a player submits a message in the chat</summary>
    public static event PlayerSubmitHandler? OnPlayerSubmit
    {
        add => PlayerSubmitEvent.Add(value);
        remove => PlayerSubmitEvent.Remove(value);
    }

    private static readonly ClassEvent<PlayerSubmitHandler> PlayerSubmitEvent = new("Chat", "PlayerSubmit");

    /// <summary>Sends a chat message to all Players</summary>
    public static void BroadcastMessage(string message) => ScriptApi.Invoke("Chat", "BroadcastMessage", message);

    /// <summary>Sends a chat message to a Player only</summary>
    public static void SendMessage(Player player, string message) =>
        ScriptApi.Invoke("Chat", "SendMessage", player, message);

    /// <summary>Called when a player submits a message in the chat</summary>
    public delegate bool PlayerSubmitHandler(string? message, Player? player);
}
