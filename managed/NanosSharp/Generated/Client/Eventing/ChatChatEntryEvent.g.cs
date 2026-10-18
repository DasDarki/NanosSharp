#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a new Chat Message is received, this is also triggered when new messages are sent programatically</summary>
public sealed class ChatChatEntryEvent : ScriptEvent
{
    /// <summary>The message</summary>
    public string? Message { get; init; }

    /// <summary>The player who sent the message or <c>nil</c> if this was called on client side or was sent through code</summary>
    public Player? Player { get; init; }
}
