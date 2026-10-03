#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Grenades are Pickables which Characters can grab and throw. It explodes after X seconds after thrown, causing damage around.</summary>
[ScriptClass("Grenade")]
public partial class Grenade : Actor, IPaintable, IPickable
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

    /// <summary>When a Character drops this Pickable</summary>
    public static event DropHandler? OnDrop
    {
        add => DropEvent.Add(value);
        remove => DropEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>Triggered when the grenade has exploded</summary>
    public static event ExplodeHandler? OnExplode
    {
        add => ExplodeEvent.Add(value);
        remove => ExplodeEvent.Remove(value);
    }

    /// <summary>When this Pickable hits something</summary>
    public static event HitHandler? OnHit
    {
        add => HitEvent.Add(value);
        remove => HitEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public static event InteractHandler? OnInteract
    {
        add => InteractEvent.Add(value);
        remove => InteractEvent.Remove(value);
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

    /// <summary>Triggered When a Character picks this up</summary>
    public static event PickUpHandler? OnPickUp
    {
        add => PickUpEvent.Add(value);
        remove => PickUpEvent.Remove(value);
    }

    /// <summary>Triggered when a Character presses the use button for this Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public static event PullUseHandler? OnPullUse
    {
        add => PullUseEvent.Add(value);
        remove => PullUseEvent.Remove(value);
    }

    /// <summary>Triggered when a Character releases the use button for this Pickable (i.e. releases left mouse button with this equipped)</summary>
    public static event ReleaseUseHandler? OnReleaseUse
    {
        add => ReleaseUseEvent.Add(value);
        remove => ReleaseUseEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when the grenade has been thrown</summary>
    public static event ThrowHandler? OnThrow
    {
        add => ThrowEvent.Add(value);
        remove => ThrowEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Grenade", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Grenade", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Grenade", "DimensionChange");
    private static readonly ClassEvent<DropHandler> DropEvent = new("Grenade", "Drop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Grenade", "EnterWater");
    private static readonly ClassEvent<ExplodeHandler> ExplodeEvent = new("Grenade", "Explode");
    private static readonly ClassEvent<HitHandler> HitEvent = new("Grenade", "Hit");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Grenade", "Interact");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Grenade", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Grenade", "NetworkAuthorityChange");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("Grenade", "PickUp");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("Grenade", "PullUse");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("Grenade", "ReleaseUse");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Grenade", "Spawn");
    private static readonly ClassEvent<ThrowHandler> ThrowEvent = new("Grenade", "Throw");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Grenade", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot ExplodeSlot = new("Explode");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetBaseDamageSlot = new("GetBaseDamage");
    private static readonly MethodSlot GetDamageFalloffSlot = new("GetDamageFalloff");
    private static readonly MethodSlot GetDamageInnerRadiusSlot = new("GetDamageInnerRadius");
    private static readonly MethodSlot GetDamageOuterRadiusSlot = new("GetDamageOuterRadius");
    private static readonly MethodSlot GetHandlerSlot = new("GetHandler");
    private static readonly MethodSlot GetHandlingModeSlot = new("GetHandlingMode");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetMinimumDamageSlot = new("GetMinimumDamage");
    private static readonly MethodSlot GetThrowForceSlot = new("GetThrowForce");
    private static readonly MethodSlot GetTimeToExplodeSlot = new("GetTimeToExplode");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAttachmentSettingsSlot = new("SetAttachmentSettings");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetCanUseSlot = new("SetCanUse");
    private static readonly MethodSlot SetCrosshairMaterialSlot = new("SetCrosshairMaterial");
    private static readonly MethodSlot SetDamageSlot = new("SetDamage");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPickableSlot = new("SetPickable");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot SetThrowForceSlot = new("SetThrowForce");
    private static readonly MethodSlot SetTimeToExplodeSlot = new("SetTimeToExplode");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets the max damage, applied within the inner radius</summary>
    public uint BaseDamage => GetBaseDamage();

    /// <summary>Gets the falloff exponent used to interpolate between the base and the minimum damage</summary>
    public float DamageFalloff => GetDamageFalloff();

    /// <summary>Gets the radius within which the base damage is fully applied</summary>
    public ushort DamageInnerRadius => GetDamageInnerRadius();

    /// <summary>Gets the radius at which the damage reaches the minimum damage</summary>
    public ushort DamageOuterRadius => GetDamageOuterRadius();

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? Handler => GetHandler();

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode HandlingMode => GetHandlingMode();

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets the min damage, applied at the outer radius</summary>
    public uint MinimumDamage => GetMinimumDamage();

    /// <summary>Gets the impulse applied when throwing</summary>
    public ushort ThrowForce
    {
        get => GetThrowForce();
        set => SetThrowForce(value);
    }

    /// <summary>Gets the time until the explosion after throwing</summary>
    public float TimeToExplode
    {
        get => GetTimeToExplode();
        set => SetTimeToExplode(value);
    }

    /// <summary>Spawns a new Grenade.</summary>
    public Grenade(
        Vector location,
        Rotator rotation,
        string? staticMeshAsset = null,
        string? explosionParticles = null,
        string? explosionSound = null,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Grenade", ScriptArguments.Trim([location, rotation, staticMeshAsset, explosionParticles, explosionSound, collisionType, gravityEnabled, spawnMode], 2));
    }

    protected Grenade()
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
        Invoke(AddStaticMeshAttachedSlot, ScriptArguments.Trim([id, staticMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, attachableId], 2));

    /// <summary>Forces this grenade to Explode</summary>
    public void Explode() => Invoke(ExplodeSlot);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot)!;

    /// <summary>Gets the max damage, applied within the inner radius</summary>
    public uint GetBaseDamage() => Invoke<uint>(GetBaseDamageSlot)!;

    /// <summary>Gets the falloff exponent used to interpolate between the base and the minimum damage</summary>
    public float GetDamageFalloff() => Invoke<float>(GetDamageFalloffSlot)!;

    /// <summary>Gets the radius within which the base damage is fully applied</summary>
    public ushort GetDamageInnerRadius() => Invoke<ushort>(GetDamageInnerRadiusSlot)!;

    /// <summary>Gets the radius at which the damage reaches the minimum damage</summary>
    public ushort GetDamageOuterRadius() => Invoke<ushort>(GetDamageOuterRadiusSlot)!;

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? GetHandler() => Invoke<Character?>(GetHandlerSlot);

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode GetHandlingMode() => Invoke<HandlingMode>(GetHandlingModeSlot)!;

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

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    public string GetMesh() => Invoke<string>(GetMeshSlot)!;

    /// <summary>Gets the min damage, applied at the outer radius</summary>
    public uint GetMinimumDamage() => Invoke<uint>(GetMinimumDamageSlot)!;

    /// <summary>Gets the impulse applied when throwing</summary>
    public ushort GetThrowForce() => Invoke<ushort>(GetThrowForceSlot)!;

    /// <summary>Gets the time until the explosion after throwing</summary>
    public float GetTimeToExplode() => Invoke<float>(GetTimeToExplodeSlot)!;

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) =>
        Invoke(PullUseSlot, ScriptArguments.Trim([releaseUseAfter], 0));

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Invoke(ReleaseUseSlot);

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Invoke(RemoveAllSkeletalMeshesAttachedSlot);

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Invoke(RemoveAllStaticMeshesAttachedSlot);

    /// <summary>Removes, if it exists, a SkeletalMesh from this Pickable given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Invoke(RemoveSkeletalMeshAttachedSlot, id);

    /// <summary>Removes, if it exists, a StaticMesh from this Pickable given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Invoke(RemoveStaticMeshAttachedSlot, id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke(ResetMaterialSlot, ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Sets the Attachment Settings for this Pickable (how it attaches to the Character when Picking up)</summary>
    public void SetAttachmentSettings(
        Vector relativeLocation,
        Rotator? relativeRotation = null,
        string? socket = null) =>
        Invoke(SetAttachmentSettingsSlot, ScriptArguments.Trim([relativeLocation, relativeRotation, socket], 1));

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    public void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null) =>
        Invoke(SetBuoyancyPontoonsSlot, ScriptArguments.Trim([pontoons], 0));

    /// <summary>Sets how this entity behaves on Water</summary>
    public void SetBuoyancySettings(
        bool? enabled = null,
        float? buoyancyCoefficient = null,
        float? buoyancyDamp = null,
        float? maxBuoyantForce = null) =>
        Invoke(SetBuoyancySettingsSlot, ScriptArguments.Trim([enabled, buoyancyCoefficient, buoyancyDamp, maxBuoyantForce], 0));

    /// <summary>Sets if this Pickable can be used by the player when held</summary>
    public void SetCanUse(bool canUse) => Invoke(SetCanUseSlot, canUse);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    public void SetCrosshairMaterial(string materialAsset) => Invoke(SetCrosshairMaterialSlot, materialAsset);

    /// <summary>Sets Damage parameters of this Grenade</summary>
    public void SetDamage(
        uint? baseDamage = null,
        uint? minimumDamage = null,
        ushort? damageInnerRadius = null,
        ushort? damageOuterRadius = null,
        float? damageFalloff = null) =>
        Invoke(SetDamageSlot, ScriptArguments.Trim([baseDamage, minimumDamage, damageInnerRadius, damageOuterRadius, damageFalloff], 0));

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

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    public void SetPickable(bool isPickable) => Invoke(SetPickableSlot, isPickable);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke(SetStaticMeshAttachedTransformSlot, id, relativeLocation, relativeRotation);

    /// <summary>Set Impulse applied when throwing</summary>
    public void SetThrowForce(ushort throwForce) => Invoke(SetThrowForceSlot, throwForce);

    /// <summary>Set Time until Explosion after thrown</summary>
    public void SetTimeToExplode(float time) => Invoke(SetTimeToExplodeSlot, time);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Drop event of this entity only.</summary>
    public ScriptSubscription SubscribeDrop(DropHandler handler) => Subscribe("Drop", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the Explode event of this entity only.</summary>
    public ScriptSubscription SubscribeExplode(ExplodeHandler handler) => Subscribe("Explode", handler);

    /// <summary>Subscribes to the Hit event of this entity only.</summary>
    public ScriptSubscription SubscribeHit(HitHandler handler) => Subscribe("Hit", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the PickUp event of this entity only.</summary>
    public ScriptSubscription SubscribePickUp(PickUpHandler handler) => Subscribe("PickUp", handler);

    /// <summary>Subscribes to the PullUse event of this entity only.</summary>
    public ScriptSubscription SubscribePullUse(PullUseHandler handler) => Subscribe("PullUse", handler);

    /// <summary>Subscribes to the ReleaseUse event of this entity only.</summary>
    public ScriptSubscription SubscribeReleaseUse(ReleaseUseHandler handler) => Subscribe("ReleaseUse", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the Throw event of this entity only.</summary>
    public ScriptSubscription SubscribeThrow(ThrowHandler handler) => Subscribe("Throw", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Grenade self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Grenade self, int oldDimension, int newDimension);

    /// <summary>When a Character drops this Pickable</summary>
    public delegate void DropHandler(Grenade self, Character? character, bool wasTriggeredByPlayer);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Grenade self);

    /// <summary>Triggered when the grenade has exploded</summary>
    public delegate void ExplodeHandler(Grenade self);

    /// <summary>When this Pickable hits something</summary>
    public delegate void HitHandler(Grenade self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Grenade self, Character? character);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Grenade self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Grenade self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered When a Character picks this up</summary>
    public delegate void PickUpHandler(Grenade self, Character? character);

    /// <summary>Triggered when a Character presses the use button for this Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public delegate void PullUseHandler(Grenade self, Character? character);

    /// <summary>Triggered when a Character releases the use button for this Pickable (i.e. releases left mouse button with this equipped)</summary>
    public delegate void ReleaseUseHandler(Grenade self, Character? character);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Grenade self);

    /// <summary>Triggered when the grenade has been thrown</summary>
    public delegate void ThrowHandler(Grenade self, Character? handler);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Grenade self, string? key, object? value);
}
