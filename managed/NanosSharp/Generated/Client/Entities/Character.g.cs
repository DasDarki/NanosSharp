#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Character
{
    /// <summary>When an Animation Montage Notify begins</summary>
    public static event AnimationBeginNotifyHandler? OnAnimationBeginNotify
    {
        add => AnimationBeginNotifyEvent.Add(value);
        remove => AnimationBeginNotifyEvent.Remove(value);
    }

    /// <summary>When an Animation Montage Notify ends</summary>
    public static event AnimationEndNotifyHandler? OnAnimationEndNotify
    {
        add => AnimationEndNotifyEvent.Add(value);
        remove => AnimationEndNotifyEvent.Remove(value);
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<AnimationBeginNotifyHandler> AnimationBeginNotifyEvent = new("Character", "AnimationBeginNotify");
    private static readonly ClassEvent<AnimationEndNotifyHandler> AnimationEndNotifyEvent = new("Character", "AnimationEndNotify");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Character", "NetworkAuthorityChange");
    private static readonly MethodSlot GetAllMorphTargetNamesSlot = new("GetAllMorphTargetNames");
    private static readonly MethodSlot SetCameraOffsetSlot = new("SetCameraOffset");
    private static readonly MethodSlot SetMaterialFromCanvasSlot = new("SetMaterialFromCanvas");
    private static readonly MethodSlot SetMaterialFromSceneCaptureSlot = new("SetMaterialFromSceneCapture");
    private static readonly MethodSlot SetMaterialFromWebUISlot = new("SetMaterialFromWebUI");

    /// <summary>Gets the camera mode</summary>
    public CameraMode CameraMode => GetCameraMode();

    /// <summary>Gets if this Character is allowed to aim</summary>
    public bool CanAim => GetCanAim();

    /// <summary>Gets if this Character is allowed to deploy the Parachute</summary>
    public bool CanDeployParachute => GetCanDeployParachute();

    /// <summary>Gets if this Character is allowed to Dive</summary>
    public bool CanDive => GetCanDive();

    /// <summary>Gets if this Character is allowed to drop the picked up item</summary>
    public bool CanDrop => GetCanDrop();

    /// <summary>Gets if this Character is allowed to grab Props</summary>
    public bool CanGrabProps => GetCanGrabProps();

    /// <summary>Gets if this Character is allowed to pick up Pickables (Weapon, Grenade, Melee...)</summary>
    public bool CanPickupPickables => GetCanPickupPickables();

    /// <summary>Gets if this Character is allowed to punch</summary>
    public bool CanPunch => GetCanPunch();

    /// <summary>Gets if this Character is allowed to sprint</summary>
    public bool CanSprint => GetCanSprint();

    /// <summary>Gets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public bool CanUsePickables => GetCanUsePickables();

    /// <summary>Gets the Fall Damage multiplier</summary>
    public ushort FallDamageTaken => GetFallDamageTaken();

    /// <summary>Gets the Footstep Volume multiplier</summary>
    public float FootstepVolumeMultiplier => GetFootstepVolumeMultiplier();

    /// <summary>Gets the current Gait Mode</summary>
    public GaitMode GaitMode => GetGaitMode();

    /// <summary>Gets the current health</summary>
    public uint Health => GetHealth();

    /// <summary>Gets the Impact Damage taken when being run over</summary>
    public ushort ImpactDamageTaken => GetImpactDamageTaken();

    /// <summary>Gets the Max Health</summary>
    public uint MaxHealth => GetMaxHealth();

    /// <summary>Gets the base Mesh Asset</summary>
    public string Mesh => GetMesh();

    /// <summary>Gets the damage dealt by this Character's punches</summary>
    public ushort PunchDamage => GetPunchDamage();

    /// <summary>Gets the movement speed multiplier</summary>
    public float SpeedMultiplier => GetSpeedMultiplier();

    /// <summary>Gets the Stance Mode</summary>
    public StanceMode StanceMode => GetStanceMode();

    /// <summary>Gets if this Character is in Shooter Mode</summary>
    public bool WeaponShooterMode => GetWeaponShooterMode();

    /// <summary>Gets if this Character has input enabled</summary>
    public bool InputEnabled => IsInputEnabled();

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool Invulnerable => IsInvulnerable();

    /// <summary>Gets whether the Character can move</summary>
    public bool MovementEnabled => IsMovementEnabled();

    /// <summary>Returns a table with all morph targets available</summary>
    public IReadOnlyList<string> AllMorphTargetNames => GetAllMorphTargetNames();

    /// <summary>Returns a table with all morph targets available</summary>
    public IReadOnlyList<string> GetAllMorphTargetNames() =>
        CallResult<IReadOnlyList<string>>(GetAllMorphTargetNamesSlot, 0)!;

    /// <summary>Sets the Vehicle Camera Offset</summary>
    public void SetCameraOffset(Vector offset) => Call(SetCameraOffsetSlot, 1, offset);

    /// <summary>Sets the material at the specified index of this Actor to a Canvas object</summary>
    public void SetMaterialFromCanvas(Entity canvas, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromCanvasSlot, 1, canvas, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a SceneCapture object</summary>
    public void SetMaterialFromSceneCapture(Entity sceneCapture, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromSceneCaptureSlot, 1, sceneCapture, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a WebUI object</summary>
    public void SetMaterialFromWebUI(Entity webui, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromWebUISlot, 1, webui, index, argument3);

    /// <summary>Subscribes to the AnimationBeginNotify event of this entity only.</summary>
    public ScriptSubscription SubscribeAnimationBeginNotify(AnimationBeginNotifyHandler handler) => Subscribe("AnimationBeginNotify", handler);

    /// <summary>Subscribes to the AnimationEndNotify event of this entity only.</summary>
    public ScriptSubscription SubscribeAnimationEndNotify(AnimationEndNotifyHandler handler) => Subscribe("AnimationEndNotify", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the AnimationBeginNotify event of this entity when a subclass overrides it.</summary>
    [EventHook("AnimationBeginNotify")]
    protected virtual void HandleAnimationBeginNotify(string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime)
    {
    }

    /// <summary>Called for the AnimationEndNotify event of this entity when a subclass overrides it.</summary>
    [EventHook("AnimationEndNotify")]
    protected virtual void HandleAnimationEndNotify(string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime)
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>When an Animation Montage Notify begins</summary>
    public delegate void AnimationBeginNotifyHandler(Character self, string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime);

    /// <summary>When an Animation Montage Notify ends</summary>
    public delegate void AnimationEndNotifyHandler(Character self, string? notifyName, string? animationName, double triggerBeginTime, double triggerEndTime);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Character self, bool isNetworkAuthority);
}
