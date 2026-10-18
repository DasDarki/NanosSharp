#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>A mouse button has been released</summary>
public sealed class InputMouseUpEvent : CancellableEvent
{
    /// <summary>The key name, e.g. <c>E</c> or <c>LeftMouseButton</c></summary>
    public string? KeyName { get; init; }

    /// <summary>Mouse X position on screen, in pixels</summary>
    public float MouseX { get; init; }

    /// <summary>Mouse Y position on screen, in pixels</summary>
    public float MouseY { get; init; }
}
