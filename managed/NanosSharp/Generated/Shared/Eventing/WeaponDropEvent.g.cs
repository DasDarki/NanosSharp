#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When a Character drops this Pickable</summary>
public sealed class WeaponDropEvent : ScriptEvent
{
    /// <summary>The Pickable which has been dropped</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>The Character that dropped it</summary>
    public Character? Character { get; init; }

    /// <summary>If the Player actively pressed the Drop binding to drop</summary>
    public bool WasTriggeredByPlayer { get; init; }
}
