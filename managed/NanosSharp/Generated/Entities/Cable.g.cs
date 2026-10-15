#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Cable represents a Physics Constraint which joins two Actors with a rope-like visual representation between them.</summary>
[ScriptClass("Cable")]
public partial class Cable : Actor, IPaintable
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

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
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

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Cable", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Cable", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Cable", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Cable", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Cable", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Cable", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Cable", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Cable", "ValueChange");
    private static readonly MethodSlot AttachEndToSlot = new("AttachEndTo");
    private static readonly MethodSlot AttachStartToSlot = new("AttachStartTo");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot DetachEndSlot = new("DetachEnd");
    private static readonly MethodSlot DetachStartSlot = new("DetachStart");
    private static readonly MethodSlot GetAttachedEndToSlot = new("GetAttachedEndTo");
    private static readonly MethodSlot GetAttachedStartToSlot = new("GetAttachedStartTo");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAngularLimitsSlot = new("SetAngularLimits");
    private static readonly MethodSlot SetAngularMotorSLERPOrientationSettingsSlot = new("SetAngularMotorSLERPOrientationSettings");
    private static readonly MethodSlot SetAngularMotorSLERPVelocitySettingsSlot = new("SetAngularMotorSLERPVelocitySettings");
    private static readonly MethodSlot SetAngularMotorTwistAndSwingOrientationSettingsSlot = new("SetAngularMotorTwistAndSwingOrientationSettings");
    private static readonly MethodSlot SetAngularMotorTwistAndSwingVelocitySettingsSlot = new("SetAngularMotorTwistAndSwingVelocitySettings");
    private static readonly MethodSlot SetCableSettingsSlot = new("SetCableSettings");
    private static readonly MethodSlot SetForcesSlot = new("SetForces");
    private static readonly MethodSlot SetLinearLimitsSlot = new("SetLinearLimits");
    private static readonly MethodSlot SetLinearMotorPositionSettingsSlot = new("SetLinearMotorPositionSettings");
    private static readonly MethodSlot SetLinearMotorVelocitySettingsSlot = new("SetLinearMotorVelocitySettings");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPhysicsConstraintSettingsSlot = new("SetPhysicsConstraintSettings");
    private static readonly MethodSlot SetRenderingSettingsSlot = new("SetRenderingSettings");

    /// <summary>Gets the Actor attached to End</summary>
    public Actor? AttachedEndTo => GetAttachedEndTo();

    /// <summary>Gets the Actor attached to Start</summary>
    public Actor? AttachedStartTo => GetAttachedStartTo();

    /// <summary>Spawns a new Cable.</summary>
    public Cable(Vector location, bool? enableVisuals = null, SpawnMode? spawnMode = null)
    {
        SpawnSelf("Cable", ScriptArguments.Trim([location, enableVisuals, spawnMode], 1));
    }

    protected Cable()
    {
    }

    /// <summary>Attaches the end of this cable to another Actor at a specific bone or relative location</summary>
    public bool AttachEndTo(
        Entity other,
        Vector? relativeLocation = null,
        string? boneName = null,
        Vector? constraintOffset = null) =>
        CallResult<bool, Entity, Vector?, string?, Vector?>(AttachEndToSlot, 1, other, relativeLocation, boneName, constraintOffset)!;

    /// <summary>Attaches the beginning of this cable to another Actor at a specific bone or relative location</summary>
    public bool AttachStartTo(Entity other, Vector? relativeLocation = null, string? boneName = null) =>
        CallResult<bool, Entity, Vector?, string?>(AttachStartToSlot, 1, other, relativeLocation, boneName)!;

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Detaches the End of this Cable</summary>
    public void DetachEnd() => Call(DetachEndSlot, 0);

    /// <summary>Detaches the Start of this Cable</summary>
    public void DetachStart() => Call(DetachStartSlot, 0);

    /// <summary>Gets the Actor attached to End</summary>
    public Actor? GetAttachedEndTo() => CallResult<Actor?>(GetAttachedEndToSlot, 0);

    /// <summary>Gets the Actor attached to Start</summary>
    public Actor? GetAttachedStartTo() => CallResult<Actor?>(GetAttachedStartToSlot, 0);

    /// <summary>Gets a Color parameter from this Actor's material</summary>
    public Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<Color?, string, int?, string?>(GetMaterialColorParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    public float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<float?, string, int?, string?>(GetMaterialScalarParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    public string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<string?, string, int?, string?>(GetMaterialTextureParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    public Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<Vector?, string, int?, string?>(GetMaterialVectorParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Call(ResetMaterialSlot, 0, index, attachableId);

    /// <summary>Sets the Physics Angular Limits of this cable</summary>
    public void SetAngularLimits(
        ConstraintMotion swing1Motion,
        ConstraintMotion swing2Motion,
        ConstraintMotion twistMotion,
        float? swing1Limit = null,
        float? swing2Limit = null,
        float? twistLimit = null) =>
        Call(SetAngularLimitsSlot, 3, swing1Motion, swing2Motion, twistMotion, swing1Limit, swing2Limit, twistLimit);

    /// <summary>Sets whether the SLERP angular orientation drive is enabled and the target orientation</summary>
    public void SetAngularMotorSLERPOrientationSettings(
        bool? enableDriveSlerp = null,
        float? strength = null,
        Rotator? target = null) =>
        Call(SetAngularMotorSLERPOrientationSettingsSlot, 0, enableDriveSlerp, strength, target);

    /// <summary>Sets whether the SLERP angular velocity drive is enabled and the target angular velocity</summary>
    public void SetAngularMotorSLERPVelocitySettings(
        bool? enableDriveSlerp = null,
        float? strength = null,
        Vector? target = null) =>
        Call(SetAngularMotorSLERPVelocitySettingsSlot, 0, enableDriveSlerp, strength, target);

    /// <summary>Sets which twist and swing angular orientation drives are enabled and the target orientation</summary>
    public void SetAngularMotorTwistAndSwingOrientationSettings(
        bool? enableDriveTwist = null,
        bool? enableDriveSwing = null,
        float? strength = null,
        Rotator? target = null) =>
        Call(SetAngularMotorTwistAndSwingOrientationSettingsSlot, 0, enableDriveTwist, enableDriveSwing, strength, target);

    /// <summary>Sets which twist and swing angular velocity drives are enabled and the target angular velocity</summary>
    public void SetAngularMotorTwistAndSwingVelocitySettings(
        bool? enableDriveTwist = null,
        bool? enableDriveSwing = null,
        float? strength = null,
        Vector? target = null) =>
        Call(SetAngularMotorTwistAndSwingVelocitySettingsSlot, 0, enableDriveTwist, enableDriveSwing, strength, target);

    /// <summary>Set the overall settings for this cable (visuals only)</summary>
    public void SetCableSettings(
        float length,
        uint? numSegments = null,
        uint? solverIterations = null,
        bool? enableStiffness = null) =>
        Call(SetCableSettingsSlot, 1, length, numSegments, solverIterations, enableStiffness);

    /// <summary>Set the forces the cable has applied (visuals only)</summary>
    public void SetForces(Vector force, float? gravityScale = null) => Call(SetForcesSlot, 1, force, gravityScale);

    /// <summary>Sets the Physics Linear Limits of this cable</summary>
    public void SetLinearLimits(
        ConstraintMotion xMotion,
        ConstraintMotion yMotion,
        ConstraintMotion zMotion,
        float? limit = null,
        float? restitution = null,
        bool? useSoftConstraint = null,
        float? stiffness = null,
        float? damping = null) =>
        Call(SetLinearLimitsSlot, 3, xMotion, yMotion, zMotion, limit, restitution, useSoftConstraint, stiffness, damping);

    /// <summary>Sets which linear position drives are enabled and the target position</summary>
    public void SetLinearMotorPositionSettings(
        bool? enableDriveX = null,
        bool? enableDriveY = null,
        bool? enableDriveZ = null,
        float? strength = null,
        Vector? target = null) =>
        Call(SetLinearMotorPositionSettingsSlot, 0, enableDriveX, enableDriveY, enableDriveZ, strength, target);

    /// <summary>Sets which linear velocity drives are enabled and the target velocity</summary>
    public void SetLinearMotorVelocitySettings(
        bool? enableDriveX = null,
        bool? enableDriveY = null,
        bool? enableDriveZ = null,
        float? strength = null,
        Vector? target = null) =>
        Call(SetLinearMotorVelocitySettingsSlot, 0, enableDriveX, enableDriveY, enableDriveZ, strength, target);

    /// <summary>Sets the material at the specified index of this Actor</summary>
    public void SetMaterial(string materialPath, int? index = null, string? attachableId = null) =>
        Call(SetMaterialSlot, 1, materialPath, index, attachableId);

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    public void SetMaterialColorParameter(
        string parameterName,
        Color color,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialColorParameterSlot, 2, parameterName, color, index, attachableId);

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    public void SetMaterialScalarParameter(
        string parameterName,
        float scalar,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialScalarParameterSlot, 2, parameterName, scalar, index, attachableId);

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    public void SetMaterialTextureParameter(
        string parameterName,
        string texturePath,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialTextureParameterSlot, 2, parameterName, texturePath, index, attachableId);

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    public void SetMaterialVectorParameter(
        string parameterName,
        Vector vector,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialVectorParameterSlot, 2, parameterName, vector, index, attachableId);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Call(SetPhysicalMaterialSlot, 1, physicalMaterialPath);

    /// <summary>Set the Physics Constraint related settings for this cable</summary>
    public void SetPhysicsConstraintSettings(bool disableCollision) =>
        Call(SetPhysicsConstraintSettingsSlot, 1, disableCollision);

    /// <summary>Set the rendering settings of this cable (visuals only)</summary>
    public void SetRenderingSettings(float width, uint numSides, float tileMaterial) =>
        Call(SetRenderingSettingsSlot, 3, width, numSides, tileMaterial);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

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

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Cable self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Cable self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Cable self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Cable self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Cable self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Cable self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Cable self, string? key, object? value);
}
