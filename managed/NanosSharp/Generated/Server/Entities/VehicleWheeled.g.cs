#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class VehicleWheeled : IDamageable
{
    /// <summary>Triggered when a Character attempts to enter the Vehicle</summary>
    public static event CharacterAttemptEnterHandler? OnCharacterAttemptEnter
    {
        add => CharacterAttemptEnterEvent.Add(value);
        remove => CharacterAttemptEnterEvent.Remove(value);
    }

    /// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
    public static event CharacterAttemptLeaveHandler? OnCharacterAttemptLeave
    {
        add => CharacterAttemptLeaveEvent.Add(value);
        remove => CharacterAttemptLeaveEvent.Remove(value);
    }

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

    private static readonly ClassEvent<CharacterAttemptEnterHandler> CharacterAttemptEnterEvent = new("VehicleWheeled", "CharacterAttemptEnter");
    private static readonly ClassEvent<CharacterAttemptLeaveHandler> CharacterAttemptLeaveEvent = new("VehicleWheeled", "CharacterAttemptLeave");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("VehicleWheeled", "DimensionChange");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("VehicleWheeled", "NetworkAuthorityChange");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot HornSlot = new("Horn");
    private static readonly MethodSlot RecreatePhysicsSlot = new("RecreatePhysics");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
    private static readonly MethodSlot SetAerodynamicsSetupSlot = new("SetAerodynamicsSetup");
    private static readonly MethodSlot SetAutoStartEngineSlot = new("SetAutoStartEngine");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetDifferentialSetupSlot = new("SetDifferentialSetup");
    private static readonly MethodSlot SetDoorSlot = new("SetDoor");
    private static readonly MethodSlot SetEngineSetupSlot = new("SetEngineSetup");
    private static readonly MethodSlot SetEngineStartedSlot = new("SetEngineStarted");
    private static readonly MethodSlot SetHeadlightsEnabledSlot = new("SetHeadlightsEnabled");
    private static readonly MethodSlot SetHeadlightsSetupSlot = new("SetHeadlightsSetup");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetHornSoundSlot = new("SetHornSound");
    private static readonly MethodSlot SetInvulnerableSlot = new("SetInvulnerable");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetSteeringSetupSlot = new("SetSteeringSetup");
    private static readonly MethodSlot SetSteeringWheelSetupSlot = new("SetSteeringWheelSetup");
    private static readonly MethodSlot SetTaillightsEnabledSlot = new("SetTaillightsEnabled");
    private static readonly MethodSlot SetTaillightsSetupSlot = new("SetTaillightsSetup");
    private static readonly MethodSlot SetTireFlatSlot = new("SetTireFlat");
    private static readonly MethodSlot SetTransmissionSetupSlot = new("SetTransmissionSetup");
    private static readonly MethodSlot SetWheelSlot = new("SetWheel");

    /// <summary>Gets the current health</summary>
    public uint Health
    {
        get => GetHealth();
        set => SetHealth(value);
    }

    /// <summary>Gets the Max Health</summary>
    public uint MaxHealth
    {
        get => GetMaxHealth();
        set => SetMaxHealth(value);
    }

    /// <summary>Checks if the engine is started</summary>
    public bool EngineStarted
    {
        get => IsEngineStarted();
        set => SetEngineStarted(value);
    }

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool Invulnerable
    {
        get => IsInvulnerable();
        set => SetInvulnerable(value);
    }

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        CallResult<uint, uint, string?, DamageType?, Vector?, Player?, Entity?>(ApplyDamageSlot, 1, damage, boneName, damageType, fromDirection, instigator, causer)!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) =>
        CallResult<float, string>(GetDamageMultiplierSlot, 1, boneName)!;

    /// <summary>Starts or stops the vehicle horn</summary>
    public void Horn(bool enableHorn) => Call(HornSlot, 1, enableHorn);

    /// <summary>Recreates the physics state of the vehicle</summary>
    public void RecreatePhysics() => Call(RecreatePhysicsSlot, 0);

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) => Call(RespawnSlot, 0, location, rotation);

    /// <summary>Configures the Vehicle Aerodynamics Settings</summary>
    public void SetAerodynamicsSetup(
        ushort? mass = null,
        float? dragCoefficient = null,
        ushort? vehicleChassisWidth = null,
        ushort? vehicleChassisHeight = null,
        float? vehicleDownforceCoefficient = null,
        Vector? centerOfMassOverride = null) =>
        Call(SetAerodynamicsSetupSlot, 0, mass, dragCoefficient, vehicleChassisWidth, vehicleChassisHeight, vehicleDownforceCoefficient, centerOfMassOverride);

    /// <summary>Sets if the Engine auto starts when the driver enters the Vehicle</summary>
    public void SetAutoStartEngine(bool autoStart) => Call(SetAutoStartEngineSlot, 1, autoStart);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Call(SetDamageMultiplierSlot, 2, boneName, multiplier);

    /// <summary>Configures the type of Differential</summary>
    public void SetDifferentialSetup(DifferentialType differentialType, float? frontRearSplit = null) =>
        Call(SetDifferentialSetupSlot, 1, differentialType, frontRearSplit);

    /// <summary>Adds a Door to the Vehicle</summary>
    public void SetDoor(
        byte seatIndex,
        Vector offsetLocation,
        Vector seatLocation,
        Rotator seatRotation,
        ushort triggerRadius,
        ushort leaveLateralOffset) =>
        Call(SetDoorSlot, 6, seatIndex, offsetLocation, seatLocation, seatRotation, triggerRadius, leaveLateralOffset);

    /// <summary>Configures the Vehicle Engine (Torque, RPM, Braking)</summary>
    public void SetEngineSetup(
        ushort? maxTorque = null,
        ushort? maxRpm = null,
        ushort? idleRpm = null,
        float? brakeEffect = null,
        ushort? revUpMoi = null,
        ushort? revDownRate = null,
        IDictionary<float, float>? torqueCurve = null) =>
        Call(SetEngineSetupSlot, 0, maxTorque, maxRpm, idleRpm, brakeEffect, revUpMoi, revDownRate, torqueCurve);

    /// <summary>Sets if the Engine is turned off/on</summary>
    public void SetEngineStarted(bool started) => Call(SetEngineStartedSlot, 1, started);

    /// <summary>Enables or disables the headlights</summary>
    public void SetHeadlightsEnabled(bool isEnabled) => Call(SetHeadlightsEnabledSlot, 1, isEnabled);

    /// <summary>Configures the Headlights offset and color</summary>
    public void SetHeadlightsSetup(Vector location, Color? color = null) =>
        Call(SetHeadlightsSetupSlot, 1, location, color);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Call(SetHealthSlot, 1, newHealth);

    /// <summary>Sets the sound asset used by the vehicle horn</summary>
    public void SetHornSound(string soundAsset) => Call(SetHornSoundSlot, 1, soundAsset);

    /// <summary>Sets if the Character can receive any damage</summary>
    public void SetInvulnerable(bool isInvulnerable) => Call(SetInvulnerableSlot, 1, isInvulnerable);

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Call(SetMaxHealthSlot, 1, maxHealth);

    /// <summary>Configures the Vehicle Steering</summary>
    public void SetSteeringSetup(
        SteeringType steeringType,
        float? angleRatio = null,
        IDictionary<float, float>? steeringCurve = null) =>
        Call(SetSteeringSetupSlot, 1, steeringType, angleRatio, steeringCurve);

    /// <summary>Configures where the Steering Wheel is located</summary>
    public void SetSteeringWheelSetup(Vector relativeLocation, byte radius, Rotator? rotation = null) =>
        Call(SetSteeringWheelSetupSlot, 2, relativeLocation, radius, rotation);

    /// <summary>Enables or disables the taillights</summary>
    public void SetTaillightsEnabled(bool isEnabled) => Call(SetTaillightsEnabledSlot, 1, isEnabled);

    /// <summary>Configures the Taillights offset</summary>
    public void SetTaillightsSetup(Vector location) => Call(SetTaillightsSetupSlot, 1, location);

    /// <summary>Sets a Tire as Flat or not</summary>
    public void SetTireFlat(int wheelIndex, bool isFlat) => Call(SetTireFlatSlot, 2, wheelIndex, isFlat);

    /// <summary>Configures the Vehicle Transmission</summary>
    public void SetTransmissionSetup(
        float? transmissionFinalRatio = null,
        ushort? transmissionChangeUpRpm = null,
        ushort? transmissionChangeDownRpm = null,
        float? transmissionGearChangeTime = null,
        float? transmissionEfficiency = null,
        IEnumerable<float>? forwardGearRatios = null,
        IEnumerable<float>? reverseGearRatios = null) =>
        Call(SetTransmissionSetupSlot, 0, transmissionFinalRatio, transmissionChangeUpRpm, transmissionChangeDownRpm, transmissionGearChangeTime, transmissionEfficiency, forwardGearRatios, reverseGearRatios);

    /// <summary>Configures a Vehicle Wheel</summary>
    public void SetWheel(
        int index,
        string boneName,
        ushort? radius = null,
        ushort? width = null,
        byte? maxSteerAngle = null,
        Vector? offset = null,
        bool? isAffectedByEngine = null,
        bool? isAffectedByBrake = null,
        bool? isAffectedByHandbrake = null,
        bool? hasAbsEnabled = null,
        bool? hasTractionControlEnabled = null,
        ushort? maxBrakeTorque = null,
        ushort? maxHandbrakeTorque = null,
        ushort? corneringStiffness = null,
        float? sideSlipModifier = null,
        float? frictionForceMultiplier = null,
        float? slipThreshold = null,
        float? skidThreshold = null,
        float? suspensionSpringRate = null,
        float? suspensionSpringPreload = null,
        float? suspensionMaxRaise = null,
        float? suspensionMaxDrop = null,
        float? suspensionSmoothing = null,
        float? suspensionDampingRatio = null,
        float? suspensionWheelLoadRatio = null,
        Vector? suspensionAxis = null,
        Vector? suspensionForceOffset = null,
        SuspensionSweepShape? suspensionSweepShape = null) =>
        Invoke(SetWheelSlot, ScriptArguments.Trim([index, boneName, radius, width, maxSteerAngle, offset, isAffectedByEngine, isAffectedByBrake, isAffectedByHandbrake, hasAbsEnabled, hasTractionControlEnabled, maxBrakeTorque, maxHandbrakeTorque, corneringStiffness, sideSlipModifier, frictionForceMultiplier, slipThreshold, skidThreshold, suspensionSpringRate, suspensionSpringPreload, suspensionMaxRaise, suspensionMaxDrop, suspensionSmoothing, suspensionDampingRatio, suspensionWheelLoadRatio, suspensionAxis, suspensionForceOffset, suspensionSweepShape], 2));

    /// <summary>Subscribes to the CharacterAttemptEnter event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptEnter(CharacterAttemptEnterHandler handler) => Subscribe("CharacterAttemptEnter", handler);

    /// <summary>Subscribes to the CharacterAttemptLeave event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptLeave(CharacterAttemptLeaveHandler handler) => Subscribe("CharacterAttemptLeave", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the CharacterAttemptEnter event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterAttemptEnter")]
    protected virtual bool HandleCharacterAttemptEnter(Character? character, int seat) => true;

    /// <summary>Called for the CharacterAttemptLeave event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterAttemptLeave")]
    protected virtual bool HandleCharacterAttemptLeave(Character? character) => true;

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

    /// <summary>Triggered when a Character attempts to enter the Vehicle</summary>
    public delegate bool CharacterAttemptEnterHandler(VehicleWheeled self, Character? character, int seat);

    /// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
    public delegate bool CharacterAttemptLeaveHandler(VehicleWheeled self, Character? character);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(VehicleWheeled self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(VehicleWheeled self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
