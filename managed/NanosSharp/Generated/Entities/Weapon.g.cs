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
        Invoke(AddStaticMeshAttachedSlot, ScriptArguments.Trim([id, staticMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, attachableId], 2));

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot)!;

    /// <summary>Gets this Weapon's Ammo Bag</summary>
    public int GetAmmoBag() => Invoke<int>(GetAmmoBagSlot)!;

    /// <summary>Gets this Weapon's Ammo Clip</summary>
    public int GetAmmoClip() => Invoke<int>(GetAmmoClipSlot)!;

    /// <summary>Gets the amount of ammo needed to fully reload</summary>
    public int GetAmmoToReload() => Invoke<int>(GetAmmoToReloadSlot)!;

    /// <summary>Gets the Animation played by the Character when firing</summary>
    public string GetAnimationCharacterFire() => Invoke<string>(GetAnimationCharacterFireSlot)!;

    /// <summary>Gets the Animation played by the Weapon when firing</summary>
    public string GetAnimationFire() => Invoke<string>(GetAnimationFireSlot)!;

    /// <summary>Gets the reload animation</summary>
    public string GetAnimationReload() => Invoke<string>(GetAnimationReloadSlot)!;

    /// <summary>Gets the color of the bullet trail</summary>
    public Color GetBulletColor() => Invoke<Color>(GetBulletColorSlot)!;

    /// <summary>Gets how many bullets are fired per shot</summary>
    public int GetBulletCount() => Invoke<int>(GetBulletCountSlot)!;

    /// <summary>Gets the time between shots</summary>
    public float GetCadence() => Invoke<float>(GetCadenceSlot)!;

    /// <summary>Gets if the weapon can be used continuously by holding the use button</summary>
    public bool GetCanHoldUse() => Invoke<bool>(GetCanHoldUseSlot)!;

    /// <summary>Gets the max number of bullets the clip can hold</summary>
    public int GetClipCapacity() => Invoke<int>(GetClipCapacitySlot)!;

    /// <summary>Gets the base damage of this Weapon</summary>
    public int GetDamage() => Invoke<int>(GetDamageSlot)!;

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    public Character? GetHandler() => Invoke<Character?>(GetHandlerSlot);

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode GetHandlingMode() => Invoke<HandlingMode>(GetHandlingModeSlot)!;

    /// <summary>Gets if the weapon is triggered/fired when releasing the use button</summary>
    public bool GetHoldReleaseUse() => Invoke<bool>(GetHoldReleaseUseSlot)!;

    /// <summary>Gets the location offset of the left hand</summary>
    public Vector GetLeftHandLocation() => Invoke<Vector>(GetLeftHandLocationSlot)!;

    /// <summary>Gets the rotation offset of the left hand</summary>
    public Rotator GetLeftHandRotation() => Invoke<Rotator>(GetLeftHandRotationSlot)!;

    /// <summary>Gets the magazine Static Mesh used when reloading</summary>
    public string GetMagazineMesh() => Invoke<string>(GetMagazineMeshSlot)!;

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

    /// <summary>Gets the Particle of the Fire Blast in the muzzle</summary>
    public string GetParticlesBarrel() => Invoke<string>(GetParticlesBarrelSlot)!;

    /// <summary>Gets the Particle of the bullet trail</summary>
    public string GetParticlesBulletTrail() => Invoke<string>(GetParticlesBulletTrailSlot)!;

    /// <summary>Gets the Particle of the ejected shells</summary>
    public string GetParticlesShells() => Invoke<string>(GetParticlesShellsSlot)!;

    /// <summary>Gets the base recoil of this Weapon</summary>
    public float GetRecoil() => Invoke<float>(GetRecoilSlot)!;

    /// <summary>Gets the offset of the right hand, relative to the camera</summary>
    public Vector GetRightHandOffset() => Invoke<Vector>(GetRightHandOffsetSlot)!;

    /// <summary>Gets the Field of View multiplier applied when aiming down sights</summary>
    public float GetSightFOVMultiplier() => Invoke<float>(GetSightFOVMultiplierSlot)!;

    /// <summary>Gets the sight location offset used when aiming down sights</summary>
    public Vector GetSightLocation() => Invoke<Vector>(GetSightLocationSlot)!;

    /// <summary>Gets the sight rotation used when aiming down sights</summary>
    public Rotator GetSightRotation() => Invoke<Rotator>(GetSightRotationSlot)!;

    /// <summary>Gets the Sound played when aiming</summary>
    public string GetSoundAim() => Invoke<string>(GetSoundAimSlot)!;

    /// <summary>Gets the Sound played when firing with an empty clip</summary>
    public string GetSoundDry() => Invoke<string>(GetSoundDrySlot)!;

    /// <summary>Gets the Sound played when firing</summary>
    public string GetSoundFire() => Invoke<string>(GetSoundFireSlot)!;

    /// <summary>Gets the Sound played when loading a magazine</summary>
    public string GetSoundLoad() => Invoke<string>(GetSoundLoadSlot)!;

    /// <summary>Gets the Sound played when unloading a magazine</summary>
    public string GetSoundUnload() => Invoke<string>(GetSoundUnloadSlot)!;

    /// <summary>Gets the Sound played when zooming</summary>
    public string GetSoundZooming() => Invoke<string>(GetSoundZoomingSlot)!;

    /// <summary>Gets the base spread of this Weapon</summary>
    public float GetSpread() => Invoke<float>(GetSpreadSlot)!;

    /// <summary>Gets the damage multiplier applied when a bullet passes through a wall</summary>
    public float GetWallbangDamageMultiplier() => Invoke<float>(GetWallbangDamageMultiplierSlot)!;

    /// <summary>Gets the max distance a bullet can pass through walls</summary>
    public float GetWallbangMaxDistance() => Invoke<float>(GetWallbangMaxDistanceSlot)!;

    /// <summary>Plays an Animation on this Weapon</summary>
    public void PlayAnimation(
        string animationPath,
        string? slotName = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Invoke(PlayAnimationSlot, ScriptArguments.Trim([animationPath, slotName, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages], 1));

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) =>
        Invoke(PullUseSlot, ScriptArguments.Trim([releaseUseAfter], 0));

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Invoke(ReleaseUseSlot);

    /// <summary>Forces this Weapon to reload</summary>
    public void Reload() => Invoke(ReloadSlot);

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

    /// <summary>Sets this Weapon's Ammo in the Bag</summary>
    public void SetAmmoBag(int newAmmoBag) => Invoke(SetAmmoBagSlot, newAmmoBag);

    /// <summary>Sets this Weapon's Ammo in the Clip</summary>
    public void SetAmmoClip(int newAmmoClip) => Invoke(SetAmmoClipSlot, newAmmoClip);

    /// <summary>Sets the ammo counts and the clip capacity at once</summary>
    public void SetAmmoSettings(uint ammoClip, uint ammoBag, uint? ammoToReload = null, uint? clipCapacity = null) =>
        Invoke(SetAmmoSettingsSlot, ScriptArguments.Trim([ammoClip, ammoBag, ammoToReload, clipCapacity], 2));

    /// <summary>Sets the Animation played by the Character when firing this Weapon</summary>
    public void SetAnimationCharacterFire(string animationCharacterFirePath, float? playRate = null) =>
        Invoke(SetAnimationCharacterFireSlot, ScriptArguments.Trim([animationCharacterFirePath, playRate], 1));

    /// <summary>Sets the Animation played by the Weapon itself when firing</summary>
    public void SetAnimationFire(string animationFirePath, float? playRate = null) =>
        Invoke(SetAnimationFireSlot, ScriptArguments.Trim([animationFirePath, playRate], 1));

    /// <summary>Sets the Animation played by the Character when reloading this Weapon</summary>
    public void SetAnimationReload(string reloadAnimationPath, float? playRate = null) =>
        Invoke(SetAnimationReloadSlot, ScriptArguments.Trim([reloadAnimationPath, playRate], 1));

    /// <summary>Sets the Attachment Settings for this Pickable (how it attaches to the Character when Picking up)</summary>
    public void SetAttachmentSettings(
        Vector relativeLocation,
        Rotator? relativeRotation = null,
        string? socket = null) =>
        Invoke(SetAttachmentSettingsSlot, ScriptArguments.Trim([relativeLocation, relativeRotation, socket], 1));

    /// <summary>Sets if the Character will reload automatically when the clip is empty</summary>
    public void SetAutoReload(bool autoReload) => Invoke(SetAutoReloadSlot, autoReload);

    /// <summary>Set the Bullet Color</summary>
    public void SetBulletColor(Color bulletColor) => Invoke(SetBulletColorSlot, bulletColor);

    /// <summary>Sets how many bullets are fired per shot and how they look and travel</summary>
    public void SetBulletSettings(uint bulletCount, float bulletMaxDistance, uint bulletVelocity, Color bulletColor) =>
        Invoke(SetBulletSettingsSlot, bulletCount, bulletMaxDistance, bulletVelocity, bulletColor);

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

    /// <summary>Sets the time between shots</summary>
    public void SetCadence(float cadence) => Invoke(SetCadenceSlot, cadence);

    /// <summary>Sets if this Pickable can be used by the player when held</summary>
    public void SetCanUse(bool canUse) => Invoke(SetCanUseSlot, canUse);

    /// <summary>Sets the max number of bullets the Weapon's clip can hold</summary>
    public void SetClipCapacity(int clip) => Invoke(SetClipCapacitySlot, clip);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    public void SetCrosshairMaterial(string materialAsset) => Invoke(SetCrosshairMaterialSlot, materialAsset);

    /// <summary>Set the Base Weapon's Damage</summary>
    public void SetDamage(int damage) => Invoke(SetDamageSlot, damage);

    /// <summary>Sets how the Character grabs this Weapon</summary>
    public void SetHandlingMode(HandlingMode handlingMode) => Invoke(SetHandlingModeSlot, handlingMode);

    /// <summary>Sets where the Character's left hand holds this Weapon, relative to the Weapon</summary>
    public void SetLeftHandTransform(Vector leftHandLocation, Rotator leftHandRotation) =>
        Invoke(SetLeftHandTransformSlot, leftHandLocation, leftHandRotation);

    /// <summary>Set the mesh used when the Character reloads the weapon</summary>
    public void SetMagazineMesh(string magazineMesh, string? magazineMeshHideBone = null) =>
        Invoke(SetMagazineMeshSlot, ScriptArguments.Trim([magazineMesh, magazineMeshHideBone], 1));

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

    /// <summary>Sets the Particle of the fire blast at the muzzle</summary>
    public void SetParticlesBarrel(string particleAssetPath) => Invoke(SetParticlesBarrelSlot, particleAssetPath);

    /// <summary>Sets the Particle of the bullet trail</summary>
    public void SetParticlesBulletTrail(string particleBulletTrailAssetPath) =>
        Invoke(SetParticlesBulletTrailSlot, particleBulletTrailAssetPath);

    /// <summary>Sets the Particle of the empty shells ejected when firing</summary>
    public void SetParticlesShells(string particleShellsAssetPath) =>
        Invoke(SetParticlesShellsSlot, particleShellsAssetPath);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Invoke(SetPhysicalMaterialSlot, physicalMaterialPath);

    /// <summary>Sets if this Pickable can be picked up from ground by the player</summary>
    public void SetPickable(bool isPickable) => Invoke(SetPickableSlot, isPickable);

    /// <summary>Sets the base recoil of this Weapon</summary>
    public void SetRecoil(float recoil) => Invoke(SetRecoilSlot, recoil);

    /// <summary>Set the offset of Right Hand</summary>
    public void SetRightHandOffset(Vector rightHandOffset) => Invoke(SetRightHandOffsetSlot, rightHandOffset);

    /// <summary>Sets the Field of View multiplier applied when aiming down sights</summary>
    public void SetSightFOVMultiplier(float sightFovMultiplier) =>
        Invoke(SetSightFOVMultiplierSlot, sightFovMultiplier);

    /// <summary>Sets the offset that aligns the Player's view with the Weapon's sight when aiming down sights</summary>
    public void SetSightTransform(Vector sightLocation, Rotator sightRotation) =>
        Invoke(SetSightTransformSlot, sightLocation, sightRotation);

    /// <summary>Sets the Sound played when starting to aim</summary>
    public void SetSoundAim(string soundAimAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundAimSlot, ScriptArguments.Trim([soundAimAssetPath, volume, pitch], 1));

    /// <summary>Sets the Sound played when trying to fire with an empty clip</summary>
    public void SetSoundDry(string soundDryAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundDrySlot, ScriptArguments.Trim([soundDryAssetPath, volume, pitch], 1));

    /// <summary>Sets the Sound played when firing</summary>
    public void SetSoundFire(string soundFireAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundFireSlot, ScriptArguments.Trim([soundFireAssetPath, volume, pitch], 1));

    /// <summary>Sound when firing with only having X remaining bullets in the magazine</summary>
    public void SetSoundFireLastBullets(string soundAssetPath, int? remainingBulletsCount = null) =>
        Invoke(SetSoundFireLastBulletsSlot, ScriptArguments.Trim([soundAssetPath, remainingBulletsCount], 1));

    /// <summary>Sets the Sound played when loading a magazine</summary>
    public void SetSoundLoad(string soundLoadAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundLoadSlot, ScriptArguments.Trim([soundLoadAssetPath, volume, pitch], 1));

    /// <summary>Sets the Sound played when unloading a magazine</summary>
    public void SetSoundUnload(string soundUnloadAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundUnloadSlot, ScriptArguments.Trim([soundUnloadAssetPath, volume, pitch], 1));

    /// <summary>Sets the Sound played when zooming</summary>
    public void SetSoundZooming(string soundZoomingAssetPath, float? volume = null, float? pitch = null) =>
        Invoke(SetSoundZoomingSlot, ScriptArguments.Trim([soundZoomingAssetPath, volume, pitch], 1));

    /// <summary>Sets the base spread of this Weapon</summary>
    public void SetSpread(float spread) => Invoke(SetSpreadSlot, spread);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke(SetStaticMeshAttachedTransformSlot, id, relativeLocation, relativeRotation);

    /// <summary>Sets if the Weapon can hold to keep firing and if it needs to release to fire</summary>
    public void SetUsageSettings(bool canHoldUse, bool holdReleaseUse) =>
        Invoke(SetUsageSettingsSlot, canHoldUse, holdReleaseUse);

    /// <summary>Sets how the bullet will pass through walls</summary>
    public void SetWallbangSettings(float maxDistance, float damageMultiplier) =>
        Invoke(SetWallbangSettingsSlot, maxDistance, damageMultiplier);

    /// <summary>Stops an Animation Montage on this Weapon</summary>
    public void StopAnimation(string? animationAsset = null) =>
        Invoke(StopAnimationSlot, ScriptArguments.Trim([animationAsset], 0));

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
