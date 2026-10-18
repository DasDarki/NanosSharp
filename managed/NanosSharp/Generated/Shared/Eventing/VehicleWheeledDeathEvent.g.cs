#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity Dies</summary>
public sealed class VehicleWheeledDeathEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;

    /// <summary>Damage of the hit that killed it</summary>
    public int LastDamageTaken { get; init; }

    /// <summary>Bone hit by the last damage</summary>
    public string? LastBoneDamaged { get; init; }

    /// <summary>Type of the damage that killed it</summary>
    public DamageType DamageTypeReason { get; init; }

    /// <summary>Direction the last damage came from</summary>
    public Vector HitFromDirection { get; init; }

    /// <summary>The Player responsible for the death, if any</summary>
    public Player? Instigator { get; init; }

    /// <summary>The object which caused the damage</summary>
    public Actor? Causer { get; init; }
}
