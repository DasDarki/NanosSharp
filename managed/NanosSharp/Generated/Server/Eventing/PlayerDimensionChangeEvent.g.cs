#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Player changes its dimension</summary>
public sealed class PlayerDimensionChangeEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;

    /// <summary>The previous dimension</summary>
    public int OldDimension { get; init; }

    /// <summary>The new dimension</summary>
    public int NewDimension { get; init; }
}
