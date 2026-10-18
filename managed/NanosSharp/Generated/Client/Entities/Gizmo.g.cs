#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A tool to transform objects at runtime.</summary>
[ScriptClass("Gizmo")]
public partial class Gizmo : Actor
{
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

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
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

    /// <summary>Triggered when the Gizmo has its transform updated</summary>
    public static event TransformHandler? OnTransform
    {
        add => TransformEvent.Add(value);
        remove => TransformEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Gizmo", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Gizmo", "Destroy");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Gizmo", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Gizmo", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Gizmo", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Gizmo", "Spawn");
    private static readonly ClassEvent<TransformHandler> TransformEvent = new("Gizmo", "Transform");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Gizmo", "ValueChange");
    private static readonly MethodSlot ActivateSlot = new("Activate");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot DeactivateSlot = new("Deactivate");
    private static readonly MethodSlot IsInteractingSlot = new("IsInteracting");
    private static readonly MethodSlot PressPointerSlot = new("PressPointer");
    private static readonly MethodSlot ReleasePointerSlot = new("ReleasePointer");
    private static readonly MethodSlot SetAlignSpaceSlot = new("SetAlignSpace");
    private static readonly MethodSlot SetSnapSettingsSlot = new("SetSnapSettings");
    private static readonly MethodSlot SetTransformModeSlot = new("SetTransformMode");

    /// <summary>Returns if it's currently interacting</summary>
    public bool Interacting => IsInteracting();

    /// <summary>Spawns a new Gizmo.</summary>
    public Gizmo()
    {
        SpawnSelf("Gizmo", ReadOnlySpan<object?>.Empty);
    }

    /// <summary>Enables the interaction, placing the Gizmo at the given transform</summary>
    public void Activate(Vector? location = null, Rotator? rotation = null, Vector? scale = null) =>
        Call(ActivateSlot, 0, location, rotation, scale);

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Disable the interaction</summary>
    public void Deactivate() => Call(DeactivateSlot, 0);

    /// <summary>Returns if it's currently interacting</summary>
    public bool IsInteracting() => CallResult<bool>(IsInteractingSlot, 0)!;

    /// <summary>Triggers the press pointer event on it</summary>
    public void PressPointer() => Call(PressPointerSlot, 0);

    /// <summary>Triggers the release pointer event on it</summary>
    public void ReleasePointer() => Call(ReleasePointerSlot, 0);

    /// <summary>Sets whether the Gizmo is aligned to the local or the world space</summary>
    public void SetAlignSpace(GizmoAlignSpace alignSpace) => Call(SetAlignSpaceSlot, 1, alignSpace);

    /// <summary>Sets the snapping grid sizes, 0 to disable snapping</summary>
    public void SetSnapSettings(float locationGridSize, float rotationGridSize, float scaleGridSize) =>
        Call(SetSnapSettingsSlot, 3, locationGridSize, rotationGridSize, scaleGridSize);

    /// <summary>Sets whether the Gizmo translates, rotates or scales</summary>
    public void SetTransformMode(GizmoTransformMode transformMode, bool? combineTranslateRotate = null) =>
        Call(SetTransformModeSlot, 1, transformMode, combineTranslateRotate);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the Transform event of this entity only.</summary>
    public ScriptSubscription SubscribeTransform(TransformHandler handler) => Subscribe("Transform", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
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
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
    {
    }

    /// <summary>Called for the Transform event of this entity when a subclass overrides it.</summary>
    [EventHook("Transform")]
    protected virtual void HandleTransform(Vector location, Rotator rotation, Vector scale)
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Gizmo self);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Gizmo self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Gizmo self);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Gizmo self, bool isNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Gizmo self);

    /// <summary>Triggered when the Gizmo has its transform updated</summary>
    public delegate void TransformHandler(Gizmo self, Vector location, Rotator rotation, Vector scale);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Gizmo self, string? key, object? value);
}
