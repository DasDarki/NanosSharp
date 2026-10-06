#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Weapons are entities with firing, reloading and aiming functionalities.They are fully customizable, all pieces of the weapon can be changed with immense possibility of creation.</summary>
[ScriptClass("Weapon")]
public partial class Weapon : Actor, IPaintable, IPickable
{
    /// <summary>When the Ammo Bag is changed, by reloading or manually setting through scripting</summary>
    public static event AmmoBagChangeHandler? OnAmmoBagChange
    {
        add => AmmoBagChangeEvent.Add(value);
        remove => AmmoBagChangeEvent.Remove(value);
    }

    /// <summary>When the Ammo Clip is changed, by reloading or manually setting through scripting</summary>
    public static event AmmoClipChangeHandler? OnAmmoClipChange
    {
        add => AmmoClipChangeEvent.Add(value);
        remove => AmmoClipChangeEvent.Remove(value);
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

    /// <summary>When a Weapon is reloaded, optionally by a Character</summary>
    public static event ReloadHandler? OnReload
    {
        add => ReloadEvent.Add(value);
        remove => ReloadEvent.Remove(value);
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

    private static readonly ClassEvent<AmmoBagChangeHandler> AmmoBagChangeEvent = new("Weapon", "AmmoBagChange");
    private static readonly ClassEvent<AmmoClipChangeHandler> AmmoClipChangeEvent = new("Weapon", "AmmoClipChange");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Weapon", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Weapon", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Weapon", "DimensionChange");
    private static readonly ClassEvent<DropHandler> DropEvent = new("Weapon", "Drop");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Weapon", "EnterWater");
    private static readonly ClassEvent<HitHandler> HitEvent = new("Weapon", "Hit");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Weapon", "Interact");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Weapon", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Weapon", "NetworkAuthorityChange");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("Weapon", "PickUp");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("Weapon", "PullUse");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("Weapon", "ReleaseUse");
    private static readonly ClassEvent<ReloadHandler> ReloadEvent = new("Weapon", "Reload");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Weapon", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Weapon", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetAmmoBagSlot = new("GetAmmoBag");
    private static readonly MethodSlot GetAmmoClipSlot = new("GetAmmoClip");
    private static readonly MethodSlot GetAmmoToReloadSlot = new("GetAmmoToReload");
    private static readonly MethodSlot GetAnimationCharacterFireSlot = new("GetAnimationCharacterFire");
    private static readonly MethodSlot GetAnimationFireSlot = new("GetAnimationFire");
    private static readonly MethodSlot GetAnimationReloadSlot = new("GetAnimationReload");
    private static readonly MethodSlot GetBulletColorSlot = new("GetBulletColor");
    private static readonly MethodSlot GetBulletCountSlot = new("GetBulletCount");
    private static readonly MethodSlot GetCadenceSlot = new("GetCadence");
    private static readonly MethodSlot GetCanHoldUseSlot = new("GetCanHoldUse");
    private static readonly MethodSlot GetClipCapacitySlot = new("GetClipCapacity");
    private static readonly MethodSlot GetDamageSlot = new("GetDamage");
    private static readonly MethodSlot GetHandlerSlot = new("GetHandler");
    private static readonly MethodSlot GetHandlingModeSlot = new("GetHandlingMode");
    private static readonly MethodSlot GetHoldReleaseUseSlot = new("GetHoldReleaseUse");
    private static readonly MethodSlot GetLeftHandLocationSlot = new("GetLeftHandLocation");
    private static readonly MethodSlot GetLeftHandRotationSlot = new("GetLeftHandRotation");
    private static readonly MethodSlot GetMagazineMeshSlot = new("GetMagazineMesh");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetParticlesBarrelSlot = new("GetParticlesBarrel");
    private static readonly MethodSlot GetParticlesBulletTrailSlot = new("GetParticlesBulletTrail");
    private static readonly MethodSlot GetParticlesShellsSlot = new("GetParticlesShells");
    private static readonly MethodSlot GetRecoilSlot = new("GetRecoil");
    private static readonly MethodSlot GetRightHandOffsetSlot = new("GetRightHandOffset");
    private static readonly MethodSlot GetSightFOVMultiplierSlot = new("GetSightFOVMultiplier");
    private static readonly MethodSlot GetSightLocationSlot = new("GetSightLocation");
    private static readonly MethodSlot GetSightRotationSlot = new("GetSightRotation");
    private static readonly MethodSlot GetSoundAimSlot = new("GetSoundAim");
    private static readonly MethodSlot GetSoundDrySlot = new("GetSoundDry");
    private static readonly MethodSlot GetSoundFireSlot = new("GetSoundFire");
    private static readonly MethodSlot GetSoundLoadSlot = new("GetSoundLoad");
    private static readonly MethodSlot GetSoundUnloadSlot = new("GetSoundUnload");
    private static readonly MethodSlot GetSoundZoomingSlot = new("GetSoundZooming");
    private static readonly MethodSlot GetSpreadSlot = new("GetSpread");
    private static readonly MethodSlot GetWallbangDamageMultiplierSlot = new("GetWallbangDamageMultiplier");
    private static readonly MethodSlot GetWallbangMaxDistanceSlot = new("GetWallbangMaxDistance");
    private static readonly MethodSlot PlayAnimationSlot = new("PlayAnimation");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot ReloadSlot = new("Reload");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetAmmoBagSlot = new("SetAmmoBag");
    private static readonly MethodSlot SetAmmoClipSlot = new("SetAmmoClip");
    private static readonly MethodSlot SetAmmoSettingsSlot = new("SetAmmoSettings");
    private static readonly MethodSlot SetAnimationCharacterFireSlot = new("SetAnimationCharacterFire");
    private static readonly MethodSlot SetAnimationFireSlot = new("SetAnimationFire");
    private static readonly MethodSlot SetAnimationReloadSlot = new("SetAnimationReload");
    private static readonly MethodSlot SetAttachmentSettingsSlot = new("SetAttachmentSettings");
    private static readonly MethodSlot SetAutoReloadSlot = new("SetAutoReload");
    private static readonly MethodSlot SetBulletColorSlot = new("SetBulletColor");
    private static readonly MethodSlot SetBulletSettingsSlot = new("SetBulletSettings");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetCadenceSlot = new("SetCadence");
    private static readonly MethodSlot SetCanUseSlot = new("SetCanUse");
    private static readonly MethodSlot SetClipCapacitySlot = new("SetClipCapacity");
    private static readonly MethodSlot SetCrosshairMaterialSlot = new("SetCrosshairMaterial");
    private static readonly MethodSlot SetDamageSlot = new("SetDamage");
    private static readonly MethodSlot SetHandlingModeSlot = new("SetHandlingMode");
    private static readonly MethodSlot SetLeftHandTransformSlot = new("SetLeftHandTransform");
    private static readonly MethodSlot SetMagazineMeshSlot = new("SetMagazineMesh");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetParticlesBarrelSlot = new("SetParticlesBarrel");
    private static readonly MethodSlot SetParticlesBulletTrailSlot = new("SetParticlesBulletTrail");
    private static readonly MethodSlot SetParticlesShellsSlot = new("SetParticlesShells");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPickableSlot = new("SetPickable");
    private static readonly MethodSlot SetRecoilSlot = new("SetRecoil");
    private static readonly MethodSlot SetRightHandOffsetSlot = new("SetRightHandOffset");
    private static readonly MethodSlot SetSightFOVMultiplierSlot = new("SetSightFOVMultiplier");
    private static readonly MethodSlot SetSightTransformSlot = new("SetSightTransform");
    private static readonly MethodSlot SetSoundAimSlot = new("SetSoundAim");
    private static readonly MethodSlot SetSoundDrySlot = new("SetSoundDry");
    private static readonly MethodSlot SetSoundFireSlot = new("SetSoundFire");
    private static readonly MethodSlot SetSoundFireLastBulletsSlot = new("SetSoundFireLastBullets");
    private static readonly MethodSlot SetSoundLoadSlot = new("SetSoundLoad");
    private static readonly MethodSlot SetSoundUnloadSlot = new("SetSoundUnload");
    private static readonly MethodSlot SetSoundZoomingSlot = new("SetSoundZooming");
    private static readonly MethodSlot SetSpreadSlot = new("SetSpread");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot SetUsageSettingsSlot = new("SetUsageSettings");
    private static readonly MethodSlot SetWallbangSettingsSlot = new("SetWallbangSettings");
    private static readonly MethodSlot StopAnimationSlot = new("StopAnimation");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets this Weapon's Ammo Bag</summary>
    public int AmmoBag
    {
        get => GetAmmoBag();
        set => SetAmmoBag(value);
    }

    /// <summary>Gets this Weapon's Ammo Clip</summary>
    public int AmmoClip
    {
        get => GetAmmoClip();
        set => SetAmmoClip(value);
    }

    /// <summary>Gets the amount of ammo needed to fully reload</summary>
    public int AmmoToReload => GetAmmoToReload();

    /// <summary>Gets the Animation played by the Character when firing</summary>
    public string AnimationCharacterFire
    {
        get => GetAnimationCharacterFire();
        set => SetAnimationCharacterFire(value);
    }

    /// <summary>Gets the Animation played by the Weapon when firing</summary>
    public string AnimationFire
    {
        get => GetAnimationFire();
        set => SetAnimationFire(value);
    }

    /// <summary>Gets the reload animation</summary>
    public string AnimationReload
    {
        get => GetAnimationReload();
        set => SetAnimationReload(value);
    }

    /// <summary>Gets the color of the bullet trail</summary>
    public Color BulletColor
    {
        get => GetBulletColor();
        set => SetBulletColor(value);
    }

    /// <summary>Gets how many bullets are fired per shot</summary>
    public int BulletCount => GetBulletCount();

    /// <summary>Gets the time between shots</summary>
    public float Cadence
    {
        get => GetCadence();
        set => SetCadence(value);
    }

    /// <summary>Gets if the weapon can be used continuously by holding the use button</summary>
    public bool CanHoldUse => GetCanHoldUse();

    /// <summary>Gets the max number of bullets the clip can hold</summary>
    public int ClipCapacity
    {
        get => GetClipCapacity();
        set => SetClipCapacity(value);
    }

    /// <summary>Gets the base damage of this Weapon</summary>
    public int Damage
    {
        get => GetDamage();
        set => SetDamage(value);
    }

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? Handler => GetHandler();

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode HandlingMode
    {
        get => GetHandlingMode();
        set => SetHandlingMode(value);
    }

    /// <summary>Gets if the weapon is triggered/fired when releasing the use button</summary>
    public bool HoldReleaseUse => GetHoldReleaseUse();

    /// <summary>Gets the location offset of the left hand</summary>
    public Vector LeftHandLocation => GetLeftHandLocation();

    /// <summary>Gets the rotation offset of the left hand</summary>
    public Rotator LeftHandRotation => GetLeftHandRotation();

    /// <summary>Gets the magazine Static Mesh used when reloading</summary>
    public string MagazineMesh
    {
        get => GetMagazineMesh();
        set => SetMagazineMesh(value);
    }

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets the Particle of the Fire Blast in the muzzle</summary>
    public string ParticlesBarrel
    {
        get => GetParticlesBarrel();
        set => SetParticlesBarrel(value);
    }

    /// <summary>Gets the Particle of the bullet trail</summary>
    public string ParticlesBulletTrail
    {
        get => GetParticlesBulletTrail();
        set => SetParticlesBulletTrail(value);
    }

    /// <summary>Gets the Particle of the ejected shells</summary>
    public string ParticlesShells
    {
        get => GetParticlesShells();
        set => SetParticlesShells(value);
    }

    /// <summary>Gets the base recoil of this Weapon</summary>
    public float Recoil
    {
        get => GetRecoil();
        set => SetRecoil(value);
    }

    /// <summary>Gets the offset of the right hand, relative to the camera</summary>
    public Vector RightHandOffset
    {
        get => GetRightHandOffset();
        set => SetRightHandOffset(value);
    }

    /// <summary>Gets the Field of View multiplier applied when aiming down sights</summary>
    public float SightFOVMultiplier
    {
        get => GetSightFOVMultiplier();
        set => SetSightFOVMultiplier(value);
    }

    /// <summary>Gets the sight location offset used when aiming down sights</summary>
    public Vector SightLocation => GetSightLocation();

    /// <summary>Gets the sight rotation used when aiming down sights</summary>
    public Rotator SightRotation => GetSightRotation();

    /// <summary>Gets the Sound played when aiming</summary>
    public string SoundAim
    {
        get => GetSoundAim();
        set => SetSoundAim(value);
    }

    /// <summary>Gets the Sound played when firing with an empty clip</summary>
    public string SoundDry
    {
        get => GetSoundDry();
        set => SetSoundDry(value);
    }

    /// <summary>Gets the Sound played when firing</summary>
    public string SoundFire
    {
        get => GetSoundFire();
        set => SetSoundFire(value);
    }

    /// <summary>Gets the Sound played when loading a magazine</summary>
    public string SoundLoad
    {
        get => GetSoundLoad();
        set => SetSoundLoad(value);
    }

    /// <summary>Gets the Sound played when unloading a magazine</summary>
    public string SoundUnload
    {
        get => GetSoundUnload();
        set => SetSoundUnload(value);
    }

    /// <summary>Gets the Sound played when zooming</summary>
    public string SoundZooming
    {
        get => GetSoundZooming();
        set => SetSoundZooming(value);
    }

    /// <summary>Gets the base spread of this Weapon</summary>
    public float Spread
    {
        get => GetSpread();
        set => SetSpread(value);
    }

    /// <summary>Gets the damage multiplier applied when a bullet passes through a wall</summary>
    public float WallbangDamageMultiplier => GetWallbangDamageMultiplier();

    /// <summary>Gets the max distance a bullet can pass through walls</summary>
    public float WallbangMaxDistance => GetWallbangMaxDistance();

    /// <summary>Spawns a new Weapon.</summary>
    public Weapon(
        Vector location,
        Rotator rotation,
        string asset,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Weapon", ScriptArguments.Trim([location, rotation, asset, collisionType, gravityEnabled, spawnMode], 3));
    }

    protected Weapon()
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

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot, 0)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot, 0)!;

    /// <summary>Gets this Weapon's Ammo Bag</summary>
    public int GetAmmoBag() => CallResult<int>(GetAmmoBagSlot, 0)!;

    /// <summary>Gets this Weapon's Ammo Clip</summary>
    public int GetAmmoClip() => CallResult<int>(GetAmmoClipSlot, 0)!;

    /// <summary>Gets the amount of ammo needed to fully reload</summary>
    public int GetAmmoToReload() => CallResult<int>(GetAmmoToReloadSlot, 0)!;

    /// <summary>Gets the Animation played by the Character when firing</summary>
    public string GetAnimationCharacterFire() => CallResult<string>(GetAnimationCharacterFireSlot, 0)!;

    /// <summary>Gets the Animation played by the Weapon when firing</summary>
    public string GetAnimationFire() => CallResult<string>(GetAnimationFireSlot, 0)!;

    /// <summary>Gets the reload animation</summary>
    public string GetAnimationReload() => CallResult<string>(GetAnimationReloadSlot, 0)!;

    /// <summary>Gets the color of the bullet trail</summary>
    public Color GetBulletColor() => CallResult<Color>(GetBulletColorSlot, 0)!;

    /// <summary>Gets how many bullets are fired per shot</summary>
    public int GetBulletCount() => CallResult<int>(GetBulletCountSlot, 0)!;

    /// <summary>Gets the time between shots</summary>
    public float GetCadence() => CallResult<float>(GetCadenceSlot, 0)!;

    /// <summary>Gets if the weapon can be used continuously by holding the use button</summary>
    public bool GetCanHoldUse() => CallResult<bool>(GetCanHoldUseSlot, 0)!;

    /// <summary>Gets the max number of bullets the clip can hold</summary>
    public int GetClipCapacity() => CallResult<int>(GetClipCapacitySlot, 0)!;

    /// <summary>Gets the base damage of this Weapon</summary>
    public int GetDamage() => CallResult<int>(GetDamageSlot, 0)!;

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? GetHandler() => CallResult<Character?>(GetHandlerSlot, 0);

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode GetHandlingMode() => CallResult<HandlingMode>(GetHandlingModeSlot, 0)!;

    /// <summary>Gets if the weapon is triggered/fired when releasing the use button</summary>
    public bool GetHoldReleaseUse() => CallResult<bool>(GetHoldReleaseUseSlot, 0)!;

    /// <summary>Gets the location offset of the left hand</summary>
    public Vector GetLeftHandLocation() => CallResult<Vector>(GetLeftHandLocationSlot, 0)!;

    /// <summary>Gets the rotation offset of the left hand</summary>
    public Rotator GetLeftHandRotation() => CallResult<Rotator>(GetLeftHandRotationSlot, 0)!;

    /// <summary>Gets the magazine Static Mesh used when reloading</summary>
    public string GetMagazineMesh() => CallResult<string>(GetMagazineMeshSlot, 0)!;

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

    /// <summary>Gets the Particle of the Fire Blast in the muzzle</summary>
    public string GetParticlesBarrel() => CallResult<string>(GetParticlesBarrelSlot, 0)!;

    /// <summary>Gets the Particle of the bullet trail</summary>
    public string GetParticlesBulletTrail() => CallResult<string>(GetParticlesBulletTrailSlot, 0)!;

    /// <summary>Gets the Particle of the ejected shells</summary>
    public string GetParticlesShells() => CallResult<string>(GetParticlesShellsSlot, 0)!;

    /// <summary>Gets the base recoil of this Weapon</summary>
    public float GetRecoil() => CallResult<float>(GetRecoilSlot, 0)!;

    /// <summary>Gets the offset of the right hand, relative to the camera</summary>
    public Vector GetRightHandOffset() => CallResult<Vector>(GetRightHandOffsetSlot, 0)!;

    /// <summary>Gets the Field of View multiplier applied when aiming down sights</summary>
    public float GetSightFOVMultiplier() => CallResult<float>(GetSightFOVMultiplierSlot, 0)!;

    /// <summary>Gets the sight location offset used when aiming down sights</summary>
    public Vector GetSightLocation() => CallResult<Vector>(GetSightLocationSlot, 0)!;

    /// <summary>Gets the sight rotation used when aiming down sights</summary>
    public Rotator GetSightRotation() => CallResult<Rotator>(GetSightRotationSlot, 0)!;

    /// <summary>Gets the Sound played when aiming</summary>
    public string GetSoundAim() => CallResult<string>(GetSoundAimSlot, 0)!;

    /// <summary>Gets the Sound played when firing with an empty clip</summary>
    public string GetSoundDry() => CallResult<string>(GetSoundDrySlot, 0)!;

    /// <summary>Gets the Sound played when firing</summary>
    public string GetSoundFire() => CallResult<string>(GetSoundFireSlot, 0)!;

    /// <summary>Gets the Sound played when loading a magazine</summary>
    public string GetSoundLoad() => CallResult<string>(GetSoundLoadSlot, 0)!;

    /// <summary>Gets the Sound played when unloading a magazine</summary>
    public string GetSoundUnload() => CallResult<string>(GetSoundUnloadSlot, 0)!;

    /// <summary>Gets the Sound played when zooming</summary>
    public string GetSoundZooming() => CallResult<string>(GetSoundZoomingSlot, 0)!;

    /// <summary>Gets the base spread of this Weapon</summary>
    public float GetSpread() => CallResult<float>(GetSpreadSlot, 0)!;

    /// <summary>Gets the damage multiplier applied when a bullet passes through a wall</summary>
    public float GetWallbangDamageMultiplier() => CallResult<float>(GetWallbangDamageMultiplierSlot, 0)!;

    /// <summary>Gets the max distance a bullet can pass through walls</summary>
    public float GetWallbangMaxDistance() => CallResult<float>(GetWallbangMaxDistanceSlot, 0)!;

    /// <summary>Plays an Animation on this Weapon</summary>
    public void PlayAnimation(
        string animationPath,
        string? slotName = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Call(PlayAnimationSlot, 1, animationPath, slotName, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages);

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) => Call(PullUseSlot, 0, releaseUseAfter);

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Call(ReleaseUseSlot, 0);

    /// <summary>Forces this Weapon to reload</summary>
    public void Reload() => Call(ReloadSlot, 0);

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

    /// <summary>Sets this Weapon's Ammo in the Bag</summary>
    public void SetAmmoBag(int newAmmoBag) => Call(SetAmmoBagSlot, 1, newAmmoBag);

    /// <summary>Sets this Weapon's Ammo in the Clip</summary>
    public void SetAmmoClip(int newAmmoClip) => Call(SetAmmoClipSlot, 1, newAmmoClip);

    /// <summary>Sets the ammo counts and the clip capacity at once</summary>
    public void SetAmmoSettings(uint ammoClip, uint ammoBag, uint? ammoToReload = null, uint? clipCapacity = null) =>
        Call(SetAmmoSettingsSlot, 2, ammoClip, ammoBag, ammoToReload, clipCapacity);

    /// <summary>Sets the Animation played by the Character when firing this Weapon</summary>
    public void SetAnimationCharacterFire(string animationCharacterFirePath, float? playRate = null) =>
        Call(SetAnimationCharacterFireSlot, 1, animationCharacterFirePath, playRate);

    /// <summary>Sets the Animation played by the Weapon itself when firing</summary>
    public void SetAnimationFire(string animationFirePath, float? playRate = null) =>
        Call(SetAnimationFireSlot, 1, animationFirePath, playRate);

    /// <summary>Sets the Animation played by the Character when reloading this Weapon</summary>
    public void SetAnimationReload(string reloadAnimationPath, float? playRate = null) =>
        Call(SetAnimationReloadSlot, 1, reloadAnimationPath, playRate);

    /// <summary>Sets the Attachment Settings for this Pickable (how it attaches to the Character when Picking up)</summary>
    public void SetAttachmentSettings(
        Vector relativeLocation,
        Rotator? relativeRotation = null,
        string? socket = null) =>
        Call(SetAttachmentSettingsSlot, 1, relativeLocation, relativeRotation, socket);

    /// <summary>Sets if the Character will reload automatically when the clip is empty</summary>
    public void SetAutoReload(bool autoReload) => Call(SetAutoReloadSlot, 1, autoReload);

    /// <summary>Set the Bullet Color</summary>
    public void SetBulletColor(Color bulletColor) => Call(SetBulletColorSlot, 1, bulletColor);

    /// <summary>Sets how many bullets are fired per shot and how they look and travel</summary>
    public void SetBulletSettings(uint bulletCount, float bulletMaxDistance, uint bulletVelocity, Color bulletColor) =>
        Call(SetBulletSettingsSlot, 4, bulletCount, bulletMaxDistance, bulletVelocity, bulletColor);

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

    /// <summary>Sets the time between shots</summary>
    public void SetCadence(float cadence) => Call(SetCadenceSlot, 1, cadence);

    /// <summary>Sets if this Pickable can be used by the player when held</summary>
    public void SetCanUse(bool canUse) => Call(SetCanUseSlot, 1, canUse);

    /// <summary>Sets the max number of bullets the Weapon's clip can hold</summary>
    public void SetClipCapacity(int clip) => Call(SetClipCapacitySlot, 1, clip);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    public void SetCrosshairMaterial(string materialAsset) => Call(SetCrosshairMaterialSlot, 1, materialAsset);

    /// <summary>Set the Base Weapon's Damage</summary>
    public void SetDamage(int damage) => Call(SetDamageSlot, 1, damage);

    /// <summary>Sets how the Character grabs this Weapon</summary>
    public void SetHandlingMode(HandlingMode handlingMode) => Call(SetHandlingModeSlot, 1, handlingMode);

    /// <summary>Sets where the Character's left hand holds this Weapon, relative to the Weapon</summary>
    public void SetLeftHandTransform(Vector leftHandLocation, Rotator leftHandRotation) =>
        Call(SetLeftHandTransformSlot, 2, leftHandLocation, leftHandRotation);

    /// <summary>Set the mesh used when the Character reloads the weapon</summary>
    public void SetMagazineMesh(string magazineMesh, string? magazineMeshHideBone = null) =>
        Call(SetMagazineMeshSlot, 1, magazineMesh, magazineMeshHideBone);

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

    /// <summary>Sets the Particle of the fire blast at the muzzle</summary>
    public void SetParticlesBarrel(string particleAssetPath) => Call(SetParticlesBarrelSlot, 1, particleAssetPath);

    /// <summary>Sets the Particle of the bullet trail</summary>
    public void SetParticlesBulletTrail(string particleBulletTrailAssetPath) =>
        Call(SetParticlesBulletTrailSlot, 1, particleBulletTrailAssetPath);

    /// <summary>Sets the Particle of the empty shells ejected when firing</summary>
    public void SetParticlesShells(string particleShellsAssetPath) =>
        Call(SetParticlesShellsSlot, 1, particleShellsAssetPath);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Call(SetPhysicalMaterialSlot, 1, physicalMaterialPath);

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    public void SetPickable(bool isPickable) => Call(SetPickableSlot, 1, isPickable);

    /// <summary>Sets the base recoil of this Weapon</summary>
    public void SetRecoil(float recoil) => Call(SetRecoilSlot, 1, recoil);

    /// <summary>Set the offset of Right Hand</summary>
    public void SetRightHandOffset(Vector rightHandOffset) => Call(SetRightHandOffsetSlot, 1, rightHandOffset);

    /// <summary>Sets the Field of View multiplier applied when aiming down sights</summary>
    public void SetSightFOVMultiplier(float sightFovMultiplier) =>
        Call(SetSightFOVMultiplierSlot, 1, sightFovMultiplier);

    /// <summary>Sets the offset that aligns the Player's view with the Weapon's sight when aiming down sights</summary>
    public void SetSightTransform(Vector sightLocation, Rotator sightRotation) =>
        Call(SetSightTransformSlot, 2, sightLocation, sightRotation);

    /// <summary>Sets the Sound played when starting to aim</summary>
    public void SetSoundAim(string soundAimAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundAimSlot, 1, soundAimAssetPath, volume, pitch);

    /// <summary>Sets the Sound played when trying to fire with an empty clip</summary>
    public void SetSoundDry(string soundDryAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundDrySlot, 1, soundDryAssetPath, volume, pitch);

    /// <summary>Sets the Sound played when firing</summary>
    public void SetSoundFire(string soundFireAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundFireSlot, 1, soundFireAssetPath, volume, pitch);

    /// <summary>Sound when firing with only having X remaining bullets in the magazine</summary>
    public void SetSoundFireLastBullets(string soundAssetPath, int? remainingBulletsCount = null) =>
        Call(SetSoundFireLastBulletsSlot, 1, soundAssetPath, remainingBulletsCount);

    /// <summary>Sets the Sound played when loading a magazine</summary>
    public void SetSoundLoad(string soundLoadAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundLoadSlot, 1, soundLoadAssetPath, volume, pitch);

    /// <summary>Sets the Sound played when unloading a magazine</summary>
    public void SetSoundUnload(string soundUnloadAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundUnloadSlot, 1, soundUnloadAssetPath, volume, pitch);

    /// <summary>Sets the Sound played when zooming</summary>
    public void SetSoundZooming(string soundZoomingAssetPath, float? volume = null, float? pitch = null) =>
        Call(SetSoundZoomingSlot, 1, soundZoomingAssetPath, volume, pitch);

    /// <summary>Sets the base spread of this Weapon</summary>
    public void SetSpread(float spread) => Call(SetSpreadSlot, 1, spread);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Call(SetStaticMeshAttachedTransformSlot, 3, id, relativeLocation, relativeRotation);

    /// <summary>Sets if the Weapon can hold to keep firing and if it needs to release to fire</summary>
    public void SetUsageSettings(bool canHoldUse, bool holdReleaseUse) =>
        Call(SetUsageSettingsSlot, 2, canHoldUse, holdReleaseUse);

    /// <summary>Sets how the bullet will pass through walls</summary>
    public void SetWallbangSettings(float maxDistance, float damageMultiplier) =>
        Call(SetWallbangSettingsSlot, 2, maxDistance, damageMultiplier);

    /// <summary>Stops an Animation Montage on this Weapon</summary>
    public void StopAnimation(string? animationAsset = null) => Call(StopAnimationSlot, 0, animationAsset);

    /// <summary>Subscribes to the AmmoBagChange event of this entity only.</summary>
    public ScriptSubscription SubscribeAmmoBagChange(AmmoBagChangeHandler handler) => Subscribe("AmmoBagChange", handler);

    /// <summary>Subscribes to the AmmoClipChange event of this entity only.</summary>
    public ScriptSubscription SubscribeAmmoClipChange(AmmoClipChangeHandler handler) => Subscribe("AmmoClipChange", handler);

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

    /// <summary>Subscribes to the Reload event of this entity only.</summary>
    public ScriptSubscription SubscribeReload(ReloadHandler handler) => Subscribe("Reload", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>When the Ammo Bag is changed, by reloading or manually setting through scripting</summary>
    public delegate void AmmoBagChangeHandler(Weapon self, int oldAmmoBag, int newAmmoBag);

    /// <summary>When the Ammo Clip is changed, by reloading or manually setting through scripting</summary>
    public delegate void AmmoClipChangeHandler(Weapon self, int oldAmmoClip, int newAmmoClip);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Weapon self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Weapon self, int oldDimension, int newDimension);

    /// <summary>When a Character drops this Pickable</summary>
    public delegate void DropHandler(Weapon self, Character? character, bool wasTriggeredByPlayer);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Weapon self);

    /// <summary>When this Pickable hits something</summary>
    public delegate void HitHandler(Weapon self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Weapon self, Character? character);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Weapon self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Weapon self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered When a Character picks this up</summary>
    public delegate void PickUpHandler(Weapon self, Character? character);

    /// <summary>Triggered when a Character presses the use button for this Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public delegate void PullUseHandler(Weapon self, Character? character);

    /// <summary>Triggered when a Character releases the use button for this Pickable (i.e. releases left mouse button with this equipped)</summary>
    public delegate void ReleaseUseHandler(Weapon self, Character? character);

    /// <summary>When a Weapon is reloaded, optionally by a Character</summary>
    public delegate void ReloadHandler(Weapon self, Character? character, int ammoToReload);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Weapon self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Weapon self, string? key, object? value);
}
