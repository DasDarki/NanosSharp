#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the mouse moves</summary>
public sealed class InputMouseMoveEvent : CancellableEvent
{
    /// <summary>Horizontal movement since the last event</summary>
    public float CursorDeltaX { get; init; }

    /// <summary>Vertical movement since the last event</summary>
    public float CursorDeltaY { get; init; }

    /// <summary>Mouse X position on screen, in pixels</summary>
    public float MouseX { get; init; }

    /// <summary>Mouse Y position on screen, in pixels</summary>
    public float MouseY { get; init; }
}
