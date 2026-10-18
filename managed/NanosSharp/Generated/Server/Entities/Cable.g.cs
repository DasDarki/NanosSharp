#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Cable
{
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

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Cable", "DimensionChange");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Cable", "NetworkAuthorityChange");
    private static readonly MethodSlot AttachEndToSlot = new("AttachEndTo");
    private static readonly MethodSlot AttachStartToSlot = new("AttachStartTo");
    private static readonly MethodSlot DetachEndSlot = new("DetachEnd");
    private static readonly MethodSlot DetachStartSlot = new("DetachStart");
    private static readonly MethodSlot SetAngularLimitsSlot = new("SetAngularLimits");
    private static readonly MethodSlot SetAngularMotorSLERPOrientationSettingsSlot = new("SetAngularMotorSLERPOrientationSettings");
    private static readonly MethodSlot SetAngularMotorSLERPVelocitySettingsSlot = new("SetAngularMotorSLERPVelocitySettings");
    private static readonly MethodSlot SetAngularMotorTwistAndSwingOrientationSettingsSlot = new("SetAngularMotorTwistAndSwingOrientationSettings");
    private static readonly MethodSlot SetAngularMotorTwistAndSwingVelocitySettingsSlot = new("SetAngularMotorTwistAndSwingVelocitySettings");
    private static readonly MethodSlot SetLinearLimitsSlot = new("SetLinearLimits");
    private static readonly MethodSlot SetLinearMotorPositionSettingsSlot = new("SetLinearMotorPositionSettings");
    private static readonly MethodSlot SetLinearMotorVelocitySettingsSlot = new("SetLinearMotorVelocitySettings");
    private static readonly MethodSlot SetPhysicsConstraintSettingsSlot = new("SetPhysicsConstraintSettings");

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

    /// <summary>Detaches the End of this Cable</summary>
    public void DetachEnd() => Call(DetachEndSlot, 0);

    /// <summary>Detaches the Start of this Cable</summary>
    public void DetachStart() => Call(DetachStartSlot, 0);

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

    /// <summary>Set the Physics Constraint related settings for this cable</summary>
    public void SetPhysicsConstraintSettings(bool disableCollision) =>
        Call(SetPhysicsConstraintSettingsSlot, 1, disableCollision);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

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

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Cable self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Cable self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
