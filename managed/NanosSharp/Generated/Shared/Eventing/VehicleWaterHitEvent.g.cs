#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when Vehicle hits something</summary>
public sealed class VehicleWaterHitEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWater VehicleWater { get; init; } = null!;

    /// <summary>The intensity of the Hit normalized by the Vehicle's weight</summary>
    public float ImpactForce { get; init; }

    /// <summary>The impulse direction it hits</summary>
    public Vector NormalImpulse { get; init; }

    /// <summary>The world 3D location of the impact</summary>
    public Vector ImpactLocation { get; init; }

    /// <summary>The Vehicle velocity at the moment it hits</summary>
    public Vector Velocity { get; init; }

    /// <summary>The other actor that hit this Vehicle</summary>
    public Actor? OtherActor { get; init; }
}
