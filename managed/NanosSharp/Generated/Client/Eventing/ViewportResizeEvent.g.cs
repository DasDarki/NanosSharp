#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the screen is resized</summary>
public sealed class ViewportResizeEvent : ScriptEvent
{
    /// <summary>The new screen size</summary>
    public Vector2D NewSize { get; init; }
}
