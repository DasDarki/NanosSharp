#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Actor changes its dimension</summary>
public sealed class CharacterDimensionChangeEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The previous dimension</summary>
    public int OldDimension { get; init; }

    /// <summary>The new dimension</summary>
    public int NewDimension { get; init; }
}
