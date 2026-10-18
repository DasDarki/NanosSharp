#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Melee
{
    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public static event InteractHandler? OnInteract
    {
        add => InteractEvent.Add(value);
        remove => InteractEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Melee", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Melee", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Melee", "NetworkAuthorityChange");
    private static readonly MethodSlot AddAnimationCharacterUseSlot = new("AddAnimationCharacterUse");
    private static readonly MethodSlot ClearAnimationsCharacterUseSlot = new("ClearAnimationsCharacterUse");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot SetBaseDamageSlot = new("SetBaseDamage");
    private static readonly MethodSlot SetDamageSettingsSlot = new("SetDamageSettings");
    private static readonly MethodSlot SetPickableSlot = new("SetPickable");

    /// <summary>Gets the Base Damage</summary>
    public uint BaseDamage
    {
        get => GetBaseDamage();
        set => SetBaseDamage(value);
    }

    /// <summary>Sets an Animation when attacking</summary>
    public void AddAnimationCharacterUse(
        string assetPath,
        float? playRate = null,
        AnimationSlotType? slotType = null) =>
        Call(AddAnimationCharacterUseSlot, 1, assetPath, playRate, slotType);

    /// <summary>Clears the Character Attack Animation list</summary>
    public void ClearAnimationsCharacterUse() => Call(ClearAnimationsCharacterUseSlot, 0);

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) => Call(PullUseSlot, 0, releaseUseAfter);

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Call(ReleaseUseSlot, 0);

    /// <summary>Sets the Base Damage</summary>
    public void SetBaseDamage(uint damage) => Call(SetBaseDamageSlot, 1, damage);

    /// <summary>Sets the times when to start applying damage and when to end</summary>
    public void SetDamageSettings(float damageStartTime, float damageDurationTime) =>
        Call(SetDamageSettingsSlot, 2, damageStartTime, damageDurationTime);

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    public void SetPickable(bool isPickable) => Call(SetPickableSlot, 1, isPickable);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Interact event of this entity when a subclass overrides it.</summary>
    [EventHook("Interact")]
    protected virtual bool HandleInteract(Character? character) => true;

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Melee self, int oldDimension, int newDimension);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Melee self, Character? character);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Melee self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
