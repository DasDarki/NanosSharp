#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when bullets hit (this will be triggered for each bullet shot)</summary>
public sealed class WeaponBulletHitEvent : ScriptEvent
{
    /// <summary>The Weapon that triggered the event</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>Location in world space of the bullet hit</summary>
    public Vector ImpactPoint { get; init; }

    /// <summary>Normal of the bullet hit in world space</summary>
    public Vector ImpactNormal { get; init; }

    /// <summary>Damage that was applied</summary>
    public int Damage { get; init; }

    /// <summary>Actor entity that was hit, if any</summary>
    public Actor? ActorHit { get; init; }
}
