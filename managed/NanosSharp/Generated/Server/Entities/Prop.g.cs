#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Prop
{
    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>When a Character interacts with this Prop (i.e. try to Grab it)</summary>
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

    /// <summary>When Prop takes Damage</summary>
    public static event TakeDamageHandler? OnTakeDamage
    {
        add => TakeDamageEvent.Add(value);
        remove => TakeDamageEvent.Remove(value);
    }

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Prop", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Prop", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Prop", "NetworkAuthorityChange");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("Prop", "TakeDamage");

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

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

    /// <summary>Called for the TakeDamage event of this entity when a subclass overrides it.</summary>
    [EventHook("TakeDamage")]
    protected virtual void HandleTakeDamage(int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, Actor? causer)
    {
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Prop self, int oldDimension, int newDimension);

    /// <summary>When a Character interacts with this Prop (i.e. try to Grab it)</summary>
    public delegate bool InteractHandler(Prop self, Character? character);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Prop self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>When Prop takes Damage</summary>
    public delegate void TakeDamageHandler(Prop self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, Actor? causer);
}
