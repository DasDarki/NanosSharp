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
        Invoke<bool>(AttachEndToSlot, ScriptArguments.Trim([other, relativeLocation, boneName, constraintOffset], 1))!;

    /// <summary>Attaches the beginning of this cable to another Actor at a specific bone or relative location</summary>
    public bool AttachStartTo(Entity other, Vector? relativeLocation = null, string? boneName = null) =>
        Invoke<bool>(AttachStartToSlot, ScriptArguments.Trim([other, relativeLocation, boneName], 1))!;

    /// <summary>Detaches the End of this Cable</summary>
    public void DetachEnd() => Invoke(DetachEndSlot);

    /// <summary>Detaches the Start of this Cable</summary>
    public void DetachStart() => Invoke(DetachStartSlot);

    /// <summary>Gets the Actor attached to End</summary>
    public Actor? GetAttachedEndTo() => Invoke<Actor?>(GetAttachedEndToSlot);

    /// <summary>Gets the Actor attached to Start</summary>
    public Actor? GetAttachedStartTo() => Invoke<Actor?>(GetAttachedStartToSlot);

    /// <summary>Gets a Color parameter from this Actor's material</summary>
    public Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Color?>(GetMaterialColorParameterSlot, ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    public float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<float?>(GetMaterialScalarParameterSlot, ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    public string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<string?>(GetMaterialTextureParameterSlot, ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    public Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Vector?>(GetMaterialVectorParameterSlot, ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke(ResetMaterialSlot, ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Sets the Physics Angular Limits of this cable</summary>
    public void SetAngularLimits(
        ConstraintMotion swing1Motion,
        ConstraintMotion swing2Motion,
        ConstraintMotion twistMotion,
        float? swing1Limit = null,
        float? swing2Limit = null,
        float? twistLimit = null) =>
        Invoke(SetAngularLimitsSlot, ScriptArguments.Trim([swing1Motion, swing2Motion, twistMotion, swing1Limit, swing2Limit, twistLimit], 3));

    /// <summary>Sets whether the SLERP angular orientation drive is enabled and the target orientation</summary>
    public void SetAngularMotorSLERPOrientationSettings(
        bool? enableDriveSlerp = null,
        float? strength = null,
        Rotator? target = null) =>
        Invoke(SetAngularMotorSLERPOrientationSettingsSlot, ScriptArguments.Trim([enableDriveSlerp, strength, target], 0));

    /// <summary>Sets whether the SLERP angular velocity drive is enabled and the target angular velocity</summary>
    public void SetAngularMotorSLERPVelocitySettings(
        bool? enableDriveSlerp = null,
        float? strength = null,
        Vector? target = null) =>
        Invoke(SetAngularMotorSLERPVelocitySettingsSlot, ScriptArguments.Trim([enableDriveSlerp, strength, target], 0));

    /// <summary>Sets which twist and swing angular orientation drives are enabled and the target orientation</summary>
    public void SetAngularMotorTwistAndSwingOrientationSettings(
        bool? enableDriveTwist = null,
        bool? enableDriveSwing = null,
        float? strength = null,
        Rotator? target = null) =>
        Invoke(SetAngularMotorTwistAndSwingOrientationSettingsSlot, ScriptArguments.Trim([enableDriveTwist, enableDriveSwing, strength, target], 0));

    /// <summary>Sets which twist and swing angular velocity drives are enabled and the target angular velocity</summary>
    public void SetAngularMotorTwistAndSwingVelocitySettings(
        bool? enableDriveTwist = null,
        bool? enableDriveSwing = null,
        float? strength = null,
        Vector? target = null) =>
        Invoke(SetAngularMotorTwistAndSwingVelocitySettingsSlot, ScriptArguments.Trim([enableDriveTwist, enableDriveSwing, strength, target], 0));

    /// <summary>Set the overall settings for this cable (visuals only)</summary>
    public void SetCableSettings(
        float length,
        uint? numSegments = null,
        uint? solverIterations = null,
        bool? enableStiffness = null) =>
        Invoke(SetCableSettingsSlot, ScriptArguments.Trim([length, numSegments, solverIterations, enableStiffness], 1));

    /// <summary>Set the forces the cable has applied (visuals only)</summary>
    public void SetForces(Vector force, float? gravityScale = null) =>
        Invoke(SetForcesSlot, ScriptArguments.Trim([force, gravityScale], 1));

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
        Invoke(SetLinearLimitsSlot, ScriptArguments.Trim([xMotion, yMotion, zMotion, limit, restitution, useSoftConstraint, stiffness, damping], 3));

    /// <summary>Sets which linear position drives are enabled and the target position</summary>
    public void SetLinearMotorPositionSettings(
        bool? enableDriveX = null,
        bool? enableDriveY = null,
        bool? enableDriveZ = null,
        float? strength = null,
        Vector? target = null) =>
        Invoke(SetLinearMotorPositionSettingsSlot, ScriptArguments.Trim([enableDriveX, enableDriveY, enableDriveZ, strength, target], 0));

    /// <summary>Sets which linear velocity drives are enabled and the target velocity</summary>
    public void SetLinearMotorVelocitySettings(
        bool? enableDriveX = null,
        bool? enableDriveY = null,
        bool? enableDriveZ = null,
        float? strength = null,
        Vector? target = null) =>
        Invoke(SetLinearMotorVelocitySettingsSlot, ScriptArguments.Trim([enableDriveX, enableDriveY, enableDriveZ, strength, target], 0));

    /// <summary>Sets the material at the specified index of this Actor</summary>
    public void SetMaterial(string materialPath, int? index = null, string? attachableId = null) =>
        Invoke(SetMaterialSlot, ScriptArguments.Trim([materialPath, index, attachableId], 1));

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    public void SetMaterialColorParameter(
        string parameterName,
        Color color,
        int? index = null,
        string? attachableId = null) =>
        Invoke(SetMaterialColorParameterSlot, ScriptArguments.Trim([parameterName, color, index, attachableId], 2));

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    public void SetMaterialScalarParameter(
        string parameterName,
        float scalar,
        int? index = null,
        string? attachableId = null) =>
        Invoke(SetMaterialScalarParameterSlot, ScriptArguments.Trim([parameterName, scalar, index, attachableId], 2));

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    public void SetMaterialTextureParameter(
        string parameterName,
        string texturePath,
        int? index = null,
        string? attachableId = null) =>
        Invoke(SetMaterialTextureParameterSlot, ScriptArguments.Trim([parameterName, texturePath, index, attachableId], 2));

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    public void SetMaterialVectorParameter(
        string parameterName,
        Vector vector,
        int? index = null,
        string? attachableId = null) =>
        Invoke(SetMaterialVectorParameterSlot, ScriptArguments.Trim([parameterName, vector, index, attachableId], 2));

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Invoke(SetPhysicalMaterialSlot, physicalMaterialPath);

    /// <summary>Set the Physics Constraint related settings for this cable</summary>
    public void SetPhysicsConstraintSettings(bool disableCollision) =>
        Invoke(SetPhysicsConstraintSettingsSlot, disableCollision);

    /// <summary>Set the rendering settings of this cable (visuals only)</summary>
    public void SetRenderingSettings(float width, uint numSides, float tileMaterial) =>
        Invoke(SetRenderingSettingsSlot, width, numSides, tileMaterial);

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
