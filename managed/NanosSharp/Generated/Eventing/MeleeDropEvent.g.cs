#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When a Character drops this Pickable</summary>
public sealed class MeleeDropEvent : ScriptEvent
{
    /// <summary>The Pickable which has been dropped</summary>
    public Melee Melee { get; init; } = null!;

    /// <summary>The Character that dropped it</summary>
    public Character? Character { get; init; }

    /// <summary>If the Player actively pressed the Drop binding to drop</summary>
    public bool WasTriggeredByPlayer { get; init; }
}
