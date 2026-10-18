#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Grenade
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

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Grenade", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Grenade", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Grenade", "NetworkAuthorityChange");
    private static readonly MethodSlot ExplodeSlot = new("Explode");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot SetDamageSlot = new("SetDamage");
    private static readonly MethodSlot SetPickableSlot = new("SetPickable");
    private static readonly MethodSlot SetThrowForceSlot = new("SetThrowForce");
    private static readonly MethodSlot SetTimeToExplodeSlot = new("SetTimeToExplode");

    /// <summary>Gets the impulse applied when throwing</summary>
    public ushort ThrowForce
    {
        get => GetThrowForce();
        set => SetThrowForce(value);
    }

    /// <summary>Gets the time until the explosion after throwing</summary>
    public float TimeToExplode
    {
        get => GetTimeToExplode();
        set => SetTimeToExplode(value);
    }

    /// <summary>Forces this grenade to Explode</summary>
    public void Explode() => Call(ExplodeSlot, 0);

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) => Call(PullUseSlot, 0, releaseUseAfter);

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Call(ReleaseUseSlot, 0);

    /// <summary>Sets Damage parameters of this Grenade</summary>
    public void SetDamage(
        uint? baseDamage = null,
        uint? minimumDamage = null,
        ushort? damageInnerRadius = null,
        ushort? damageOuterRadius = null,
        float? damageFalloff = null) =>
        Call(SetDamageSlot, 0, baseDamage, minimumDamage, damageInnerRadius, damageOuterRadius, damageFalloff);

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    public void SetPickable(bool isPickable) => Call(SetPickableSlot, 1, isPickable);

    /// <summary>Set Impulse applied when throwing</summary>
    public void SetThrowForce(ushort throwForce) => Call(SetThrowForceSlot, 1, throwForce);

    /// <summary>Set Time until Explosion after thrown</summary>
    public void SetTimeToExplode(float time) => Call(SetTimeToExplodeSlot, 1, time);

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
    public delegate void DimensionChangeHandler(Grenade self, int oldDimension, int newDimension);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Grenade self, Character? character);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Grenade self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
