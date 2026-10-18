#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the mouse scrolls</summary>
public sealed class InputMouseScrollEvent : CancellableEvent
{
    /// <summary>Mouse X position on screen, in pixels</summary>
    public float MouseX { get; init; }

    /// <summary>Mouse Y position on screen, in pixels</summary>
    public float MouseY { get; init; }

    /// <summary>Scroll amount, positive when scrolling up and negative when scrolling down</summary>
    public float Delta { get; init; }
}
