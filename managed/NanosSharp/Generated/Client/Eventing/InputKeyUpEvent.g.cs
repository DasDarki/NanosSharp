#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>A keyboard key has been released</summary>
public sealed class InputKeyUpEvent : CancellableEvent
{
    /// <summary>The key pressed</summary>
    public string? KeyName { get; init; }

    /// <summary>The amount depressed in case of this is an Axis input</summary>
    public float Delta { get; init; }
}
