#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class VehicleWater : IDamageable
{
    /// <summary>Triggered when a Character attempts to enter the Vehicle</summary>
    public static event CharacterAttemptEnterHandler? OnCharacterAttemptEnter
    {
        add => CharacterAttemptEnterEvent.Add(value);
        remove => CharacterAttemptEnterEvent.Remove(value);
    }

    /// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
    public static event CharacterAttemptLeaveHandler? OnCharacterAttemptLeave
    {
        add => CharacterAttemptLeaveEvent.Add(value);
        remove => CharacterAttemptLeaveEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<CharacterAttemptEnterHandler> CharacterAttemptEnterEvent = new("VehicleWater", "CharacterAttemptEnter");
    private static readonly ClassEvent<CharacterAttemptLeaveHandler> CharacterAttemptLeaveEvent = new("VehicleWater", "CharacterAttemptLeave");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("VehicleWater", "DimensionChange");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("VehicleWater", "NetworkAuthorityChange");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetDoorSlot = new("SetDoor");
    private static readonly MethodSlot SetEngineOffsetSlot = new("SetEngineOffset");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetInvulnerableSlot = new("SetInvulnerable");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetThrustStrengthSlot = new("SetThrustStrength");

    /// <summary>Gets the current health</summary>
    public uint Health
    {
        get => GetHealth();
        set => SetHealth(value);
    }

    /// <summary>Gets the Max Health</summary>
    public uint MaxHealth
    {
        get => GetMaxHealth();
        set => SetMaxHealth(value);
    }

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool Invulnerable
    {
        get => IsInvulnerable();
        set => SetInvulnerable(value);
    }

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        CallResult<uint, uint, string?, DamageType?, Vector?, Player?, Entity?>(ApplyDamageSlot, 1, damage, boneName, damageType, fromDirection, instigator, causer)!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) =>
        CallResult<float, string>(GetDamageMultiplierSlot, 1, boneName)!;

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) => Call(RespawnSlot, 0, location, rotation);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Call(SetDamageMultiplierSlot, 2, boneName, multiplier);

    /// <summary>Adds a Door to the Vehicle</summary>
    public void SetDoor(
        byte seatIndex,
        Vector offsetLocation,
        Vector seatLocation,
        Rotator seatRotation,
        ushort triggerRadius,
        ushort leaveLateralOffset) =>
        Call(SetDoorSlot, 6, seatIndex, offsetLocation, seatLocation, seatRotation, triggerRadius, leaveLateralOffset);

    /// <summary>Sets the relative location of the Engine (where the Physics Thruster, sounds and effects are attached)</summary>
    public void SetEngineOffset(Vector offset) => Call(SetEngineOffsetSlot, 1, offset);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Call(SetHealthSlot, 1, newHealth);

    /// <summary>Sets if the Character can receive any damage</summary>
    public void SetInvulnerable(bool isInvulnerable) => Call(SetInvulnerableSlot, 1, isInvulnerable);

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Call(SetMaxHealthSlot, 1, maxHealth);

    /// <summary>Sets the force of the Engine/Thruster</summary>
    public void SetThrustStrength(ushort force) => Call(SetThrustStrengthSlot, 1, force);

    /// <summary>Subscribes to the CharacterAttemptEnter event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptEnter(CharacterAttemptEnterHandler handler) => Subscribe("CharacterAttemptEnter", handler);

    /// <summary>Subscribes to the CharacterAttemptLeave event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptLeave(CharacterAttemptLeaveHandler handler) => Subscribe("CharacterAttemptLeave", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the CharacterAttemptEnter event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterAttemptEnter")]
    protected virtual bool HandleCharacterAttemptEnter(Character? character, int seat) => true;

    /// <summary>Called for the CharacterAttemptLeave event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterAttemptLeave")]
    protected virtual bool HandleCharacterAttemptLeave(Character? character) => true;

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Triggered when a Character attempts to enter the Vehicle</summary>
    public delegate bool CharacterAttemptEnterHandler(VehicleWater self, Character? character, int seat);

    /// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
    public delegate bool CharacterAttemptLeaveHandler(VehicleWater self, Character? character);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(VehicleWater self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(VehicleWater self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
