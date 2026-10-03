#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Melee represents an Entity which can be Pickable by a Character and can be used to melee attack, Characters can hold it with hands with pre-defined handling modes.</summary>
[ScriptClass("Melee")]
public partial class Melee : Actor, IPaintable, IPickable
{
    /// <summary>Triggered when the Character effectively attacks with this Melee</summary>
    public static event AttackHandler? OnAttack
    {
        add => AttackEvent.Add(value);
        remove => AttackEvent.Remove(value);
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

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<AttackHandler> AttackEvent = new("Melee", "Attack");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Melee", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Melee", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Melee", "DimensionChange");
    private static readonly ClassEvent<DropHandler> DropEvent = new("Melee", "Drop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Melee", "EnterWater");
    private static readonly ClassEvent<HitHandler> HitEvent = new("Melee", "Hit");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Melee", "Interact");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Melee", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Melee", "NetworkAuthorityChange");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("Melee", "PickUp");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("Melee", "PullUse");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("Melee", "ReleaseUse");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Melee", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Melee", "ValueChange");
    private static readonly MethodSlot AddAnimationCharacterUseSlot = new("AddAnimationCharacterUse");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot ClearAnimationsCharacterUseSlot = new("ClearAnimationsCharacterUse");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetAnimationsCharacterUseSlot = new("GetAnimationsCharacterUse");
    private static readonly MethodSlot GetBaseDamageSlot = new("GetBaseDamage");
    private static readonly MethodSlot GetCooldownSlot = new("GetCooldown");
    private static readonly MethodSlot GetHandlerSlot = new("GetHandler");
    private static readonly MethodSlot GetHandlingModeSlot = new("GetHandlingMode");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetSoundUseSlot = new("GetSoundUse");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAttachmentSettingsSlot = new("SetAttachmentSettings");
    private static readonly MethodSlot SetBaseDamageSlot = new("SetBaseDamage");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetCanUseSlot = new("SetCanUse");
    private static readonly MethodSlot SetCooldownSlot = new("SetCooldown");
    private static readonly MethodSlot SetCrosshairMaterialSlot = new("SetCrosshairMaterial");
    private static readonly MethodSlot SetDamageSettingsSlot = new("SetDamageSettings");
    private static readonly MethodSlot SetImpactSoundSlot = new("SetImpactSound");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPickableSlot = new("SetPickable");
    private static readonly MethodSlot SetSoundUseSlot = new("SetSoundUse");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets the Animations when Character uses it</summary>
    public IReadOnlyList<string> AnimationsCharacterUse => GetAnimationsCharacterUse();

    /// <summary>Gets the Base Damage</summary>
    public uint BaseDamage
    {
        get => GetBaseDamage();
        set => SetBaseDamage(value);
    }

    /// <summary>Gets the Cooldown between usages</summary>
    public float Cooldown
    {
        get => GetCooldown();
        set => SetCooldown(value);
    }

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? Handler => GetHandler();

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode HandlingMode => GetHandlingMode();

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets the Sound when Character uses it</summary>
    public string SoundUse
    {
        get => GetSoundUse();
        set => SetSoundUse(value);
    }

    /// <summary>Spawns a new Melee.</summary>
    public Melee(
        Vector location,
        Rotator rotation,
        string asset,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        HandlingMode? handlingMode = null,
        string? crosshairMaterial = null,
        bool? canUse = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Melee", ScriptArguments.Trim([location, rotation, asset, collisionType, gravityEnabled, handlingMode, crosshairMaterial, canUse, spawnMode], 3));
    }

    protected Melee()
    {
    }

    /// <summary>Sets an Animation when attacking</summary>
    public void AddAnimationCharacterUse(
        string assetPath,
        float? playRate = null,
        AnimationSlotType? slotType = null) =>
        Invoke(AddAnimationCharacterUseSlot, ScriptArguments.Trim([assetPath, playRate, slotType], 1));

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

    /// <summary>Clears the Character Attack Animation list</summary>
    public void ClearAnimationsCharacterUse() => Invoke(ClearAnimationsCharacterUseSlot);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot)!;

    /// <summary>Gets the Animations when Character uses it</summary>
    public IReadOnlyList<string> GetAnimationsCharacterUse() =>
        Invoke<IReadOnlyList<string>>(GetAnimationsCharacterUseSlot)!;

    /// <summary>Gets the Base Damage</summary>
    public uint GetBaseDamage() => Invoke<uint>(GetBaseDamageSlot)!;

    /// <summary>Gets the Cooldown between usages</summary>
    public float GetCooldown() => Invoke<float>(GetCooldownSlot)!;

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

    /// <summary>Gets the Sound when Character uses it</summary>
    public string GetSoundUse() => Invoke<string>(GetSoundUseSlot)!;

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

    /// <summary>Sets the Base Damage</summary>
    public void SetBaseDamage(uint damage) => Invoke(SetBaseDamageSlot, damage);

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

    /// <summary>Sets the cooldown between attacking</summary>
    public void SetCooldown(float cooldown) => Invoke(SetCooldownSlot, cooldown);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    public void SetCrosshairMaterial(string materialAsset) => Invoke(SetCrosshairMaterialSlot, materialAsset);

    /// <summary>Sets the times when to start applying damage and when to end</summary>
    public void SetDamageSettings(float damageStartTime, float damageDurationTime) =>
        Invoke(SetDamageSettingsSlot, damageStartTime, damageDurationTime);

    /// <summary>Sets the Sound when hitting something</summary>
    public void SetImpactSound(SurfaceType surfaceType, string assetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetImpactSoundSlot, ScriptArguments.Trim([surfaceType, assetPath, volume, pitch], 2));

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

    /// <summary>Sets the Sound when attacking</summary>
    public void SetSoundUse(string assetPath, float? argument2 = null, float? argument3 = null) =>
        Invoke(SetSoundUseSlot, ScriptArguments.Trim([assetPath, argument2, argument3], 1));

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke(SetStaticMeshAttachedTransformSlot, id, relativeLocation, relativeRotation);

    /// <summary>Subscribes to the Attack event of this entity only.</summary>
    public ScriptSubscription SubscribeAttack(AttackHandler handler) => Subscribe("Attack", handler);

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

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when the Character effectively attacks with this Melee</summary>
    public delegate void AttackHandler(Melee self, Character? handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Melee self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Melee self, int oldDimension, int newDimension);

    /// <summary>When a Character drops this Pickable</summary>
    public delegate void DropHandler(Melee self, Character? character, bool wasTriggeredByPlayer);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Melee self);

    /// <summary>When this Pickable hits something</summary>
    public delegate void HitHandler(Melee self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Melee self, Character? character);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Melee self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Melee self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered When a Character picks this up</summary>
    public delegate void PickUpHandler(Melee self, Character? character);

    /// <summary>Triggered when a Character presses the use button for this Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public delegate void PullUseHandler(Melee self, Character? character);

    /// <summary>Triggered when a Character releases the use button for this Pickable (i.e. releases left mouse button with this equipped)</summary>
    public delegate void ReleaseUseHandler(Melee self, Character? character);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Melee self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Melee self, string? key, object? value);
}
