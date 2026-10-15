#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Trigger class is a utility class to trigger events when any Entity enters an Area.</summary>
[ScriptClass("Trigger")]
public partial class Trigger : Actor
{
    /// <summary>Triggered when something overlaps this Trigger</summary>
    public static event BeginOverlapHandler? OnBeginOverlap
    {
        add => BeginOverlapEvent.Add(value);
        remove => BeginOverlapEvent.Remove(value);
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is destroyed</summary>
    public static event DestroyHandler? OnDestroy
    {
        add => DestroyEvent.Add(value);
        remove => DestroyEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when something leaves this Trigger</summary>
    public static event EndOverlapHandler? OnEndOverlap
    {
        add => EndOverlapEvent.Add(value);
        remove => EndOverlapEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<BeginOverlapHandler> BeginOverlapEvent = new("Trigger", "BeginOverlap");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Trigger", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Trigger", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Trigger", "DimensionChange");
    private static readonly ClassEvent<EndOverlapHandler> EndOverlapEvent = new("Trigger", "EndOverlap");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Trigger", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Trigger", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Trigger", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Trigger", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Trigger", "ValueChange");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot ForceOverlapCheckingSlot = new("ForceOverlapChecking");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot SetColorSlot = new("SetColor");
    private static readonly MethodSlot SetExtentSlot = new("SetExtent");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetOverlapOnlyClassesSlot = new("SetOverlapOnlyClasses");
    private static readonly MethodSlot SetRadiusSlot = new("SetRadius");

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

    /// <summary>Spawns a new Trigger.</summary>
    public Trigger(
        Vector location,
        Rotator rotation,
        Vector extent,
        TriggerType? triggerType = null,
        bool? isVisible = null,
        Color? color = null,
        IEnumerable<string>? overlapOnlyClasses = null)
    {
        SpawnSelf("Trigger", ScriptArguments.Trim([location, rotation, extent, triggerType, isVisible, color, overlapOnlyClasses], 3));
    }

    protected Trigger()
    {
    }

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Forces an Overlap check to occur, will immediately trigger overlaps</summary>
    public void ForceOverlapChecking() => Call(ForceOverlapCheckingSlot, 0);

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector GetInitialLocation() => CallResult<Vector>(GetInitialLocationSlot, 0)!;

    /// <summary>Sets the Trigger color (if visible)</summary>
    public void SetColor(Color color) => Call(SetColorSlot, 1, color);

    /// <summary>Sets the extent size of this trigger (sphere triggers can receive as float for radius)</summary>
    public void SetExtent(Vector extent) => Call(SetExtentSlot, 1, extent);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

    /// <summary>Sets the filter to Trigger to only overlap specific Classes. Leave it empty for all Classes</summary>
    public void SetOverlapOnlyClasses(IEnumerable<string> overlapOnlyClasses) =>
        Call(SetOverlapOnlyClassesSlot, 1, overlapOnlyClasses);

    /// <summary>Sets the radius of a sphere Trigger</summary>
    public void SetRadius(float radius) => Call(SetRadiusSlot, 1, radius);

    /// <summary>Subscribes to the BeginOverlap event of this entity only.</summary>
    public ScriptSubscription SubscribeBeginOverlap(BeginOverlapHandler handler) => Subscribe("BeginOverlap", handler);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EndOverlap event of this entity only.</summary>
    public ScriptSubscription SubscribeEndOverlap(EndOverlapHandler handler) => Subscribe("EndOverlap", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the BeginOverlap event of this entity when a subclass overrides it.</summary>
    [EventHook("BeginOverlap")]
    protected virtual void HandleBeginOverlap(Actor? entity)
    {
    }

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the EndOverlap event of this entity when a subclass overrides it.</summary>
    [EventHook("EndOverlap")]
    protected virtual void HandleEndOverlap(Actor? entity)
    {
    }

    /// <summary>Called for the EnterWater event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterWater")]
    protected virtual void HandleEnterWater()
    {
    }

    /// <summary>Called for the LeaveWater event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveWater")]
    protected virtual void HandleLeaveWater()
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when something overlaps this Trigger</summary>
    public delegate void BeginOverlapHandler(Trigger self, Actor? entity);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Trigger self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Trigger self, int oldDimension, int newDimension);

    /// <summary>Triggered when something leaves this Trigger</summary>
    public delegate void EndOverlapHandler(Trigger self, Actor? entity);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Trigger self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Trigger self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Trigger self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Trigger self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Trigger self, string? key, object? value);
}
