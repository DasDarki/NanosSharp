#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a text message was received</summary>
public sealed class WebSocketReceiveMessageEvent : ScriptEvent
{
    /// <summary>The WebSocket</summary>
    public WebSocket WebSocket { get; init; } = null!;

    /// <summary>The received text message</summary>
    public string? Message { get; init; }
}
