#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character presses the use button for this Pickable (i.e. clicks left mouse button with this equipped)</summary>
public sealed class WeaponPullUseEvent : ScriptEvent
{
    /// <summary>The Pickable which has just been used</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>The Character that used it</summary>
    public Character? Character { get; init; }
}
