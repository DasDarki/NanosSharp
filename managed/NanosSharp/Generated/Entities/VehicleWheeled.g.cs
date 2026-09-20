#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Vehicles are wheeled entities which Characters can possess and drive.</summary>
[ScriptClass("VehicleWheeled")]
public partial class VehicleWheeled : Actor, IDamageable, IPaintable, IVehicle
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

    /// <summary>Triggered when a Character fully enters the Vehicle</summary>
    public static event CharacterEnterHandler? OnCharacterEnter
    {
        add => CharacterEnterEvent.Add(value);
        remove => CharacterEnterEvent.Remove(value);
    }

    /// <summary>Triggered when a Character fully leaves the Vehicle</summary>
    public static event CharacterLeaveHandler? OnCharacterLeave
    {
        add => CharacterLeaveEvent.Add(value);
        remove => CharacterLeaveEvent.Remove(value);
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
    }

    /// <summary>When Entity Dies</summary>
    public static event DeathHandler? OnDeath
    {
        add => DeathEvent.Add(value);
        remove => DeathEvent.Remove(value);
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

    /// <summary>Triggered when the engine starts</summary>
    public static event EngineStartHandler? OnEngineStart
    {
        add => EngineStartEvent.Add(value);
        remove => EngineStartEvent.Remove(value);
    }

    /// <summary>Triggered when the engine stops</summary>
    public static event EngineStopHandler? OnEngineStop
    {
        add => EngineStopEvent.Add(value);
        remove => EngineStopEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public static event HealthChangeHandler? OnHealthChange
    {
        add => HealthChangeEvent.Add(value);
        remove => HealthChangeEvent.Remove(value);
    }

    /// <summary>Triggered when Vehicle hits something</summary>
    public static event HitHandler? OnHit
    {
        add => HitEvent.Add(value);
        remove => HitEvent.Remove(value);
    }

    /// <summary>Triggered when Vehicle honks</summary>
    public static event HornHandler? OnHorn
    {
        add => HornEvent.Add(value);
        remove => HornEvent.Remove(value);
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

    /// <summary>When Entity Respawns</summary>
    public static event RespawnHandler? OnRespawn
    {
        add => RespawnEvent.Add(value);
        remove => RespawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when this Entity takes damage</summary>
    public static event TakeDamageHandler? OnTakeDamage
    {
        add => TakeDamageEvent.Add(value);
        remove => TakeDamageEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<CharacterAttemptEnterHandler> CharacterAttemptEnterEvent = new("VehicleWheeled", "CharacterAttemptEnter");
    private static readonly ClassEvent<CharacterAttemptLeaveHandler> CharacterAttemptLeaveEvent = new("VehicleWheeled", "CharacterAttemptLeave");
    private static readonly ClassEvent<CharacterEnterHandler> CharacterEnterEvent = new("VehicleWheeled", "CharacterEnter");
    private static readonly ClassEvent<CharacterLeaveHandler> CharacterLeaveEvent = new("VehicleWheeled", "CharacterLeave");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("VehicleWheeled", "ClassRegister");
    private static readonly ClassEvent<DeathHandler> DeathEvent = new("VehicleWheeled", "Death");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("VehicleWheeled", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("VehicleWheeled", "DimensionChange");
    private static readonly ClassEvent<EngineStartHandler> EngineStartEvent = new("VehicleWheeled", "EngineStart");
    private static readonly ClassEvent<EngineStopHandler> EngineStopEvent = new("VehicleWheeled", "EngineStop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("VehicleWheeled", "EnterWater");
    private static readonly ClassEvent<HealthChangeHandler> HealthChangeEvent = new("VehicleWheeled", "HealthChange");
    private static readonly ClassEvent<HitHandler> HitEvent = new("VehicleWheeled", "Hit");
    private static readonly ClassEvent<HornHandler> HornEvent = new("VehicleWheeled", "Horn");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("VehicleWheeled", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("VehicleWheeled", "NetworkAuthorityChange");
    private static readonly ClassEvent<RespawnHandler> RespawnEvent = new("VehicleWheeled", "Respawn");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("VehicleWheeled", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("VehicleWheeled", "TakeDamage");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("VehicleWheeled", "ValueChange");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets all configured Doors</summary>
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Doors => GetDoors();

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

    /// <summary>Gets the Asset name</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets all passengers</summary>
    public IReadOnlyList<object?> Passengers => GetPassengers();

    /// <summary>Returns if the entity is dead</summary>
    public bool Dead => IsDead();

    /// <summary>Checks if the engine is started</summary>
    public bool EngineStarted
    {
        get => IsEngineStarted();
        set => SetEngineStarted(value);
    }

    /// <summary>Spawns a new VehicleWheeled.</summary>
    public VehicleWheeled(
        Vector location,
        Rotator rotation,
        string asset,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        bool? autoUnflip = null,
        string? engineSound = null,
        string? hornSound = null,
        string? brakeSound = null,
        string? engineStartSound = null,
        string? vehicleDoorSound = null,
        bool? autoStartEngine = null,
        string? customAnimationBlueprint = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("VehicleWheeled", ScriptArguments.Trim([location, rotation, asset, collisionType, gravityEnabled, autoUnflip, engineSound, hornSound, brakeSound, engineStartSound, vehicleDoorSound, autoStartEngine, customAnimationBlueprint, spawnMode], 3));
    }

    protected VehicleWheeled()
    {
    }

    /// <summary>Attaches a Skeletal Mesh as master pose to this entity</summary>
    public void AddSkeletalMeshAttached(
        string id,
        string skeletalMeshPath,
        string? socket = null,
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? useParentBounds = null,
        bool? useBaseLeaderPoseComponent = null,
        string? animationPath = null,
        string? attachableId = null) =>
        Invoke("AddSkeletalMeshAttached", ScriptArguments.Trim([id, skeletalMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, useBaseLeaderPoseComponent, animationPath, attachableId], 2));

    /// <summary>Attaches a Static Mesh to this entity</summary>
    public void AddStaticMeshAttached(
        string id,
        string staticMeshPath,
        string? socket = null,
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? useParentBounds = null,
        string? attachableId = null) =>
        Invoke("AddStaticMeshAttached", ScriptArguments.Trim([id, staticMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, attachableId], 2));

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        Invoke<uint>("ApplyDamage", ScriptArguments.Trim([damage, boneName, damageType, fromDirection, instigator, causer], 1))!;

    /// <summary>Assigns and Binds an Animation Blueprint Event Dispatcher</summary>
    public ScriptFunction BindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate callback) =>
        Invoke<ScriptFunction>("BindAnimationBlueprintEventDispatcher", dispatcherName, ScriptCallback.From(callback))!;

    /// <summary>Calls an Animation Blueprint Event or Function</summary>
    public object?[] CallAnimationBlueprintEvent(string eventName, params object?[] arguments) =>
        Invoke<object?[]>("CallAnimationBlueprintEvent", [eventName, .. arguments])!;

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>("GetAllSkeletalMeshAttached")!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>("GetAllStaticMeshAttached")!;

    /// <summary>Gets an Animation Blueprint Property/Variable value directly</summary>
    public object? GetAnimationBlueprintPropertyValue(string propertyName) =>
        Invoke<object?>("GetAnimationBlueprintPropertyValue", propertyName);

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) => Invoke<float>("GetDamageMultiplier", boneName)!;

    /// <summary>Gets all configured Doors</summary>
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> GetDoors() =>
        Invoke<IReadOnlyList<IReadOnlyDictionary<string, object?>>>("GetDoors")!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => Invoke<uint>("GetHealth")!;

    /// <summary>Gets a Color parameter from this Actor's material</summary>
    public Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Color?>("GetMaterialColorParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    public float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<float?>("GetMaterialScalarParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    public string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<string?>("GetMaterialTextureParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    public Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Vector?>("GetMaterialVectorParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets the Max Health</summary>
    public uint GetMaxHealth() => Invoke<uint>("GetMaxHealth")!;

    /// <summary>Gets the Asset name</summary>
    public string GetMesh() => Invoke<string>("GetMesh")!;

    /// <summary>Gets a passenger from a seat</summary>
    public Character? GetPassenger(int seat) => Invoke<Character?>("GetPassenger", seat);

    /// <summary>Gets all passengers</summary>
    public IReadOnlyList<object?> GetPassengers() => Invoke<IReadOnlyList<object?>>("GetPassengers")!;

    /// <summary>Hides a bone of this Vehicle</summary>
    public void HideBone(string boneName) => Invoke("HideBone", boneName);

    /// <summary>Starts or stops the vehicle horn</summary>
    public void Horn(bool enableHorn) => Invoke("Horn", enableHorn);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => Invoke<bool>("IsBoneHidden", boneName)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => Invoke<bool>("IsDead")!;

    /// <summary>Checks if the engine is started</summary>
    public bool IsEngineStarted() => Invoke<bool>("IsEngineStarted")!;

    /// <summary>Plays an Animation on this Vehicle</summary>
    public void PlayAnimation(
        string animationPath,
        string? slotName = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Invoke("PlayAnimation", ScriptArguments.Trim([animationPath, slotName, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages], 1));

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Invoke("RemoveAllSkeletalMeshesAttached");

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Invoke("RemoveAllStaticMeshesAttached");

    /// <summary>Removes, if it exists, a SkeletalMesh from this Vehicle given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Invoke("RemoveSkeletalMeshAttached", id);

    /// <summary>Removes, if it exists, a StaticMesh from this Vehicle given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Invoke("RemoveStaticMeshAttached", id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke("ResetMaterial", ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) =>
        Invoke("Respawn", ScriptArguments.Trim([location, rotation], 0));

    /// <summary>Configures the Vehicle Aerodynamics Settings</summary>
    public void SetAerodynamicsSetup(
        ushort? mass = null,
        float? dragCoefficient = null,
        ushort? vehicleChassisWidth = null,
        ushort? vehicleChassisHeight = null,
        float? vehicleDownforceCoefficient = null,
        Vector? centerOfMassOverride = null) =>
        Invoke("SetAerodynamicsSetup", ScriptArguments.Trim([mass, dragCoefficient, vehicleChassisWidth, vehicleChassisHeight, vehicleDownforceCoefficient, centerOfMassOverride], 0));

    /// <summary>Sets an Animation Blueprint Property/Variable value directly</summary>
    public void SetAnimationBlueprintPropertyValue(string propertyName, object? value) =>
        Invoke("SetAnimationBlueprintPropertyValue", propertyName, value);

    /// <summary>Sets if the Engine auto starts when the driver enters the Vehicle</summary>
    public void SetAutoStartEngine(bool autoStart) => Invoke("SetAutoStartEngine", autoStart);

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    public void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null) =>
        Invoke("SetBuoyancyPontoons", ScriptArguments.Trim([pontoons], 0));

    /// <summary>Sets how this entity behaves on Water</summary>
    public void SetBuoyancySettings(
        bool? enabled = null,
        float? buoyancyCoefficient = null,
        float? buoyancyDamp = null,
        float? maxBuoyantForce = null) =>
        Invoke("SetBuoyancySettings", ScriptArguments.Trim([enabled, buoyancyCoefficient, buoyancyDamp, maxBuoyantForce], 0));

    /// <summary>Sets how far the camera stays from this Vehicle while driving it</summary>
    public void SetCameraArmLength(ushort armLength) => Invoke("SetCameraArmLength", armLength);

    /// <summary>Sets the Vehicle Camera Offset</summary>
    public void SetCameraOffset(Vector offset) => Invoke("SetCameraOffset", offset);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Invoke("SetDamageMultiplier", boneName, multiplier);

    /// <summary>Configures the type of Differential</summary>
    public void SetDifferentialSetup(DifferentialType differentialType, float? frontRearSplit = null) =>
        Invoke("SetDifferentialSetup", ScriptArguments.Trim([differentialType, frontRearSplit], 1));

    /// <summary>Adds a Door to the Vehicle</summary>
    public void SetDoor(
        byte seatIndex,
        Vector offsetLocation,
        Vector seatLocation,
        Rotator seatRotation,
        ushort triggerRadius,
        ushort leaveLateralOffset) =>
        Invoke("SetDoor", seatIndex, offsetLocation, seatLocation, seatRotation, triggerRadius, leaveLateralOffset);

    /// <summary>Configures the Vehicle Engine (Torque, RPM, Braking)</summary>
    public void SetEngineSetup(
        ushort? maxTorque = null,
        ushort? maxRpm = null,
        ushort? idleRpm = null,
        float? brakeEffect = null,
        ushort? revUpMoi = null,
        ushort? revDownRate = null,
        IDictionary<float, float>? torqueCurve = null) =>
        Invoke("SetEngineSetup", ScriptArguments.Trim([maxTorque, maxRpm, idleRpm, brakeEffect, revUpMoi, revDownRate, torqueCurve], 0));

    /// <summary>Sets if the Engine is turned off/on</summary>
    public void SetEngineStarted(bool started) => Invoke("SetEngineStarted", started);

    /// <summary>Configures the Explosion Settings when health reaches zero</summary>
    public void SetExplosionSettings(
        Vector engineRelativeLocation,
        IEnumerable<byte> materialsIndexBurntOverride,
        IEnumerable<byte> materialsIndexBurntInvisible) =>
        Invoke("SetExplosionSettings", engineRelativeLocation, materialsIndexBurntOverride, materialsIndexBurntInvisible);

    /// <summary>Enables or disables the headlights</summary>
    public void SetHeadlightsEnabled(bool isEnabled) => Invoke("SetHeadlightsEnabled", isEnabled);

    /// <summary>Configures the Headlights offset and color</summary>
    public void SetHeadlightsSetup(Vector location, Color? color = null) =>
        Invoke("SetHeadlightsSetup", ScriptArguments.Trim([location, color], 1));

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Invoke("SetHealth", newHealth);

    /// <summary>Sets the sound asset used by the vehicle horn</summary>
    public void SetHornSound(string soundAsset) => Invoke("SetHornSound", soundAsset);

    /// <summary>Sets the material at the specified index of this Actor</summary>
    public void SetMaterial(string materialPath, int? index = null, string? attachableId = null) =>
        Invoke("SetMaterial", ScriptArguments.Trim([materialPath, index, attachableId], 1));

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    public void SetMaterialColorParameter(
        string parameterName,
        Color color,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialColorParameter", ScriptArguments.Trim([parameterName, color, index, attachableId], 2));

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    public void SetMaterialScalarParameter(
        string parameterName,
        float scalar,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialScalarParameter", ScriptArguments.Trim([parameterName, scalar, index, attachableId], 2));

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    public void SetMaterialTextureParameter(
        string parameterName,
        string texturePath,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialTextureParameter", ScriptArguments.Trim([parameterName, texturePath, index, attachableId], 2));

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    public void SetMaterialVectorParameter(
        string parameterName,
        Vector vector,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialVectorParameter", ScriptArguments.Trim([parameterName, vector, index, attachableId], 2));

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Invoke("SetMaxHealth", maxHealth);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) => Invoke("SetPhysicalMaterial", physicalMaterialPath);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke("SetStaticMeshAttachedTransform", id, relativeLocation, relativeRotation);

    /// <summary>Configures the Vehicle Steering</summary>
    public void SetSteeringSetup(
        SteeringType steeringType,
        float? angleRatio = null,
        IDictionary<float, float>? steeringCurve = null) =>
        Invoke("SetSteeringSetup", ScriptArguments.Trim([steeringType, angleRatio, steeringCurve], 1));

    /// <summary>Configures where the Steering Wheel is located</summary>
    public void SetSteeringWheelSetup(Vector relativeLocation, byte radius, Rotator? rotation = null) =>
        Invoke("SetSteeringWheelSetup", ScriptArguments.Trim([relativeLocation, radius, rotation], 2));

    /// <summary>Enables or disables the taillights</summary>
    public void SetTaillightsEnabled(bool isEnabled) => Invoke("SetTaillightsEnabled", isEnabled);

    /// <summary>Configures the Taillights offset</summary>
    public void SetTaillightsSetup(Vector location) => Invoke("SetTaillightsSetup", location);

    /// <summary>Sets a Tire as Flat or not</summary>
    public void SetTireFlat(int wheelIndex, bool isFlat) => Invoke("SetTireFlat", wheelIndex, isFlat);

    /// <summary>Configures the Vehicle Transmission</summary>
    public void SetTransmissionSetup(
        float? transmissionFinalRatio = null,
        ushort? transmissionChangeUpRpm = null,
        ushort? transmissionChangeDownRpm = null,
        float? transmissionGearChangeTime = null,
        float? transmissionEfficiency = null,
        IEnumerable<float>? forwardGearRatios = null,
        IEnumerable<float>? reverseGearRatios = null) =>
        Invoke("SetTransmissionSetup", ScriptArguments.Trim([transmissionFinalRatio, transmissionChangeUpRpm, transmissionChangeDownRpm, transmissionGearChangeTime, transmissionEfficiency, forwardGearRatios, reverseGearRatios], 0));

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
        Invoke("SetWheel", ScriptArguments.Trim([index, boneName, radius, width, maxSteerAngle, offset, isAffectedByEngine, isAffectedByBrake, isAffectedByHandbrake, hasAbsEnabled, hasTractionControlEnabled, maxBrakeTorque, maxHandbrakeTorque, corneringStiffness, sideSlipModifier, frictionForceMultiplier, slipThreshold, skidThreshold, suspensionSpringRate, suspensionSpringPreload, suspensionMaxRaise, suspensionMaxDrop, suspensionSmoothing, suspensionDampingRatio, suspensionWheelLoadRatio, suspensionAxis, suspensionForceOffset, suspensionSweepShape], 2));

    /// <summary>Stops an Animation Montage on this Vehicle</summary>
    public void StopAnimation(string? animationAsset = null) =>
        Invoke("StopAnimation", ScriptArguments.Trim([animationAsset], 0));

    /// <summary>UnHide a bone of this Vehicle</summary>
    public void UnHideBone(string boneName) => Invoke("UnHideBone", boneName);

    /// <summary>Unbinds an Animation Blueprint Event Dispatcher</summary>
    public void UnbindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null) =>
        Invoke("UnbindAnimationBlueprintEventDispatcher", ScriptArguments.Trim([dispatcherName, ScriptCallback.From(callback)], 1));

    /// <summary>Subscribes to the CharacterAttemptEnter event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptEnter(CharacterAttemptEnterHandler handler) => Subscribe("CharacterAttemptEnter", handler);

    /// <summary>Subscribes to the CharacterAttemptLeave event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterAttemptLeave(CharacterAttemptLeaveHandler handler) => Subscribe("CharacterAttemptLeave", handler);

    /// <summary>Subscribes to the CharacterEnter event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterEnter(CharacterEnterHandler handler) => Subscribe("CharacterEnter", handler);

    /// <summary>Subscribes to the CharacterLeave event of this entity only.</summary>
    public ScriptSubscription SubscribeCharacterLeave(CharacterLeaveHandler handler) => Subscribe("CharacterLeave", handler);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Death event of this entity only.</summary>
    public ScriptSubscription SubscribeDeath(DeathHandler handler) => Subscribe("Death", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EngineStart event of this entity only.</summary>
    public ScriptSubscription SubscribeEngineStart(EngineStartHandler handler) => Subscribe("EngineStart", handler);

    /// <summary>Subscribes to the EngineStop event of this entity only.</summary>
    public ScriptSubscription SubscribeEngineStop(EngineStopHandler handler) => Subscribe("EngineStop", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the HealthChange event of this entity only.</summary>
    public ScriptSubscription SubscribeHealthChange(HealthChangeHandler handler) => Subscribe("HealthChange", handler);

    /// <summary>Subscribes to the Hit event of this entity only.</summary>
    public ScriptSubscription SubscribeHit(HitHandler handler) => Subscribe("Hit", handler);

    /// <summary>Subscribes to the Horn event of this entity only.</summary>
    public ScriptSubscription SubscribeHorn(HornHandler handler) => Subscribe("Horn", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Respawn event of this entity only.</summary>
    public ScriptSubscription SubscribeRespawn(RespawnHandler handler) => Subscribe("Respawn", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a Character attempts to enter the Vehicle</summary>
    public delegate bool CharacterAttemptEnterHandler(VehicleWheeled self, Character? character, int seat);

    /// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
    public delegate bool CharacterAttemptLeaveHandler(VehicleWheeled self, Character? character);

    /// <summary>Triggered when a Character fully enters the Vehicle</summary>
    public delegate void CharacterEnterHandler(VehicleWheeled self, Character? character, int seatIndex);

    /// <summary>Triggered when a Character fully leaves the Vehicle</summary>
    public delegate void CharacterLeaveHandler(VehicleWheeled self, Character? character, int seatIndex);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>When Entity Dies</summary>
    public delegate void DeathHandler(VehicleWheeled self, int lastDamageTaken, string? lastBoneDamaged, DamageType damageTypeReason, Vector hitFromDirection, Player? instigator, Actor? causer);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(VehicleWheeled self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(VehicleWheeled self, int oldDimension, int newDimension);

    /// <summary>Triggered when the engine starts</summary>
    public delegate void EngineStartHandler(VehicleWheeled self);

    /// <summary>Triggered when the engine stops</summary>
    public delegate void EngineStopHandler(VehicleWheeled self);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(VehicleWheeled self);

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public delegate void HealthChangeHandler(VehicleWheeled self, int oldHealth, int newHealth);

    /// <summary>Triggered when Vehicle hits something</summary>
    public delegate void HitHandler(VehicleWheeled self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>Triggered when Vehicle honks</summary>
    public delegate void HornHandler(VehicleWheeled self, bool isHonking);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(VehicleWheeled self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(VehicleWheeled self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>When Entity Respawns</summary>
    public delegate void RespawnHandler(VehicleWheeled self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(VehicleWheeled self);

    /// <summary>Triggered when this Entity takes damage</summary>
    public delegate void TakeDamageHandler(VehicleWheeled self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(VehicleWheeled self, string? key, object? value);
}
