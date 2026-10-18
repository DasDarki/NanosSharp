#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class CharacterGASP
{
    /// <summary>Triggered when a Character attempts to enter a vehicle</summary>
    public static event AttemptEnterVehicleHandler? OnAttemptEnterVehicle
    {
        add => AttemptEnterVehicleEvent.Add(value);
        remove => AttemptEnterVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when this Character attempts to leave a vehicle</summary>
    public static event AttemptLeaveVehicleHandler? OnAttemptLeaveVehicle
    {
        add => AttemptLeaveVehicleEvent.Add(value);
        remove => AttemptLeaveVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
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

    private static readonly ClassEvent<AttemptEnterVehicleHandler> AttemptEnterVehicleEvent = new("CharacterGASP", "AttemptEnterVehicle");
    private static readonly ClassEvent<AttemptLeaveVehicleHandler> AttemptLeaveVehicleEvent = new("CharacterGASP", "AttemptLeaveVehicle");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("CharacterGASP", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("CharacterGASP", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("CharacterGASP", "NetworkAuthorityChange");
    private static readonly MethodSlot EnterVehicleSlot = new("EnterVehicle");
    private static readonly MethodSlot GrabPropSlot = new("GrabProp");
    private static readonly MethodSlot LeaveVehicleSlot = new("LeaveVehicle");
    private static readonly MethodSlot UnGrabPropSlot = new("UnGrabProp");

    /// <summary>Gets the Team of this Character</summary>
    public uint Team => GetTeam();

    /// <summary>Enters the Vehicle at Seat (0 - Driver)</summary>
    public void EnterVehicle(Entity vehicle, byte? seat = null) => Call(EnterVehicleSlot, 1, vehicle, seat);

    /// <summary>Gives a Prop to the Character</summary>
    public void GrabProp(Prop prop) => Call(GrabPropSlot, 1, prop);

    /// <summary>Leaves the current Vehicle</summary>
    public void LeaveVehicle() => Call(LeaveVehicleSlot, 0);

    /// <summary>UnGrabs/Drops the Prop the Character is holding</summary>
    public void UnGrabProp() => Call(UnGrabPropSlot, 0);

    /// <summary>Subscribes to the AttemptEnterVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptEnterVehicle(AttemptEnterVehicleHandler handler) => Subscribe("AttemptEnterVehicle", handler);

    /// <summary>Subscribes to the AttemptLeaveVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptLeaveVehicle(AttemptLeaveVehicleHandler handler) => Subscribe("AttemptLeaveVehicle", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the AttemptEnterVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptEnterVehicle")]
    protected virtual bool HandleAttemptEnterVehicle(Entity? vehicle, int seatIndex) => true;

    /// <summary>Called for the AttemptLeaveVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptLeaveVehicle")]
    protected virtual bool HandleAttemptLeaveVehicle(Entity? vehicle) => true;

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Interact event of this entity when a subclass overrides it.</summary>
    [EventHook("Interact")]
    protected virtual bool HandleInteract(Prop? @object) => true;

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Triggered when a Character attempts to enter a vehicle</summary>
    public delegate bool AttemptEnterVehicleHandler(CharacterGASP self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when this Character attempts to leave a vehicle</summary>
    public delegate bool AttemptLeaveVehicleHandler(CharacterGASP self, Entity? vehicle);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(CharacterGASP self, int oldDimension, int newDimension);

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
    public delegate bool InteractHandler(CharacterGASP self, Prop? @object);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(CharacterGASP self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
