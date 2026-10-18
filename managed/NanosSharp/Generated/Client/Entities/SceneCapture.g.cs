#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Scene Capture is an Actor which captures a fully dynamic image of the scene into a Texture. It captures the scene from its view frustum, stores that view as an image, which is then used within a Material.</summary>
[ScriptClass("SceneCapture")]
public partial class SceneCapture : Actor
{
    /// <summary>Triggered when this SceneCapture does an update/renders a frame</summary>
    public static event CaptureHandler? OnCapture
    {
        add => CaptureEvent.Add(value);
        remove => CaptureEvent.Remove(value);
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

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<CaptureHandler> CaptureEvent = new("SceneCapture", "Capture");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("SceneCapture", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("SceneCapture", "Destroy");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("SceneCapture", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("SceneCapture", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("SceneCapture", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("SceneCapture", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("SceneCapture", "ValueChange");
    private static readonly MethodSlot AddRenderActorSlot = new("AddRenderActor");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot CaptureSceneSlot = new("CaptureScene");
    private static readonly MethodSlot ClearRenderActorsSlot = new("ClearRenderActors");
    private static readonly MethodSlot EncodeToBase64Slot = new("EncodeToBase64");
    private static readonly MethodSlot EncodeToBase64AsyncSlot = new("EncodeToBase64Async");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot RemoveRenderActorSlot = new("RemoveRenderActor");
    private static readonly MethodSlot ResizeSlot = new("Resize");
    private static readonly MethodSlot SetDistanceOptimizationEnabledSlot = new("SetDistanceOptimizationEnabled");
    private static readonly MethodSlot SetFOVAngleSlot = new("SetFOVAngle");
    private static readonly MethodSlot SetFreezeSlot = new("SetFreeze");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetRenderRateSlot = new("SetRenderRate");
    private static readonly MethodSlot SetShowFlagSlot = new("SetShowFlag");

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

    /// <summary>Spawns a new SceneCapture.</summary>
    public SceneCapture(
        Vector? location = null,
        Rotator? rotation = null,
        int? width = null,
        int? height = null,
        float? renderRate = null,
        float? viewDistance = null,
        float? fovAngle = null,
        bool? enableDistanceOptimization = null)
    {
        SpawnSelf("SceneCapture", ScriptArguments.Trim([location, rotation, width, height, renderRate, viewDistance, fovAngle, enableDistanceOptimization], 0));
    }

    protected SceneCapture()
    {
    }

    /// <summary>Adds an Actor to the Render Only list</summary>
    public void AddRenderActor(Entity actor) => Call(AddRenderActorSlot, 1, actor);

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Captures the scene. If deferred is true, performs deferred capture and records the frame; otherwise captures immediately and triggers Capture event</summary>
    public void CaptureScene(bool? deferred = null) => Call(CaptureSceneSlot, 0, deferred);

    /// <summary>Clears the Render Only list</summary>
    public void ClearRenderActors() => Call(ClearRenderActorsSlot, 0);

    /// <summary>Takes a snapshot of the SceneCapture and returns a Base64 of it</summary>
    public string EncodeToBase64(ImageFormat? imageFormat = null) =>
        CallResult<string, ImageFormat?>(EncodeToBase64Slot, 0, imageFormat)!;

    /// <summary>Takes a snapshot of the SceneCapture and returns a Base64 of it (asynchronously)</summary>
    public void EncodeToBase64Async(ImageFormat? imageFormat, Delegate callback) =>
        Call(EncodeToBase64AsyncSlot, 2, imageFormat, ScriptCallback.From(callback));

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector GetInitialLocation() => CallResult<Vector>(GetInitialLocationSlot, 0)!;

    /// <summary>Removes an Actor from the Render Only list</summary>
    public void RemoveRenderActor(Entity actor) => Call(RemoveRenderActorSlot, 1, actor);

    /// <summary>Change the output Texture size Note: too high texture will make the capture slower and will affect game performance</summary>
    public void Resize(uint width, uint height) => Call(ResizeSlot, 2, width, height);

    /// <summary>Enables or not the rendering frequency optimization if the entities with this Material are too far</summary>
    public void SetDistanceOptimizationEnabled(bool enabled) => Call(SetDistanceOptimizationEnabledSlot, 1, enabled);

    /// <summary>Sets the FOV</summary>
    public void SetFOVAngle(float angle) => Call(SetFOVAngleSlot, 1, angle);

    /// <summary>Stops or resumes capturing</summary>
    public void SetFreeze(bool freeze) => Call(SetFreezeSlot, 1, freeze);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

    /// <summary>Set how frequent is the capture Note: Set to 0 to capture every frame, or -1 to disable auto-capturing</summary>
    public void SetRenderRate(float renderRate) => Call(SetRenderRateSlot, 1, renderRate);

    /// <summary>Enables/Disables rendering features from being captured</summary>
    public void SetShowFlag(string flag, bool enable) => Call(SetShowFlagSlot, 2, flag, enable);

    /// <summary>Subscribes to the Capture event of this entity only.</summary>
    public ScriptSubscription SubscribeCapture(CaptureHandler handler) => Subscribe("Capture", handler);

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

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Capture event of this entity when a subclass overrides it.</summary>
    [EventHook("Capture")]
    protected virtual void HandleCapture()
    {
    }

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

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when this SceneCapture does an update/renders a frame</summary>
    public delegate void CaptureHandler(SceneCapture self);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(SceneCapture self);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(SceneCapture self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(SceneCapture self);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(SceneCapture self, bool isNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(SceneCapture self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(SceneCapture self, string? key, object? value);
}
