#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a player submits a message in the chat</summary>
public sealed class ChatPlayerSubmitEvent : CancellableEvent
{
    /// <summary>The message sent by the player</summary>
    public string? Message { get; init; }

    /// <summary>The player who sent the message - on client it will always be the localplayer</summary>
    public Player? Player { get; init; }
}
