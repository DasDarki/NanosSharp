#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When this Pickable hits something</summary>
public sealed class GrenadeHitEvent : ScriptEvent
{
    /// <summary>The Actor that was hit</summary>
    public Grenade Grenade { get; init; } = null!;

    /// <summary>The intensity of the hit normalized by the Pickable's weight</summary>
    public float ImpactForce { get; init; }

    /// <summary>The impulse direction of the hit</summary>
    public Vector NormalImpulse { get; init; }

    /// <summary>The world space location of the impact</summary>
    public Vector ImpactLocation { get; init; }

    /// <summary>The Pickable's velocity at the moment it hit</summary>
    public Vector Velocity { get; init; }

    /// <summary>The other actor that hit this Pickable</summary>
    public Actor? OtherActor { get; init; }
}
