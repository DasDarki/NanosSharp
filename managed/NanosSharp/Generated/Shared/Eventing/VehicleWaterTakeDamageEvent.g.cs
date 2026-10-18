#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Entity takes damage</summary>
public sealed class VehicleWaterTakeDamageEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public VehicleWater VehicleWater { get; init; } = null!;

    /// <summary>Amount of damage taken</summary>
    public int Damage { get; init; }

    /// <summary>Damaged bone</summary>
    public string? Bone { get; init; }

    /// <summary>Damage Type</summary>
    public DamageType Type { get; init; }

    /// <summary>Direction of the damage relative to the damaged actor</summary>
    public Vector FromDirection { get; init; }

    /// <summary>The player which caused the damage</summary>
    public Player? Instigator { get; init; }

    /// <summary>The object which caused the damage</summary>
    public object? Causer { get; init; }
}
