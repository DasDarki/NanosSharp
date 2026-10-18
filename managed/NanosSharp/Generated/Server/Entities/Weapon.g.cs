#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Weapon
{
    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public static event InteractHandler? OnInteract
    {
        add => InteractEvent.Add(value);
        remove => InteractEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Weapon", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Weapon", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Weapon", "NetworkAuthorityChange");
    private static readonly MethodSlot PullUseSlot = new("PullUse");
    private static readonly MethodSlot ReleaseUseSlot = new("ReleaseUse");
    private static readonly MethodSlot ReloadSlot = new("Reload");
    private static readonly MethodSlot SetAmmoBagSlot = new("SetAmmoBag");
    private static readonly MethodSlot SetAmmoClipSlot = new("SetAmmoClip");
    private static readonly MethodSlot SetAmmoSettingsSlot = new("SetAmmoSettings");
    private static readonly MethodSlot SetAnimationCharacterFireSlot = new("SetAnimationCharacterFire");
    private static readonly MethodSlot SetAnimationFireSlot = new("SetAnimationFire");
    private static readonly MethodSlot SetAnimationReloadSlot = new("SetAnimationReload");
    private static readonly MethodSlot SetAutoReloadSlot = new("SetAutoReload");
    private static readonly MethodSlot SetBulletColorSlot = new("SetBulletColor");
    private static readonly MethodSlot SetBulletSettingsSlot = new("SetBulletSettings");
    private static readonly MethodSlot SetCadenceSlot = new("SetCadence");
    private static readonly MethodSlot SetClipCapacitySlot = new("SetClipCapacity");
    private static readonly MethodSlot SetDamageSlot = new("SetDamage");
    private static readonly MethodSlot SetHandlingModeSlot = new("SetHandlingMode");
    private static readonly MethodSlot SetLeftHandTransformSlot = new("SetLeftHandTransform");
    private static readonly MethodSlot SetMagazineMeshSlot = new("SetMagazineMesh");
    private static readonly MethodSlot SetParticlesBarrelSlot = new("SetParticlesBarrel");
    private static readonly MethodSlot SetParticlesBulletTrailSlot = new("SetParticlesBulletTrail");
    private static readonly MethodSlot SetParticlesShellsSlot = new("SetParticlesShells");
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
    private static readonly MethodSlot SetUsageSettingsSlot = new("SetUsageSettings");
    private static readonly MethodSlot SetWallbangSettingsSlot = new("SetWallbangSettings");

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

    /// <summary>Gets the time between shots</summary>
    public float Cadence
    {
        get => GetCadence();
        set => SetCadence(value);
    }

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

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode HandlingMode
    {
        get => GetHandlingMode();
        set => SetHandlingMode(value);
    }

    /// <summary>Gets the magazine Static Mesh used when reloading</summary>
    public string MagazineMesh
    {
        get => GetMagazineMesh();
        set => SetMagazineMesh(value);
    }

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

    /// <summary>Pulls the usage of this Pickable (will start firing if this is a weapon)</summary>
    public void PullUse(float? releaseUseAfter = null) => Call(PullUseSlot, 0, releaseUseAfter);

    /// <summary>Releases the usage of this Pickable (will stop firing if this is a weapon)</summary>
    public void ReleaseUse() => Call(ReleaseUseSlot, 0);

    /// <summary>Forces this Weapon to reload</summary>
    public void Reload() => Call(ReloadSlot, 0);

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

    /// <summary>Sets if the Character will reload automatically when the clip is empty</summary>
    public void SetAutoReload(bool autoReload) => Call(SetAutoReloadSlot, 1, autoReload);

    /// <summary>Set the Bullet Color</summary>
    public void SetBulletColor(Color bulletColor) => Call(SetBulletColorSlot, 1, bulletColor);

    /// <summary>Sets how many bullets are fired per shot and how they look and travel</summary>
    public void SetBulletSettings(uint bulletCount, float bulletMaxDistance, uint bulletVelocity, Color bulletColor) =>
        Call(SetBulletSettingsSlot, 4, bulletCount, bulletMaxDistance, bulletVelocity, bulletColor);

    /// <summary>Sets the time between shots</summary>
    public void SetCadence(float cadence) => Call(SetCadenceSlot, 1, cadence);

    /// <summary>Sets the max number of bullets the Weapon's clip can hold</summary>
    public void SetClipCapacity(int clip) => Call(SetClipCapacitySlot, 1, clip);

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

    /// <summary>Sets the Particle of the fire blast at the muzzle</summary>
    public void SetParticlesBarrel(string particleAssetPath) => Call(SetParticlesBarrelSlot, 1, particleAssetPath);

    /// <summary>Sets the Particle of the bullet trail</summary>
    public void SetParticlesBulletTrail(string particleBulletTrailAssetPath) =>
        Call(SetParticlesBulletTrailSlot, 1, particleBulletTrailAssetPath);

    /// <summary>Sets the Particle of the empty shells ejected when firing</summary>
    public void SetParticlesShells(string particleShellsAssetPath) =>
        Call(SetParticlesShellsSlot, 1, particleShellsAssetPath);

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

    /// <summary>Sets if the Weapon can hold to keep firing and if it needs to release to fire</summary>
    public void SetUsageSettings(bool canHoldUse, bool holdReleaseUse) =>
        Call(SetUsageSettingsSlot, 2, canHoldUse, holdReleaseUse);

    /// <summary>Sets how the bullet will pass through walls</summary>
    public void SetWallbangSettings(float maxDistance, float damageMultiplier) =>
        Call(SetWallbangSettingsSlot, 2, maxDistance, damageMultiplier);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Interact event of this entity when a subclass overrides it.</summary>
    [EventHook("Interact")]
    protected virtual bool HandleInteract(Character? character) => true;

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Weapon self, int oldDimension, int newDimension);

    /// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
    public delegate bool InteractHandler(Weapon self, Character? character);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Weapon self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
