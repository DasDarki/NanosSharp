#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class for all Damageable entities. It provides Health and Damage related methods and events.</summary>
public partial interface IDamageable
{

    /// <summary>Gets the current health</summary>
    uint Health { get; set; }

    /// <summary>Gets the Max Health</summary>
    uint MaxHealth { get; set; }

    /// <summary>Returns if the entity is dead</summary>
    bool Dead { get; }

    /// <summary>Do damage to this entity</summary>
    uint ApplyDamage(uint damage, string? boneName = null, DamageType? damageType = null, Vector? fromDirection = null, Player? instigator = null, Entity? causer = null);

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    float GetDamageMultiplier(string boneName);

    /// <summary>Gets the current health</summary>
    uint GetHealth();

    /// <summary>Gets the Max Health</summary>
    uint GetMaxHealth();

    /// <summary>Returns if the entity is dead</summary>
    bool IsDead();

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    void Respawn(Vector? location = null, Rotator? rotation = null);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    void SetDamageMultiplier(string boneName, float multiplier);

    /// <summary>Sets the Health of this Entity</summary>
    void SetHealth(uint newHealth);

    /// <summary>Sets the MaxHealth of this Entity</summary>
    void SetMaxHealth(uint maxHealth);
}
