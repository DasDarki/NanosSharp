#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Prop takes Damage</summary>
public sealed class PropTakeDamageEvent : ScriptEvent
{
    /// <summary>the object that takes the damage</summary>
    public Prop Prop { get; init; } = null!;

    /// <summary>amount of damage</summary>
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
    public Actor? Causer { get; init; }
}
