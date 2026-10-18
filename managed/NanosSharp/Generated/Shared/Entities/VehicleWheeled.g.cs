#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Vehicles are wheeled entities which Characters can possess and drive.</summary>
[ScriptClass("VehicleWheeled")]
public partial class VehicleWheeled : Actor, IPaintable, IVehicle
{
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

    private static readonly ClassEvent<CharacterEnterHandler> CharacterEnterEvent = new("VehicleWheeled", "CharacterEnter");
    private static readonly ClassEvent<CharacterLeaveHandler> CharacterLeaveEvent = new("VehicleWheeled", "CharacterLeave");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("VehicleWheeled", "ClassRegister");
    private static readonly ClassEvent<DeathHandler> DeathEvent = new("VehicleWheeled", "Death");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("VehicleWheeled", "Destroy");
    private static readonly ClassEvent<EngineStartHandler> EngineStartEvent = new("VehicleWheeled", "EngineStart");
    private static readonly ClassEvent<EngineStopHandler> EngineStopEvent = new("VehicleWheeled", "EngineStop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("VehicleWheeled", "EnterWater");
    private static readonly ClassEvent<HealthChangeHandler> HealthChangeEvent = new("VehicleWheeled", "HealthChange");
    private static readonly ClassEvent<HitHandler> HitEvent = new("VehicleWheeled", "Hit");
    private static readonly ClassEvent<HornHandler> HornEvent = new("VehicleWheeled", "Horn");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("VehicleWheeled", "LeaveWater");
    private static readonly ClassEvent<RespawnHandler> RespawnEvent = new("VehicleWheeled", "Respawn");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("VehicleWheeled", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("VehicleWheeled", "TakeDamage");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("VehicleWheeled", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot BindAnimationBlueprintEventDispatcherSlot = new("BindAnimationBlueprintEventDispatcher");
    private static readonly MethodSlot CallAnimationBlueprintEventSlot = new("CallAnimationBlueprintEvent");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetAnimationBlueprintPropertyValueSlot = new("GetAnimationBlueprintPropertyValue");
    private static readonly MethodSlot GetAssetNameSlot = new("GetAssetName");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetDoorsSlot = new("GetDoors");
    private static readonly MethodSlot GetHealthSlot = new("GetHealth");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMaxHealthSlot = new("GetMaxHealth");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetPassengerSlot = new("GetPassenger");
    private static readonly MethodSlot GetPassengersSlot = new("GetPassengers");
    private static readonly MethodSlot HideBoneSlot = new("HideBone");
    private static readonly MethodSlot IsBoneHiddenSlot = new("IsBoneHidden");
    private static readonly MethodSlot IsDeadSlot = new("IsDead");
    private static readonly MethodSlot IsEngineStartedSlot = new("IsEngineStarted");
    private static readonly MethodSlot IsInvulnerableSlot = new("IsInvulnerable");
    private static readonly MethodSlot PlayAnimationSlot = new("PlayAnimation");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAnimationBlueprintPropertyValueSlot = new("SetAnimationBlueprintPropertyValue");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetCameraArmLengthSlot = new("SetCameraArmLength");
    private static readonly MethodSlot SetCameraOffsetSlot = new("SetCameraOffset");
    private static readonly MethodSlot SetExplosionSettingsSlot = new("SetExplosionSettings");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot StopAnimationSlot = new("StopAnimation");
    private static readonly MethodSlot UnHideBoneSlot = new("UnHideBone");
    private static readonly MethodSlot UnbindAnimationBlueprintEventDispatcherSlot = new("UnbindAnimationBlueprintEventDispatcher");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets the asset path the entity was spawned with</summary>
    public string AssetName => GetAssetName();

    /// <summary>Gets all configured Doors</summary>
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Doors => GetDoors();

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

    /// <summary>Gets the Asset name</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets all passengers</summary>
    public IReadOnlyList<object?> Passengers => GetPassengers();

    /// <summary>Returns if the entity is dead</summary>
    public bool Dead => IsDead();

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
        Invoke(AddSkeletalMeshAttachedSlot, ScriptArguments.Trim([id, skeletalMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, useBaseLeaderPoseComponent, animationPath, attachableId], 2));

    /// <summary>Attaches a Static Mesh to this entity</summary>
    public void AddStaticMeshAttached(
        string id,
        string staticMeshPath,
        string? socket = null,
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? useParentBounds = null,
        string? attachableId = null) =>
        Call(AddStaticMeshAttachedSlot, 2, id, staticMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, attachableId);

    /// <summary>Assigns and Binds an Animation Blueprint Event Dispatcher</summary>
    public ScriptFunction BindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate callback) =>
        CallResult<ScriptFunction, string, ScriptCallback?>(BindAnimationBlueprintEventDispatcherSlot, 2, dispatcherName, ScriptCallback.From(callback))!;

    /// <summary>Calls an Animation Blueprint Event or Function</summary>
    public object?[] CallAnimationBlueprintEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic<object?[]>(CallAnimationBlueprintEventSlot, [eventName], arguments)!;

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot, 0)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot, 0)!;

    /// <summary>Gets an Animation Blueprint Property/Variable value directly</summary>
    public object? GetAnimationBlueprintPropertyValue(string propertyName) =>
        CallResult<object?, string>(GetAnimationBlueprintPropertyValueSlot, 1, propertyName);

    /// <summary>Gets the asset path the entity was spawned with</summary>
    public string GetAssetName() => CallResult<string>(GetAssetNameSlot, 0)!;

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets all configured Doors</summary>
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> GetDoors() =>
        CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>>(GetDoorsSlot, 0)!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => CallResult<uint>(GetHealthSlot, 0)!;

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector GetInitialLocation() => CallResult<Vector>(GetInitialLocationSlot, 0)!;

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

    /// <summary>Gets the Max Health</summary>
    public uint GetMaxHealth() => CallResult<uint>(GetMaxHealthSlot, 0)!;

    /// <summary>Gets the Asset name</summary>
    public string GetMesh() => CallResult<string>(GetMeshSlot, 0)!;

    /// <summary>Gets a passenger from a seat</summary>
    public Character? GetPassenger(int seat) => CallResult<Character?, int>(GetPassengerSlot, 1, seat);

    /// <summary>Gets all passengers</summary>
    public IReadOnlyList<object?> GetPassengers() => CallResult<IReadOnlyList<object?>>(GetPassengersSlot, 0)!;

    /// <summary>Hides a bone of this Vehicle</summary>
    public void HideBone(string boneName) => Call(HideBoneSlot, 1, boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => CallResult<bool, string>(IsBoneHiddenSlot, 1, boneName)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => CallResult<bool>(IsDeadSlot, 0)!;

    /// <summary>Checks if the engine is started</summary>
    public bool IsEngineStarted() => CallResult<bool>(IsEngineStartedSlot, 0)!;

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool IsInvulnerable() => CallResult<bool>(IsInvulnerableSlot, 0)!;

    /// <summary>Plays an Animation on this Vehicle</summary>
    public void PlayAnimation(
        string animationPath,
        string? slotName = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Call(PlayAnimationSlot, 1, animationPath, slotName, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages);

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Call(RemoveAllSkeletalMeshesAttachedSlot, 0);

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Call(RemoveAllStaticMeshesAttachedSlot, 0);

    /// <summary>Removes, if it exists, a SkeletalMesh from this Vehicle given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Call(RemoveSkeletalMeshAttachedSlot, 1, id);

    /// <summary>Removes, if it exists, a StaticMesh from this Vehicle given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Call(RemoveStaticMeshAttachedSlot, 1, id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Call(ResetMaterialSlot, 0, index, attachableId);

    /// <summary>Sets an Animation Blueprint Property/Variable value directly</summary>
    public void SetAnimationBlueprintPropertyValue(string propertyName, object? value) =>
        Call(SetAnimationBlueprintPropertyValueSlot, 2, propertyName, value);

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    public void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null) =>
        Call(SetBuoyancyPontoonsSlot, 0, pontoons);

    /// <summary>Sets how this entity behaves on Water</summary>
    public void SetBuoyancySettings(
        bool? enabled = null,
        float? buoyancyCoefficient = null,
        float? buoyancyDamp = null,
        float? maxBuoyantForce = null) =>
        Call(SetBuoyancySettingsSlot, 0, enabled, buoyancyCoefficient, buoyancyDamp, maxBuoyantForce);

    /// <summary>Sets how far the camera stays from this Vehicle while driving it</summary>
    public void SetCameraArmLength(ushort armLength) => Call(SetCameraArmLengthSlot, 1, armLength);

    /// <summary>Sets the Vehicle Camera Offset</summary>
    public void SetCameraOffset(Vector offset) => Call(SetCameraOffsetSlot, 1, offset);

    /// <summary>Configures the Explosion Settings when health reaches zero</summary>
    public void SetExplosionSettings(
        Vector engineRelativeLocation,
        IEnumerable<byte> materialsIndexBurntOverride,
        IEnumerable<byte> materialsIndexBurntInvisible) =>
        Call(SetExplosionSettingsSlot, 3, engineRelativeLocation, materialsIndexBurntOverride, materialsIndexBurntInvisible);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

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

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Call(SetStaticMeshAttachedTransformSlot, 3, id, relativeLocation, relativeRotation);

    /// <summary>Stops an Animation Montage on this Vehicle</summary>
    public void StopAnimation(string? animationAsset = null) => Call(StopAnimationSlot, 0, animationAsset);

    /// <summary>UnHide a bone of this Vehicle</summary>
    public void UnHideBone(string boneName) => Call(UnHideBoneSlot, 1, boneName);

    /// <summary>Unbinds an Animation Blueprint Event Dispatcher</summary>
    public void UnbindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null) =>
        Call(UnbindAnimationBlueprintEventDispatcherSlot, 1, dispatcherName, ScriptCallback.From(callback));

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

    /// <summary>Subscribes to the Respawn event of this entity only.</summary>
    public ScriptSubscription SubscribeRespawn(RespawnHandler handler) => Subscribe("Respawn", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the CharacterEnter event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterEnter")]
    protected virtual void HandleCharacterEnter(Character? character, int seatIndex)
    {
    }

    /// <summary>Called for the CharacterLeave event of this entity when a subclass overrides it.</summary>
    [EventHook("CharacterLeave")]
    protected virtual void HandleCharacterLeave(Character? character, int seatIndex)
    {
    }

    /// <summary>Called for the Death event of this entity when a subclass overrides it.</summary>
    [EventHook("Death")]
    protected virtual void HandleDeath(int lastDamageTaken, string? lastBoneDamaged, DamageType damageTypeReason, Vector hitFromDirection, Player? instigator, Actor? causer)
    {
    }

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the EngineStart event of this entity when a subclass overrides it.</summary>
    [EventHook("EngineStart")]
    protected virtual void HandleEngineStart()
    {
    }

    /// <summary>Called for the EngineStop event of this entity when a subclass overrides it.</summary>
    [EventHook("EngineStop")]
    protected virtual void HandleEngineStop()
    {
    }

    /// <summary>Called for the EnterWater event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterWater")]
    protected virtual void HandleEnterWater()
    {
    }

    /// <summary>Called for the HealthChange event of this entity when a subclass overrides it.</summary>
    [EventHook("HealthChange")]
    protected virtual void HandleHealthChange(int oldHealth, int newHealth)
    {
    }

    /// <summary>Called for the Hit event of this entity when a subclass overrides it.</summary>
    [EventHook("Hit")]
    protected virtual void HandleHit(float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor)
    {
    }

    /// <summary>Called for the Horn event of this entity when a subclass overrides it.</summary>
    [EventHook("Horn")]
    protected virtual void HandleHorn(bool isHonking)
    {
    }

    /// <summary>Called for the LeaveWater event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveWater")]
    protected virtual void HandleLeaveWater()
    {
    }

    /// <summary>Called for the Respawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Respawn")]
    protected virtual void HandleRespawn()
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
    {
    }

    /// <summary>Called for the TakeDamage event of this entity when a subclass overrides it.</summary>
    [EventHook("TakeDamage")]
    protected virtual void HandleTakeDamage(int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer)
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

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

    /// <summary>When Entity Respawns</summary>
    public delegate void RespawnHandler(VehicleWheeled self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(VehicleWheeled self);

    /// <summary>Triggered when this Entity takes damage</summary>
    public delegate void TakeDamageHandler(VehicleWheeled self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(VehicleWheeled self, string? key, object? value);
}
