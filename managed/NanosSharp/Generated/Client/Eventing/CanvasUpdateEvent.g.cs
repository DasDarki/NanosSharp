#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the Canvas needs to be paintedYou can only call <c>:Draw...()</c> methods from inside this event</summary>
public sealed class CanvasUpdateEvent : ScriptEvent
{
    /// <summary>The Canvas that triggered the event</summary>
    public Canvas Canvas { get; init; } = null!;

    /// <summary>Width in pixels</summary>
    public int Width { get; init; }

    /// <summary>Height in pixels</summary>
    public int Height { get; init; }
}
