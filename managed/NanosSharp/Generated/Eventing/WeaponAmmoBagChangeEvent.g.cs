#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When the Ammo Bag is changed, by reloading or manually setting through scripting</summary>
public sealed class WeaponAmmoBagChangeEvent : ScriptEvent
{
    /// <summary>The Weapon that triggered the event</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>The previous bag ammo</summary>
    public int OldAmmoBag { get; init; }

    /// <summary>The new bag ammo</summary>
    public int NewAmmoBag { get; init; }
}
