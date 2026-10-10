#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the connection was closed</summary>
public sealed class WebSocketCloseEvent : ScriptEvent
{
    /// <summary>The WebSocket</summary>
    public WebSocket WebSocket { get; init; } = null!;
}
