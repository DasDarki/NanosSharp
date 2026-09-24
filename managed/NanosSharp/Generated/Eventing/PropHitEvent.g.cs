#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Prop hits something</summary>
public sealed class PropHitEvent : ScriptEvent
{
    /// <summary>The prop which has been hit</summary>
    public Prop Prop { get; init; } = null!;

    /// <summary>The intensity of the Hit normalized by the Prop's weight</summary>
    public float ImpactForce { get; init; }

    /// <summary>The impulse direction it hits</summary>
    public Vector NormalImpulse { get; init; }

    /// <summary>The world 3D location of the impact</summary>
    public Vector ImpactLocation { get; init; }

    /// <summary>The Prop velocity at the moment it hits</summary>
    public Vector Velocity { get; init; }

    /// <summary>The other actor that hit this Prop</summary>
    public Actor? OtherActor { get; init; }
}
