#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Weapon
{
    /// <summary>Triggered when bullets hit (this will be triggered for each bullet shot)</summary>
    public static event BulletHitHandler? OnBulletHit
    {
        add => BulletHitEvent.Add(value);
        remove => BulletHitEvent.Remove(value);
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<BulletHitHandler> BulletHitEvent = new("Weapon", "BulletHit");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Weapon", "NetworkAuthorityChange");
    private static readonly MethodSlot SetMaterialFromCanvasSlot = new("SetMaterialFromCanvas");
    private static readonly MethodSlot SetMaterialFromSceneCaptureSlot = new("SetMaterialFromSceneCapture");
    private static readonly MethodSlot SetMaterialFromWebUISlot = new("SetMaterialFromWebUI");

    /// <summary>Gets this Weapon's Ammo Bag</summary>
    public int AmmoBag => GetAmmoBag();

    /// <summary>Gets this Weapon's Ammo Clip</summary>
    public int AmmoClip => GetAmmoClip();

    /// <summary>Gets the Animation played by the Character when firing</summary>
    public string AnimationCharacterFire => GetAnimationCharacterFire();

    /// <summary>Gets the Animation played by the Weapon when firing</summary>
    public string AnimationFire => GetAnimationFire();

    /// <summary>Gets the reload animation</summary>
    public string AnimationReload => GetAnimationReload();

    /// <summary>Gets the color of the bullet trail</summary>
    public Color BulletColor => GetBulletColor();

    /// <summary>Gets the time between shots</summary>
    public float Cadence => GetCadence();

    /// <summary>Gets the max number of bullets the clip can hold</summary>
    public int ClipCapacity => GetClipCapacity();

    /// <summary>Gets the base damage of this Weapon</summary>
    public int Damage => GetDamage();

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    public HandlingMode HandlingMode => GetHandlingMode();

    /// <summary>Gets the magazine Static Mesh used when reloading</summary>
    public string MagazineMesh => GetMagazineMesh();

    /// <summary>Gets the Particle of the Fire Blast in the muzzle</summary>
    public string ParticlesBarrel => GetParticlesBarrel();

    /// <summary>Gets the Particle of the bullet trail</summary>
    public string ParticlesBulletTrail => GetParticlesBulletTrail();

    /// <summary>Gets the Particle of the ejected shells</summary>
    public string ParticlesShells => GetParticlesShells();

    /// <summary>Gets the base recoil of this Weapon</summary>
    public float Recoil => GetRecoil();

    /// <summary>Gets the offset of the right hand, relative to the camera</summary>
    public Vector RightHandOffset => GetRightHandOffset();

    /// <summary>Gets the Field of View multiplier applied when aiming down sights</summary>
    public float SightFOVMultiplier => GetSightFOVMultiplier();

    /// <summary>Gets the Sound played when aiming</summary>
    public string SoundAim => GetSoundAim();

    /// <summary>Gets the Sound played when firing with an empty clip</summary>
    public string SoundDry => GetSoundDry();

    /// <summary>Gets the Sound played when firing</summary>
    public string SoundFire => GetSoundFire();

    /// <summary>Gets the Sound played when loading a magazine</summary>
    public string SoundLoad => GetSoundLoad();

    /// <summary>Gets the Sound played when unloading a magazine</summary>
    public string SoundUnload => GetSoundUnload();

    /// <summary>Gets the Sound played when zooming</summary>
    public string SoundZooming => GetSoundZooming();

    /// <summary>Gets the base spread of this Weapon</summary>
    public float Spread => GetSpread();

    /// <summary>Sets the material at the specified index of this Actor to a Canvas object</summary>
    public void SetMaterialFromCanvas(Entity canvas, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromCanvasSlot, 1, canvas, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a SceneCapture object</summary>
    public void SetMaterialFromSceneCapture(Entity sceneCapture, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromSceneCaptureSlot, 1, sceneCapture, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a WebUI object</summary>
    public void SetMaterialFromWebUI(Entity webui, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromWebUISlot, 1, webui, index, argument3);

    /// <summary>Subscribes to the BulletHit event of this entity only.</summary>
    public ScriptSubscription SubscribeBulletHit(BulletHitHandler handler) => Subscribe("BulletHit", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the BulletHit event of this entity when a subclass overrides it.</summary>
    [EventHook("BulletHit")]
    protected virtual void HandleBulletHit(Vector impactPoint, Vector impactNormal, int damage, Actor? actorHit)
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Triggered when bullets hit (this will be triggered for each bullet shot)</summary>
    public delegate void BulletHitHandler(Weapon self, Vector impactPoint, Vector impactNormal, int damage, Actor? actorHit);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Weapon self, bool isNetworkAuthority);
}
