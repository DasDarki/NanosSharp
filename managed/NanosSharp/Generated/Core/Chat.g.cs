#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
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
    private static readonly StaticMethodSlot BroadcastMessageSlot = new("Chat", "BroadcastMessage");
    private static readonly StaticMethodSlot SendMessageSlot = new("Chat", "SendMessage");

    /// <summary>Sends a chat message to all Players</summary>
    public static void BroadcastMessage(string message) => ScriptApi.Invoke(BroadcastMessageSlot, message);

    /// <summary>Sends a chat message to a Player only</summary>
    public static void SendMessage(Player player, string message) => ScriptApi.Invoke(SendMessageSlot, player, message);

    /// <summary>Called when a player submits a message in the chat</summary>
    public delegate bool PlayerSubmitHandler(string? message, Player? player);
}
