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

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
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
    private static readonly ClassEvent<DropHandler> DropEvent = new("Melee", "Drop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Melee", "EnterWater");
    private static readonly ClassEvent<HitHandler> HitEvent = new("Melee", "Hit");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Melee", "LeaveWater");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("Melee", "PickUp");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("Melee", "PullUse");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("Melee", "ReleaseUse");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Melee", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Melee", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetAnimationsCharacterUseSlot = new("GetAnimationsCharacterUse");
    private static readonly MethodSlot GetAssetNameSlot = new("GetAssetName");
    private static readonly MethodSlot GetBaseDamageSlot = new("GetBaseDamage");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetCooldownSlot = new("GetCooldown");
    private static readonly MethodSlot GetHandlerSlot = new("GetHandler");
    private static readonly MethodSlot GetHandlingModeSlot = new("GetHandlingMode");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetSoundUseSlot = new("GetSoundUse");
    private static readonly MethodSlot HideBoneSlot = new("HideBone");
    private static readonly MethodSlot IsBoneHiddenSlot = new("IsBoneHidden");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAttachmentSettingsSlot = new("SetAttachmentSettings");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetCanUseSlot = new("SetCanUse");
    private static readonly MethodSlot SetCooldownSlot = new("SetCooldown");
    private static readonly MethodSlot SetCrosshairMaterialSlot = new("SetCrosshairMaterial");
    private static readonly MethodSlot SetImpactSoundSlot = new("SetImpactSound");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetSoundUseSlot = new("SetSoundUse");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot UnHideBoneSlot = new("UnHideBone");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets the Animations when Character uses it</summary>
    public IReadOnlyList<string> AnimationsCharacterUse => GetAnimationsCharacterUse();

    /// <summary>Gets the asset path the entity was spawned with</summary>
    public string AssetName => GetAssetName();

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

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

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

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot, 0)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot, 0)!;

    /// <summary>Gets the Animations when Character uses it</summary>
    public IReadOnlyList<string> GetAnimationsCharacterUse() =>
        CallResult<IReadOnlyList<string>>(GetAnimationsCharacterUseSlot, 0)!;

    /// <summary>Gets the asset path the entity was spawned with</summary>
    public string GetAssetName() => CallResult<string>(GetAssetNameSlot, 0)!;

    /// <summary>Gets the Base Damage</summary>
    public uint GetBaseDamage() => CallResult<uint>(GetBaseDamageSlot, 0)!;

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets the Cooldown between usages</summary>
    public float GetCooldown() => CallResult<float>(GetCooldownSlot, 0)!;

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? GetHandler() => CallResult<Character?>(GetHandlerSlot, 0);

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode GetHandlingMode() => CallResult<HandlingMode>(GetHandlingModeSlot, 0)!;

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

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    public string GetMesh() => CallResult<string>(GetMeshSlot, 0)!;

    /// <summary>Gets the Sound when Character uses it</summary>
    public string GetSoundUse() => CallResult<string>(GetSoundUseSlot, 0)!;

    /// <summary>Hides a bone of this Pawn</summary>
    public void HideBone(string boneName) => Call(HideBoneSlot, 1, boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => CallResult<bool, string>(IsBoneHiddenSlot, 1, boneName)!;

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Call(RemoveAllSkeletalMeshesAttachedSlot, 0);

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Call(RemoveAllStaticMeshesAttachedSlot, 0);

    /// <summary>Removes, if it exists, a SkeletalMesh from this Pickable given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Call(RemoveSkeletalMeshAttachedSlot, 1, id);

    /// <summary>Removes, if it exists, a StaticMesh from this Pickable given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Call(RemoveStaticMeshAttachedSlot, 1, id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Call(ResetMaterialSlot, 0, index, attachableId);

    /// <summary>Sets the Attachment Settings for this Pickable (how it attaches to the Character when Picking up)</summary>
    public void SetAttachmentSettings(
        Vector relativeLocation,
        Rotator? relativeRotation = null,
        string? socket = null) =>
        Call(SetAttachmentSettingsSlot, 1, relativeLocation, relativeRotation, socket);

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

    /// <summary>Sets if this Pickable can be used by the player when held</summary>
    public void SetCanUse(bool canUse) => Call(SetCanUseSlot, 1, canUse);

    /// <summary>Sets the cooldown between attacking</summary>
    public void SetCooldown(float cooldown) => Call(SetCooldownSlot, 1, cooldown);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    public void SetCrosshairMaterial(string materialAsset) => Call(SetCrosshairMaterialSlot, 1, materialAsset);

    /// <summary>Sets the Sound when hitting something</summary>
    public void SetImpactSound(SurfaceType surfaceType, string assetPath, float? volume = null, float? pitch = null) =>
        Call(SetImpactSoundSlot, 2, surfaceType, assetPath, volume, pitch);

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

    /// <summary>Sets the Sound when attacking</summary>
    public void SetSoundUse(string assetPath, float? argument2 = null, float? argument3 = null) =>
        Call(SetSoundUseSlot, 1, assetPath, argument2, argument3);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Call(SetStaticMeshAttachedTransformSlot, 3, id, relativeLocation, relativeRotation);

    /// <summary>Unhides a bone of this Pawn</summary>
    public void UnHideBone(string boneName) => Call(UnHideBoneSlot, 1, boneName);

    /// <summary>Subscribes to the Attack event of this entity only.</summary>
    public ScriptSubscription SubscribeAttack(AttackHandler handler) => Subscribe("Attack", handler);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the Drop event of this entity only.</summary>
    public ScriptSubscription SubscribeDrop(DropHandler handler) => Subscribe("Drop", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the Hit event of this entity only.</summary>
    public ScriptSubscription SubscribeHit(HitHandler handler) => Subscribe("Hit", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

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

    /// <summary>Called for the Attack event of this entity when a subclass overrides it.</summary>
    [EventHook("Attack")]
    protected virtual void HandleAttack(Character? handler)
    {
    }

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the Drop event of this entity when a subclass overrides it.</summary>
    [EventHook("Drop")]
    protected virtual void HandleDrop(Character? character, bool wasTriggeredByPlayer)
    {
    }

    /// <summary>Called for the EnterWater event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterWater")]
    protected virtual void HandleEnterWater()
    {
    }

    /// <summary>Called for the Hit event of this entity when a subclass overrides it.</summary>
    [EventHook("Hit")]
    protected virtual void HandleHit(float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor)
    {
    }

    /// <summary>Called for the LeaveWater event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveWater")]
    protected virtual void HandleLeaveWater()
    {
    }

    /// <summary>Called for the PickUp event of this entity when a subclass overrides it.</summary>
    [EventHook("PickUp")]
    protected virtual void HandlePickUp(Character? character)
    {
    }

    /// <summary>Called for the PullUse event of this entity when a subclass overrides it.</summary>
    [EventHook("PullUse")]
    protected virtual void HandlePullUse(Character? character)
    {
    }

    /// <summary>Called for the ReleaseUse event of this entity when a subclass overrides it.</summary>
    [EventHook("ReleaseUse")]
    protected virtual void HandleReleaseUse(Character? character)
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

    /// <summary>Triggered when the Character effectively attacks with this Melee</summary>
    public delegate void AttackHandler(Melee self, Character? handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Melee self);

    /// <summary>When a Character drops this Pickable</summary>
    public delegate void DropHandler(Melee self, Character? character, bool wasTriggeredByPlayer);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Melee self);

    /// <summary>When this Pickable hits something</summary>
    public delegate void HitHandler(Melee self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Melee self);

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
