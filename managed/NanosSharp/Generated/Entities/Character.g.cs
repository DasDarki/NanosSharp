#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Characters represents Actors which can be possessed, can move and interact with world. They are the default Skeletal Mesh Character built for nanos world.</summary>
[ScriptClass("Character")]
public partial class Character : Actor, IDamageable, IPaintable, IPawn
{
    /// <summary>Triggered when the Character effectively attacks with a Melee</summary>
    public static event AttackHandler? OnAttack
    {
        add => AttackEvent.Add(value);
        remove => AttackEvent.Remove(value);
    }

    /// <summary>Triggered when a Character attempts to enter a vehicle</summary>
    public static event AttemptEnterVehicleHandler? OnAttemptEnterVehicle
    {
        add => AttemptEnterVehicleEvent.Add(value);
        remove => AttemptEnterVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when this Character attempts to leave a vehicle</summary>
    public static event AttemptLeaveVehicleHandler? OnAttemptLeaveVehicle
    {
        add => AttemptLeaveVehicleEvent.Add(value);
        remove => AttemptLeaveVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when this Character attempts to reload</summary>
    public static event AttemptReloadHandler? OnAttemptReload
    {
        add => AttemptReloadEvent.Add(value);
        remove => AttemptReloadEvent.Remove(value);
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

    /// <summary>When Character drops the currently picked up Pickable</summary>
    public static event DropHandler? OnDrop
    {
        add => DropEvent.Add(value);
        remove => DropEvent.Remove(value);
    }

    /// <summary>When Character enters a vehicle</summary>
    public static event EnterVehicleHandler? OnEnterVehicle
    {
        add => EnterVehicleEvent.Add(value);
        remove => EnterVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>When Character grabs up a Prop</summary>
    public static event GrabPropHandler? OnGrabProp
    {
        add => GrabPropEvent.Add(value);
        remove => GrabPropEvent.Remove(value);
    }

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public static event HealthChangeHandler? OnHealthChange
    {
        add => HealthChangeEvent.Add(value);
        remove => HealthChangeEvent.Remove(value);
    }

    /// <summary>When Character highlights/looks at a Prop or a Pickable</summary>
    public static event HighlightHandler? OnHighlight
    {
        add => HighlightEvent.Add(value);
        remove => HighlightEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
    public static event InteractHandler? OnInteract
    {
        add => InteractEvent.Add(value);
        remove => InteractEvent.Remove(value);
    }

    /// <summary>When Character leaves a vehicle</summary>
    public static event LeaveVehicleHandler? OnLeaveVehicle
    {
        add => LeaveVehicleEvent.Add(value);
        remove => LeaveVehicleEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
    }

    /// <summary>Called when AI reaches its destination, or when it fails</summary>
    public static event MoveCompleteHandler? OnMoveComplete
    {
        add => MoveCompleteEvent.Add(value);
        remove => MoveCompleteEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>When Character picks up anything</summary>
    public static event PickUpHandler? OnPickUp
    {
        add => PickUpEvent.Add(value);
        remove => PickUpEvent.Remove(value);
    }

    /// <summary>When this Pawn is possessed by a Player</summary>
    public static event PossessHandler? OnPossess
    {
        add => PossessEvent.Add(value);
        remove => PossessEvent.Remove(value);
    }

    /// <summary>Triggered when a Character presses the use button for a Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public static event PullUseHandler? OnPullUse
    {
        add => PullUseEvent.Add(value);
        remove => PullUseEvent.Remove(value);
    }

    /// <summary>When Character punches</summary>
    public static event PunchHandler? OnPunch
    {
        add => PunchEvent.Add(value);
        remove => PunchEvent.Remove(value);
    }

    /// <summary>When Character enters or leaves ragdoll</summary>
    public static event RagdollModeChangeHandler? OnRagdollModeChange
    {
        add => RagdollModeChangeEvent.Add(value);
        remove => RagdollModeChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Character releases the use button for a Pickable (i.e. releases left mouse button with this equipped)</summary>
    public static event ReleaseUseHandler? OnReleaseUse
    {
        add => ReleaseUseEvent.Add(value);
        remove => ReleaseUseEvent.Remove(value);
    }

    /// <summary>When Character reloads a weapon</summary>
    public static event ReloadHandler? OnReload
    {
        add => ReloadEvent.Add(value);
        remove => ReloadEvent.Remove(value);
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

    /// <summary>When Character drops a Prop</summary>
    public static event UnGrabPropHandler? OnUnGrabProp
    {
        add => UnGrabPropEvent.Add(value);
        remove => UnGrabPropEvent.Remove(value);
    }

    /// <summary>When this Pawn is unpossessed by a Player</summary>
    public static event UnPossessHandler? OnUnPossess
    {
        add => UnPossessEvent.Add(value);
        remove => UnPossessEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<AttackHandler> AttackEvent = new("Character", "Attack");
    private static readonly ClassEvent<AttemptEnterVehicleHandler> AttemptEnterVehicleEvent = new("Character", "AttemptEnterVehicle");
    private static readonly ClassEvent<AttemptLeaveVehicleHandler> AttemptLeaveVehicleEvent = new("Character", "AttemptLeaveVehicle");
    private static readonly ClassEvent<AttemptReloadHandler> AttemptReloadEvent = new("Character", "AttemptReload");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Character", "ClassRegister");
    private static readonly ClassEvent<DeathHandler> DeathEvent = new("Character", "Death");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Character", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Character", "DimensionChange");
    private static readonly ClassEvent<DropHandler> DropEvent = new("Character", "Drop");
    private static readonly ClassEvent<EnterVehicleHandler> EnterVehicleEvent = new("Character", "EnterVehicle");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Character", "EnterWater");
    private static readonly ClassEvent<GrabPropHandler> GrabPropEvent = new("Character", "GrabProp");
    private static readonly ClassEvent<HealthChangeHandler> HealthChangeEvent = new("Character", "HealthChange");
    private static readonly ClassEvent<HighlightHandler> HighlightEvent = new("Character", "Highlight");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Character", "Interact");
    private static readonly ClassEvent<LeaveVehicleHandler> LeaveVehicleEvent = new("Character", "LeaveVehicle");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Character", "LeaveWater");
    private static readonly ClassEvent<MoveCompleteHandler> MoveCompleteEvent = new("Character", "MoveComplete");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Character", "NetworkAuthorityChange");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("Character", "PickUp");
    private static readonly ClassEvent<PossessHandler> PossessEvent = new("Character", "Possess");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("Character", "PullUse");
    private static readonly ClassEvent<PunchHandler> PunchEvent = new("Character", "Punch");
    private static readonly ClassEvent<RagdollModeChangeHandler> RagdollModeChangeEvent = new("Character", "RagdollModeChange");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("Character", "ReleaseUse");
    private static readonly ClassEvent<ReloadHandler> ReloadEvent = new("Character", "Reload");
    private static readonly ClassEvent<RespawnHandler> RespawnEvent = new("Character", "Respawn");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Character", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("Character", "TakeDamage");
    private static readonly ClassEvent<UnGrabPropHandler> UnGrabPropEvent = new("Character", "UnGrabProp");
    private static readonly ClassEvent<UnPossessHandler> UnPossessEvent = new("Character", "UnPossess");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Character", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot ClearMorphTargetsSlot = new("ClearMorphTargets");
    private static readonly MethodSlot DropSlot = new("Drop");
    private static readonly MethodSlot EnterVehicleSlot = new("EnterVehicle");
    private static readonly MethodSlot FollowSlot = new("Follow");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetCameraModeSlot = new("GetCameraMode");
    private static readonly MethodSlot GetCanAimSlot = new("GetCanAim");
    private static readonly MethodSlot GetCanCrouchSlot = new("GetCanCrouch");
    private static readonly MethodSlot GetCanDeployParachuteSlot = new("GetCanDeployParachute");
    private static readonly MethodSlot GetCanDiveSlot = new("GetCanDive");
    private static readonly MethodSlot GetCanDropSlot = new("GetCanDrop");
    private static readonly MethodSlot GetCanGrabPropsSlot = new("GetCanGrabProps");
    private static readonly MethodSlot GetCanJumpSlot = new("GetCanJump");
    private static readonly MethodSlot GetCanPickupPickablesSlot = new("GetCanPickupPickables");
    private static readonly MethodSlot GetCanPunchSlot = new("GetCanPunch");
    private static readonly MethodSlot GetCanSprintSlot = new("GetCanSprint");
    private static readonly MethodSlot GetCanUsePickablesSlot = new("GetCanUsePickables");
    private static readonly MethodSlot GetCapsuleSizeSlot = new("GetCapsuleSize");
    private static readonly MethodSlot GetControlRotationSlot = new("GetControlRotation");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot GetFallDamageTakenSlot = new("GetFallDamageTaken");
    private static readonly MethodSlot GetFallingModeSlot = new("GetFallingMode");
    private static readonly MethodSlot GetFlyingModeSlot = new("GetFlyingMode");
    private static readonly MethodSlot GetFootstepVolumeMultiplierSlot = new("GetFootstepVolumeMultiplier");
    private static readonly MethodSlot GetGaitModeSlot = new("GetGaitMode");
    private static readonly MethodSlot GetGrabbedPropSlot = new("GetGrabbedProp");
    private static readonly MethodSlot GetGravityScaleSlot = new("GetGravityScale");
    private static readonly MethodSlot GetHealthSlot = new("GetHealth");
    private static readonly MethodSlot GetImpactDamageTakenSlot = new("GetImpactDamageTaken");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot GetJumpZVelocitySlot = new("GetJumpZVelocity");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMaxHealthSlot = new("GetMaxHealth");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetMorphTargetSlot = new("GetMorphTarget");
    private static readonly MethodSlot GetMovingToSlot = new("GetMovingTo");
    private static readonly MethodSlot GetPickedSlot = new("GetPicked");
    private static readonly MethodSlot GetPlayerSlot = new("GetPlayer");
    private static readonly MethodSlot GetPunchDamageSlot = new("GetPunchDamage");
    private static readonly MethodSlot GetRadialDamageToRagdollSlot = new("GetRadialDamageToRagdoll");
    private static readonly MethodSlot GetSpeedMultiplierSlot = new("GetSpeedMultiplier");
    private static readonly MethodSlot GetStanceModeSlot = new("GetStanceMode");
    private static readonly MethodSlot GetSwimmingModeSlot = new("GetSwimmingMode");
    private static readonly MethodSlot GetTeamSlot = new("GetTeam");
    private static readonly MethodSlot GetVehicleSlot = new("GetVehicle");
    private static readonly MethodSlot GetVehicleSeatSlot = new("GetVehicleSeat");
    private static readonly MethodSlot GetViewModeSlot = new("GetViewMode");
    private static readonly MethodSlot GetWeaponAimModeSlot = new("GetWeaponAimMode");
    private static readonly MethodSlot GetWeaponShooterModeSlot = new("GetWeaponShooterMode");
    private static readonly MethodSlot GrabPropSlot = new("GrabProp");
    private static readonly MethodSlot HideBoneSlot = new("HideBone");
    private static readonly MethodSlot IsBoneHiddenSlot = new("IsBoneHidden");
    private static readonly MethodSlot IsDeadSlot = new("IsDead");
    private static readonly MethodSlot IsInRagdollModeSlot = new("IsInRagdollMode");
    private static readonly MethodSlot IsInputEnabledSlot = new("IsInputEnabled");
    private static readonly MethodSlot IsInvulnerableSlot = new("IsInvulnerable");
    private static readonly MethodSlot IsMovementEnabledSlot = new("IsMovementEnabled");
    private static readonly MethodSlot JumpSlot = new("Jump");
    private static readonly MethodSlot LeaveVehicleSlot = new("LeaveVehicle");
    private static readonly MethodSlot LookAtSlot = new("LookAt");
    private static readonly MethodSlot MoveToSlot = new("MoveTo");
    private static readonly MethodSlot PickUpSlot = new("PickUp");
    private static readonly MethodSlot PlayAnimationSlot = new("PlayAnimation");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot ResetPhysicalAnimationSettingsSlot = new("ResetPhysicalAnimationSettings");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
    private static readonly MethodSlot SetAIAvoidanceSettingsSlot = new("SetAIAvoidanceSettings");
    private static readonly MethodSlot SetAccelerationSettingsSlot = new("SetAccelerationSettings");
    private static readonly MethodSlot SetAirControlSlot = new("SetAirControl");
    private static readonly MethodSlot SetAnimationIdleWalkRunCrouchingSlot = new("SetAnimationIdleWalkRunCrouching");
    private static readonly MethodSlot SetAnimationIdleWalkRunProningSlot = new("SetAnimationIdleWalkRunProning");
    private static readonly MethodSlot SetAnimationIdleWalkRunStandingSlot = new("SetAnimationIdleWalkRunStanding");
    private static readonly MethodSlot SetAnimationsTransitionCrouchingProningSlot = new("SetAnimationsTransitionCrouchingProning");
    private static readonly MethodSlot SetAnimationsTransitionStandingCrouchingSlot = new("SetAnimationsTransitionStandingCrouching");
    private static readonly MethodSlot SetBrakingSettingsSlot = new("SetBrakingSettings");
    private static readonly MethodSlot SetCameraModeSlot = new("SetCameraMode");
    private static readonly MethodSlot SetCanAimSlot = new("SetCanAim");
    private static readonly MethodSlot SetCanCrouchSlot = new("SetCanCrouch");
    private static readonly MethodSlot SetCanDeployParachuteSlot = new("SetCanDeployParachute");
    private static readonly MethodSlot SetCanDiveSlot = new("SetCanDive");
    private static readonly MethodSlot SetCanDropSlot = new("SetCanDrop");
    private static readonly MethodSlot SetCanGrabPropsSlot = new("SetCanGrabProps");
    private static readonly MethodSlot SetCanJumpSlot = new("SetCanJump");
    private static readonly MethodSlot SetCanPickupPickablesSlot = new("SetCanPickupPickables");
    private static readonly MethodSlot SetCanPunchSlot = new("SetCanPunch");
    private static readonly MethodSlot SetCanSprintSlot = new("SetCanSprint");
    private static readonly MethodSlot SetCanUsePickablesSlot = new("SetCanUsePickables");
    private static readonly MethodSlot SetCapsuleSizeSlot = new("SetCapsuleSize");
    private static readonly MethodSlot SetControlRotationSlot = new("SetControlRotation");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetDeathSoundSlot = new("SetDeathSound");
    private static readonly MethodSlot SetFOVMultiplierSlot = new("SetFOVMultiplier");
    private static readonly MethodSlot SetFallDamageTakenSlot = new("SetFallDamageTaken");
    private static readonly MethodSlot SetFlyingModeSlot = new("SetFlyingMode");
    private static readonly MethodSlot SetFootstepVolumeMultiplierSlot = new("SetFootstepVolumeMultiplier");
    private static readonly MethodSlot SetGaitModeSlot = new("SetGaitMode");
    private static readonly MethodSlot SetGravityScaleSlot = new("SetGravityScale");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetHighFallingTimeSlot = new("SetHighFallingTime");
    private static readonly MethodSlot SetHitReactionEnabledSlot = new("SetHitReactionEnabled");
    private static readonly MethodSlot SetImpactDamageTakenSlot = new("SetImpactDamageTaken");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetInputEnabledSlot = new("SetInputEnabled");
    private static readonly MethodSlot SetInvulnerableSlot = new("SetInvulnerable");
    private static readonly MethodSlot SetJumpZVelocitySlot = new("SetJumpZVelocity");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetMeshSlot = new("SetMesh");
    private static readonly MethodSlot SetMeshSettingsSlot = new("SetMeshSettings");
    private static readonly MethodSlot SetMorphTargetSlot = new("SetMorphTarget");
    private static readonly MethodSlot SetMovementEnabledSlot = new("SetMovementEnabled");
    private static readonly MethodSlot SetPainSoundSlot = new("SetPainSound");
    private static readonly MethodSlot SetParachuteTextureSlot = new("SetParachuteTexture");
    private static readonly MethodSlot SetPhysicalAnimationSettingsSlot = new("SetPhysicalAnimationSettings");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPhysicsAssetSlot = new("SetPhysicsAsset");
    private static readonly MethodSlot SetPunchDamageSlot = new("SetPunchDamage");
    private static readonly MethodSlot SetRadialDamageToRagdollSlot = new("SetRadialDamageToRagdoll");
    private static readonly MethodSlot SetRagdollModeSlot = new("SetRagdollMode");
    private static readonly MethodSlot SetRagdollOnDeathEnabledSlot = new("SetRagdollOnDeathEnabled");
    private static readonly MethodSlot SetRagdollOnHitEnabledSlot = new("SetRagdollOnHitEnabled");
    private static readonly MethodSlot SetRagdollStandUpCooldownSlot = new("SetRagdollStandUpCooldown");
    private static readonly MethodSlot SetSimulatePhysicsSlot = new("SetSimulatePhysics");
    private static readonly MethodSlot SetSpeedMultiplierSlot = new("SetSpeedMultiplier");
    private static readonly MethodSlot SetStanceModeSlot = new("SetStanceMode");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot SetTeamSlot = new("SetTeam");
    private static readonly MethodSlot SetViewModeSlot = new("SetViewMode");
    private static readonly MethodSlot SetWeaponAimModeSlot = new("SetWeaponAimMode");
    private static readonly MethodSlot SetWeaponShooterModeSlot = new("SetWeaponShooterMode");
    private static readonly MethodSlot StopAnimationSlot = new("StopAnimation");
    private static readonly MethodSlot StopMovementSlot = new("StopMovement");
    private static readonly MethodSlot UnGrabPropSlot = new("UnGrabProp");
    private static readonly MethodSlot UnHideBoneSlot = new("UnHideBone");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets the camera mode</summary>
    public CameraMode CameraMode
    {
        get => GetCameraMode();
        set => SetCameraMode(value);
    }

    /// <summary>Gets if this Character is allowed to aim</summary>
    public bool CanAim
    {
        get => GetCanAim();
        set => SetCanAim(value);
    }

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool CanCrouch
    {
        get => GetCanCrouch();
        set => SetCanCrouch(value);
    }

    /// <summary>Gets if this Character is allowed to deploy the Parachute</summary>
    public bool CanDeployParachute
    {
        get => GetCanDeployParachute();
        set => SetCanDeployParachute(value);
    }

    /// <summary>Gets if this Character is allowed to Dive</summary>
    public bool CanDive
    {
        get => GetCanDive();
        set => SetCanDive(value);
    }

    /// <summary>Gets if this Character is allowed to drop the picked up item</summary>
    public bool CanDrop
    {
        get => GetCanDrop();
        set => SetCanDrop(value);
    }

    /// <summary>Gets if this Character is allowed to grab Props</summary>
    public bool CanGrabProps
    {
        get => GetCanGrabProps();
        set => SetCanGrabProps(value);
    }

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool CanJump
    {
        get => GetCanJump();
        set => SetCanJump(value);
    }

    /// <summary>Gets if this Character is allowed to pick up Pickables (Weapon, Grenade, Melee...)</summary>
    public bool CanPickupPickables
    {
        get => GetCanPickupPickables();
        set => SetCanPickupPickables(value);
    }

    /// <summary>Gets if this Character is allowed to punch</summary>
    public bool CanPunch
    {
        get => GetCanPunch();
        set => SetCanPunch(value);
    }

    /// <summary>Gets if this Character is allowed to sprint</summary>
    public bool CanSprint
    {
        get => GetCanSprint();
        set => SetCanSprint(value);
    }

    /// <summary>Gets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public bool CanUsePickables
    {
        get => GetCanUsePickables();
        set => SetCanUsePickables(value);
    }

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> CapsuleSize => GetCapsuleSize();

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator ControlRotation
    {
        get => GetControlRotation();
        set => SetControlRotation(value);
    }

    /// <summary>Gets the Fall Damage multiplier</summary>
    public ushort FallDamageTaken
    {
        get => GetFallDamageTaken();
        set => SetFallDamageTaken(value);
    }

    /// <summary>Gets the current Falling Mode</summary>
    public FallingMode FallingMode => GetFallingMode();

    /// <summary>Gets if it's in Flying mode</summary>
    public bool FlyingMode
    {
        get => GetFlyingMode();
        set => SetFlyingMode(value);
    }

    /// <summary>Gets the Footstep Volume multiplier</summary>
    public float FootstepVolumeMultiplier
    {
        get => GetFootstepVolumeMultiplier();
        set => SetFootstepVolumeMultiplier(value);
    }

    /// <summary>Gets the current Gait Mode</summary>
    public GaitMode GaitMode
    {
        get => GetGaitMode();
        set => SetGaitMode(value);
    }

    /// <summary>Gets the Grabbed Prop</summary>
    public Prop? GrabbedProp => GetGrabbedProp();

    /// <summary>Gets the gravity scale</summary>
    public float GravityScale
    {
        get => GetGravityScale();
        set => SetGravityScale(value);
    }

    /// <summary>Gets the current health</summary>
    public uint Health
    {
        get => GetHealth();
        set => SetHealth(value);
    }

    /// <summary>Gets the Impact Damage taken when being run over</summary>
    public ushort ImpactDamageTaken
    {
        get => GetImpactDamageTaken();
        set => SetImpactDamageTaken(value);
    }

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    public ushort JumpZVelocity
    {
        get => GetJumpZVelocity();
        set => SetJumpZVelocity(value);
    }

    /// <summary>Gets the Max Health</summary>
    public uint MaxHealth
    {
        get => GetMaxHealth();
        set => SetMaxHealth(value);
    }

    /// <summary>Gets the base Mesh Asset</summary>
    public string Mesh
    {
        get => GetMesh();
        set => SetMesh(value);
    }

    /// <summary>Gets the Moving To location</summary>
    public Vector MovingTo => GetMovingTo();

    /// <summary>Gets the Pickable if picking up</summary>
    public object? Picked => GetPicked();

    /// <summary>Gets the possessing Player</summary>
    public Player? Player => GetPlayer();

    /// <summary>Gets the damage dealt by this Character's punches</summary>
    public ushort PunchDamage
    {
        get => GetPunchDamage();
        set => SetPunchDamage(value);
    }

    /// <summary>Gets the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public short RadialDamageToRagdoll
    {
        get => GetRadialDamageToRagdoll();
        set => SetRadialDamageToRagdoll(value);
    }

    /// <summary>Gets the movement speed multiplier</summary>
    public float SpeedMultiplier
    {
        get => GetSpeedMultiplier();
        set => SetSpeedMultiplier(value);
    }

    /// <summary>Gets the Stance Mode</summary>
    public StanceMode StanceMode
    {
        get => GetStanceMode();
        set => SetStanceMode(value);
    }

    /// <summary>Gets the Swimming Mode</summary>
    public SwimmingMode SwimmingMode => GetSwimmingMode();

    /// <summary>Gets the Team of this Character</summary>
    public uint Team
    {
        get => GetTeam();
        set => SetTeam(value);
    }

    /// <summary>Gets the entered Vehicle</summary>
    public object? Vehicle => GetVehicle();

    /// <summary>Gets the entered Vehicle seat</summary>
    public long VehicleSeat => GetVehicleSeat();

    /// <summary>Gets the View Mode</summary>
    public ViewMode ViewMode
    {
        get => GetViewMode();
        set => SetViewMode(value);
    }

    /// <summary>Gets the Weapon Aim Mode</summary>
    public AimMode WeaponAimMode
    {
        get => GetWeaponAimMode();
        set => SetWeaponAimMode(value);
    }

    /// <summary>Gets if this Character is in Shooter Mode</summary>
    public bool WeaponShooterMode
    {
        get => GetWeaponShooterMode();
        set => SetWeaponShooterMode(value);
    }

    /// <summary>Returns if the entity is dead</summary>
    public bool Dead => IsDead();

    /// <summary>Gets if Character is in ragdoll mode</summary>
    public bool InRagdollMode => IsInRagdollMode();

    /// <summary>Gets if this Character has input enabled</summary>
    public bool InputEnabled
    {
        get => IsInputEnabled();
        set => SetInputEnabled(value);
    }

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool Invulnerable
    {
        get => IsInvulnerable();
        set => SetInvulnerable(value);
    }

    /// <summary>Gets whether the Character can move</summary>
    public bool MovementEnabled
    {
        get => IsMovementEnabled();
        set => SetMovementEnabled(value);
    }

    /// <summary>Spawns a new Character.</summary>
    public Character(
        Vector location,
        Rotator rotation,
        string skeletalMeshAsset,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        int? maxHealth = null,
        string? deathSound = null,
        string? painSound = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Character", ScriptArguments.Trim([location, rotation, skeletalMeshAsset, collisionType, gravityEnabled, maxHealth, deathSound, painSound, spawnMode], 3));
    }

    protected Character()
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

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        CallResult<uint, uint, string?, DamageType?, Vector?, Player?, Entity?>(ApplyDamageSlot, 1, damage, boneName, damageType, fromDirection, instigator, causer)!;

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Clear all Morph Target that are set to this Mesh</summary>
    public void ClearMorphTargets() => Call(ClearMorphTargetsSlot, 0);

    /// <summary>Drops any Pickable the Character is holding</summary>
    public void Drop() => Call(DropSlot, 0);

    /// <summary>Enters the Vehicle at Seat (0 - Driver)</summary>
    public void EnterVehicle(Entity vehicle, byte? seat = null) => Call(EnterVehicleSlot, 1, vehicle, seat);

    /// <summary>AI: Makes this Pawn follow another actor</summary>
    public void Follow(
        Entity actor,
        float? acceptanceRadius = null,
        bool? stopOnSucceed = null,
        bool? stopOnFail = null,
        float? updateRate = null) =>
        Call(FollowSlot, 1, actor, acceptanceRadius, stopOnSucceed, stopOnFail, updateRate);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot, 0)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        CallResult<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot, 0)!;

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets the camera mode</summary>
    public CameraMode GetCameraMode() => CallResult<CameraMode>(GetCameraModeSlot, 0)!;

    /// <summary>Gets if this Character is allowed to aim</summary>
    public bool GetCanAim() => CallResult<bool>(GetCanAimSlot, 0)!;

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool GetCanCrouch() => CallResult<bool>(GetCanCrouchSlot, 0)!;

    /// <summary>Gets if this Character is allowed to deploy the Parachute</summary>
    public bool GetCanDeployParachute() => CallResult<bool>(GetCanDeployParachuteSlot, 0)!;

    /// <summary>Gets if this Character is allowed to Dive</summary>
    public bool GetCanDive() => CallResult<bool>(GetCanDiveSlot, 0)!;

    /// <summary>Gets if this Character is allowed to drop the picked up item</summary>
    public bool GetCanDrop() => CallResult<bool>(GetCanDropSlot, 0)!;

    /// <summary>Gets if this Character is allowed to grab Props</summary>
    public bool GetCanGrabProps() => CallResult<bool>(GetCanGrabPropsSlot, 0)!;

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool GetCanJump() => CallResult<bool>(GetCanJumpSlot, 0)!;

    /// <summary>Gets if this Character is allowed to pick up Pickables (Weapon, Grenade, Melee...)</summary>
    public bool GetCanPickupPickables() => CallResult<bool>(GetCanPickupPickablesSlot, 0)!;

    /// <summary>Gets if this Character is allowed to punch</summary>
    public bool GetCanPunch() => CallResult<bool>(GetCanPunchSlot, 0)!;

    /// <summary>Gets if this Character is allowed to sprint</summary>
    public bool GetCanSprint() => CallResult<bool>(GetCanSprintSlot, 0)!;

    /// <summary>Gets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public bool GetCanUsePickables() => CallResult<bool>(GetCanUsePickablesSlot, 0)!;

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> GetCapsuleSize() =>
        CallResult<IReadOnlyDictionary<string, float>>(GetCapsuleSizeSlot, 0)!;

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator GetControlRotation() => CallResult<Rotator>(GetControlRotationSlot, 0)!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) =>
        CallResult<float, string>(GetDamageMultiplierSlot, 1, boneName)!;

    /// <summary>Gets the Fall Damage multiplier</summary>
    public ushort GetFallDamageTaken() => CallResult<ushort>(GetFallDamageTakenSlot, 0)!;

    /// <summary>Gets the current Falling Mode</summary>
    public FallingMode GetFallingMode() => CallResult<FallingMode>(GetFallingModeSlot, 0)!;

    /// <summary>Gets if it's in Flying mode</summary>
    public bool GetFlyingMode() => CallResult<bool>(GetFlyingModeSlot, 0)!;

    /// <summary>Gets the Footstep Volume multiplier</summary>
    public float GetFootstepVolumeMultiplier() => CallResult<float>(GetFootstepVolumeMultiplierSlot, 0)!;

    /// <summary>Gets the current Gait Mode</summary>
    public GaitMode GetGaitMode() => CallResult<GaitMode>(GetGaitModeSlot, 0)!;

    /// <summary>Gets the Grabbed Prop</summary>
    public Prop? GetGrabbedProp() => CallResult<Prop?>(GetGrabbedPropSlot, 0);

    /// <summary>Gets the gravity scale</summary>
    public float GetGravityScale() => CallResult<float>(GetGravityScaleSlot, 0)!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => CallResult<uint>(GetHealthSlot, 0)!;

    /// <summary>Gets the Impact Damage taken when being run over</summary>
    public ushort GetImpactDamageTaken() => CallResult<ushort>(GetImpactDamageTakenSlot, 0)!;

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector GetInitialLocation() => CallResult<Vector>(GetInitialLocationSlot, 0)!;

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    public ushort GetJumpZVelocity() => CallResult<ushort>(GetJumpZVelocitySlot, 0)!;

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

    /// <summary>Gets the base Mesh Asset</summary>
    public string GetMesh() => CallResult<string>(GetMeshSlot, 0)!;

    /// <summary>Returns the value of a Morph Target</summary>
    public float GetMorphTarget(string name) => CallResult<float, string>(GetMorphTargetSlot, 1, name)!;

    /// <summary>Gets the Moving To location</summary>
    public Vector GetMovingTo() => CallResult<Vector>(GetMovingToSlot, 0)!;

    /// <summary>Gets the Pickable if picking up</summary>
    public object? GetPicked() => CallResult<object?>(GetPickedSlot, 0);

    /// <summary>Gets the possessing Player</summary>
    public Player? GetPlayer() => CallResult<Player?>(GetPlayerSlot, 0);

    /// <summary>Gets the damage dealt by this Character's punches</summary>
    public ushort GetPunchDamage() => CallResult<ushort>(GetPunchDamageSlot, 0)!;

    /// <summary>Gets the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public short GetRadialDamageToRagdoll() => CallResult<short>(GetRadialDamageToRagdollSlot, 0)!;

    /// <summary>Gets the movement speed multiplier</summary>
    public float GetSpeedMultiplier() => CallResult<float>(GetSpeedMultiplierSlot, 0)!;

    /// <summary>Gets the Stance Mode</summary>
    public StanceMode GetStanceMode() => CallResult<StanceMode>(GetStanceModeSlot, 0)!;

    /// <summary>Gets the Swimming Mode</summary>
    public SwimmingMode GetSwimmingMode() => CallResult<SwimmingMode>(GetSwimmingModeSlot, 0)!;

    /// <summary>Gets the Team of this Character</summary>
    public uint GetTeam() => CallResult<uint>(GetTeamSlot, 0)!;

    /// <summary>Gets the entered Vehicle</summary>
    public object? GetVehicle() => CallResult<object?>(GetVehicleSlot, 0);

    /// <summary>Gets the entered Vehicle seat</summary>
    public long GetVehicleSeat() => CallResult<long>(GetVehicleSeatSlot, 0)!;

    /// <summary>Gets the View Mode</summary>
    public ViewMode GetViewMode() => CallResult<ViewMode>(GetViewModeSlot, 0)!;

    /// <summary>Gets the Weapon Aim Mode</summary>
    public AimMode GetWeaponAimMode() => CallResult<AimMode>(GetWeaponAimModeSlot, 0)!;

    /// <summary>Gets if this Character is in Shooter Mode</summary>
    public bool GetWeaponShooterMode() => CallResult<bool>(GetWeaponShooterModeSlot, 0)!;

    /// <summary>Gives a Prop to the Character</summary>
    public void GrabProp(Prop prop) => Call(GrabPropSlot, 1, prop);

    /// <summary>Hides a bone of this Pawn</summary>
    public void HideBone(string boneName) => Call(HideBoneSlot, 1, boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => CallResult<bool, string>(IsBoneHiddenSlot, 1, boneName)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => CallResult<bool>(IsDeadSlot, 0)!;

    /// <summary>Gets if Character is in ragdoll mode</summary>
    public bool IsInRagdollMode() => CallResult<bool>(IsInRagdollModeSlot, 0)!;

    /// <summary>Gets if this Character has input enabled</summary>
    public bool IsInputEnabled() => CallResult<bool>(IsInputEnabledSlot, 0)!;

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool IsInvulnerable() => CallResult<bool>(IsInvulnerableSlot, 0)!;

    /// <summary>Gets whether the Character can move</summary>
    public bool IsMovementEnabled() => CallResult<bool>(IsMovementEnabledSlot, 0)!;

    /// <summary>Triggers this Pawn to jump</summary>
    public void Jump() => Call(JumpSlot, 0);

    /// <summary>Leaves the current Vehicle</summary>
    public void LeaveVehicle() => Call(LeaveVehicleSlot, 0);

    /// <summary>AI: Tries to make this Character to look at Location</summary>
    public void LookAt(Vector location) => Call(LookAtSlot, 1, location);

    /// <summary>AI: Makes this Pawn walk to the Location</summary>
    public void MoveTo(Vector location, float? acceptanceRadius = null) =>
        Call(MoveToSlot, 1, location, acceptanceRadius);

    /// <summary>Gives a Melee/Grenade/Weapon (Pickable) to the Character</summary>
    public void PickUp(Entity pickable) => Call(PickUpSlot, 1, pickable);

    /// <summary>Plays an Animation Montage on this character</summary>
    public void PlayAnimation(
        string animationPath,
        AnimationSlotType? slotType = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Call(PlayAnimationSlot, 1, animationPath, slotType, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages);

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Call(RemoveAllSkeletalMeshesAttachedSlot, 0);

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Call(RemoveAllStaticMeshesAttachedSlot, 0);

    /// <summary>Removes, if it exists, a SkeletalMesh from this entity given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Call(RemoveSkeletalMeshAttachedSlot, 1, id);

    /// <summary>Removes, if it exists, a StaticMesh from this entity given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Call(RemoveStaticMeshAttachedSlot, 1, id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Call(ResetMaterialSlot, 0, index, attachableId);

    /// <summary>Resets all Physical Animation settings</summary>
    public void ResetPhysicalAnimationSettings() => Call(ResetPhysicalAnimationSettingsSlot, 0);

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) => Call(RespawnSlot, 0, location, rotation);

    /// <summary>AI: Configures the RVO (Reciprocal Velocity Obstacles) Avoidance system used by the AIs when moving</summary>
    public void SetAIAvoidanceSettings(bool enabled, ushort? avoidanceConsiderationRadius = null) =>
        Call(SetAIAvoidanceSettingsSlot, 1, enabled, avoidanceConsiderationRadius);

    /// <summary>Sets the Movement Max Acceleration of this Character</summary>
    public void SetAccelerationSettings(
        ushort? walking = null,
        ushort? parachuting = null,
        ushort? skydiving = null,
        ushort? falling = null,
        ushort? swimming = null,
        ushort? swimmingSurface = null,
        ushort? flying = null) =>
        Call(SetAccelerationSettingsSlot, 0, walking, parachuting, skydiving, falling, swimming, swimmingSurface, flying);

    /// <summary>Sets the amount of movement control allowed when it is in air</summary>
    public void SetAirControl(
        float? airControl = null,
        float? boostMultiplier = null,
        float? boostVelocityThreshold = null) =>
        Call(SetAirControlSlot, 0, airControl, boostMultiplier, boostVelocityThreshold);

    /// <summary>Sets the Blend Space (2D) Animation for CrouchingHorizontal Axis stands for Speed X and Vertical Axis for Speed Y</summary>
    public void SetAnimationIdleWalkRunCrouching(string blendSpacePath, bool? enableTurnInPlace = null) =>
        Call(SetAnimationIdleWalkRunCrouchingSlot, 1, blendSpacePath, enableTurnInPlace);

    /// <summary>Sets the Blend Space (2D) Animation for ProningHorizontal Axis stands for Speed X and Vertical Axis for Speed Y</summary>
    public void SetAnimationIdleWalkRunProning(string blendSpacePath) =>
        Call(SetAnimationIdleWalkRunProningSlot, 1, blendSpacePath);

    /// <summary>Sets the Blend Space (2D) Animation for Standing</summary>
    public void SetAnimationIdleWalkRunStanding(string blendSpacePath, bool? enableTurnInPlace = null) =>
        Call(SetAnimationIdleWalkRunStandingSlot, 1, blendSpacePath, enableTurnInPlace);

    /// <summary>Sets the Transition Animation between Crouching and Proning</summary>
    public void SetAnimationsTransitionCrouchingProning(string crouchingToProning, string proningToCrouching) =>
        Call(SetAnimationsTransitionCrouchingProningSlot, 2, crouchingToProning, proningToCrouching);

    /// <summary>Sets the Transition Animation between Standing and Crouching</summary>
    public void SetAnimationsTransitionStandingCrouching(string standingToCrouching, string crouchingToStanding) =>
        Call(SetAnimationsTransitionStandingCrouchingSlot, 2, standingToCrouching, crouchingToStanding);

    /// <summary>Sets the Movement Braking Settings of this Pawn</summary>
    public void SetBrakingSettings(
        float? groundFriction = null,
        float? brakingFrictionFactor = null,
        ushort? brakingWalking = null,
        ushort? brakingFlying = null,
        ushort? brakingSwimming = null,
        ushort? brakingFalling = null) =>
        Call(SetBrakingSettingsSlot, 0, groundFriction, brakingFrictionFactor, brakingWalking, brakingFlying, brakingSwimming, brakingFalling);

    /// <summary>Sets the Camera Mode</summary>
    public void SetCameraMode(CameraMode cameraMode) => Call(SetCameraModeSlot, 1, cameraMode);

    /// <summary>Sets if this Character is allowed to Aim</summary>
    public void SetCanAim(bool canAim) => Call(SetCanAimSlot, 1, canAim);

    /// <summary>Sets if this Pawn is allowed to Crouch</summary>
    public void SetCanCrouch(bool canCrouch) => Call(SetCanCrouchSlot, 1, canCrouch);

    /// <summary>Sets if this Character is allowed to deploy the Parachute</summary>
    public void SetCanDeployParachute(bool canDeployParachute) =>
        Call(SetCanDeployParachuteSlot, 1, canDeployParachute);

    /// <summary>Sets if this Character is allowed to Dive</summary>
    public void SetCanDive(bool canDive) => Call(SetCanDiveSlot, 1, canDive);

    /// <summary>Sets if this Character is allowed to Drop the Picked up item</summary>
    public void SetCanDrop(bool canDrop) => Call(SetCanDropSlot, 1, canDrop);

    /// <summary>Sets if this Character is allowed to Grab any Prop</summary>
    public void SetCanGrabProps(bool canGrabProps) => Call(SetCanGrabPropsSlot, 1, canGrabProps);

    /// <summary>Sets if this Pawn is allowed to Jump</summary>
    public void SetCanJump(bool canJump) => Call(SetCanJumpSlot, 1, canJump);

    /// <summary>Sets if this Character is allowed to Pick up any Pickable (Weapon, Grenade, Melee...)</summary>
    public void SetCanPickupPickables(bool canPickup) => Call(SetCanPickupPickablesSlot, 1, canPickup);

    /// <summary>Sets if this Character is allowed to Punch</summary>
    public void SetCanPunch(bool canPunch) => Call(SetCanPunchSlot, 1, canPunch);

    /// <summary>Sets if this Character is allowed to Sprint</summary>
    public void SetCanSprint(bool canSprint) => Call(SetCanSprintSlot, 1, canSprint);

    /// <summary>Sets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public void SetCanUsePickables(bool canUse) => Call(SetCanUsePickablesSlot, 1, canUse);

    /// <summary>Sets this Pawn's Capsule size</summary>
    public void SetCapsuleSize(ushort? radius = null, ushort? halfHeight = null, ushort? crouchedHalfHeight = null) =>
        Call(SetCapsuleSizeSlot, 0, radius, halfHeight, crouchedHalfHeight);

    /// <summary>Sets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public void SetControlRotation(Rotator controlRotation) => Call(SetControlRotationSlot, 1, controlRotation);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Call(SetDamageMultiplierSlot, 2, boneName, multiplier);

    /// <summary>Changes the Death sound when Character dies</summary>
    public void SetDeathSound(string soundAsset) => Call(SetDeathSoundSlot, 1, soundAsset);

    /// <summary>Sets the Field of View multiplier</summary>
    public void SetFOVMultiplier(float multiplier) => Call(SetFOVMultiplierSlot, 1, multiplier);

    /// <summary>Set the Fall Damage multiplier taken when falling from High places</summary>
    public void SetFallDamageTaken(ushort damage) => Call(SetFallDamageTakenSlot, 1, damage);

    /// <summary>Enables or disables the Flying Mode, which removes gravity and allows moving in all directions</summary>
    public void SetFlyingMode(bool flyingMode) => Call(SetFlyingModeSlot, 1, flyingMode);

    /// <summary>Set the Footstep Volume multiplier</summary>
    public void SetFootstepVolumeMultiplier(float volumeMultiplier) =>
        Call(SetFootstepVolumeMultiplierSlot, 1, volumeMultiplier);

    /// <summary>Sets the Gait Mode (walking or sprinting)</summary>
    public void SetGaitMode(GaitMode gaitMode) => Call(SetGaitModeSlot, 1, gaitMode);

    /// <summary>Changes the Gravity Scale</summary>
    public void SetGravityScale(float gravityScale) => Call(SetGravityScaleSlot, 1, gravityScale);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Call(SetHealthSlot, 1, newHealth);

    /// <summary>Sets time to transition to HighFalling state</summary>
    public void SetHighFallingTime(float time) => Call(SetHighFallingTimeSlot, 1, time);

    /// <summary>Sets if the character will react to damage by applying small dynamic ragdoll effect to local bones damaged</summary>
    public void SetHitReactionEnabled(bool isEnabled) => Call(SetHitReactionEnabledSlot, 1, isEnabled);

    /// <summary>Set the Impact Damage taken when being run over by things</summary>
    public void SetImpactDamageTaken(ushort impactDamage) => Call(SetImpactDamageTakenSlot, 1, impactDamage);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

    /// <summary>Enables/Disables Character's Input</summary>
    public void SetInputEnabled(bool isEnabled) => Call(SetInputEnabledSlot, 1, isEnabled);

    /// <summary>Sets if the Character can receive any damage</summary>
    public void SetInvulnerable(bool isInvulnerable) => Call(SetInvulnerableSlot, 1, isInvulnerable);

    /// <summary>Sets the velocity of the jump</summary>
    public void SetJumpZVelocity(ushort jumpZVelocity) => Call(SetJumpZVelocitySlot, 1, jumpZVelocity);

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

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Call(SetMaxHealthSlot, 1, maxHealth);

    /// <summary>Changes the Character Mesh at runtime</summary>
    public void SetMesh(string skeletalMeshAsset) => Call(SetMeshSlot, 1, skeletalMeshAsset);

    /// <summary>Configures the mesh attachment settings and visibility</summary>
    public void SetMeshSettings(
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? isVisible = null) =>
        Call(SetMeshSettingsSlot, 0, relativeLocation, relativeRotation, isVisible);

    /// <summary>Sets the value of a Morph Target</summary>
    public void SetMorphTarget(string name, float value) => Call(SetMorphTargetSlot, 2, name, value);

    /// <summary>Enables or disables the movement of the Character</summary>
    public void SetMovementEnabled(bool enabled) => Call(SetMovementEnabledSlot, 1, enabled);

    /// <summary>Changes the Pain sound when Character takes damage</summary>
    public void SetPainSound(string soundAsset) => Call(SetPainSoundSlot, 1, soundAsset);

    /// <summary>Changes the texture of the Character's parachute</summary>
    public void SetParachuteTexture(string texture) => Call(SetParachuteTextureSlot, 1, texture);

    /// <summary>Applies the physical animation settings to the body given</summary>
    public void SetPhysicalAnimationSettings(
        string bone,
        bool includeSelf,
        bool isLocalSimulation,
        float? orientationStrength = null,
        float? angularVelocityStrength = null,
        float? positionStrength = null,
        float? velocityStrength = null,
        float? maxLinearForce = null,
        float? maxAngularForce = null) =>
        Invoke(SetPhysicalAnimationSettingsSlot, ScriptArguments.Trim([bone, includeSelf, isLocalSimulation, orientationStrength, angularVelocityStrength, positionStrength, velocityStrength, maxLinearForce, maxAngularForce], 3));

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Call(SetPhysicalMaterialSlot, 1, physicalMaterialPath);

    /// <summary>Sets the Physics Asset for the Character</summary>
    public void SetPhysicsAsset(string physicsAsset) => Call(SetPhysicsAssetSlot, 1, physicsAsset);

    /// <summary>Set the Punch Damage this Character will apply on others</summary>
    public void SetPunchDamage(ushort punchDamage) => Call(SetPunchDamageSlot, 1, punchDamage);

    /// <summary>Set the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public void SetRadialDamageToRagdoll(short damage) => Call(SetRadialDamageToRagdollSlot, 1, damage);

    /// <summary>Enables or disables the Ragdoll Mode, making the Character fall as a physics body</summary>
    public void SetRagdollMode(bool ragdollEnabled) => Call(SetRagdollModeSlot, 1, ragdollEnabled);

    /// <summary>Sets if the character will enter ragdoll mode when it dies. You can set it to false to apply custom Death animations</summary>
    public void SetRagdollOnDeathEnabled(bool isEnabled) => Call(SetRagdollOnDeathEnabledSlot, 1, isEnabled);

    /// <summary>Sets if the character will enter ragdoll mode when being hit by an object or falling from a high place</summary>
    public void SetRagdollOnHitEnabled(bool isEnabled) => Call(SetRagdollOnHitEnabledSlot, 1, isEnabled);

    /// <summary>Sets the cooldown time (in seconds) before this Character can stand up from ragdoll</summary>
    public void SetRagdollStandUpCooldown(float time) => Call(SetRagdollStandUpCooldownSlot, 1, time);

    /// <summary>Sets the Character Capsule to simulate physics</summary>
    public void SetSimulatePhysics(bool simulatePhysics) => Call(SetSimulatePhysicsSlot, 1, simulatePhysics);

    /// <summary>Sets the multiplier applied to all movement speeds</summary>
    public void SetSpeedMultiplier(float speedMultiplier) => Call(SetSpeedMultiplierSlot, 1, speedMultiplier);

    /// <summary>Sets the Stance Mode (standing, crouching or proning)</summary>
    public void SetStanceMode(StanceMode stanceMode) => Call(SetStanceModeSlot, 1, stanceMode);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Call(SetStaticMeshAttachedTransformSlot, 3, id, relativeLocation, relativeRotation);

    /// <summary>Sets a Team which will disable damaging same Team Members</summary>
    public void SetTeam(uint team) => Call(SetTeamSlot, 1, team);

    /// <summary>Sets the View Mode (first person, third person or top down)</summary>
    public void SetViewMode(ViewMode viewMode) => Call(SetViewModeSlot, 1, viewMode);

    /// <summary>Sets the Weapon's Aim Mode</summary>
    public void SetWeaponAimMode(AimMode aimMode) => Call(SetWeaponAimModeSlot, 1, aimMode);

    /// <summary>Sets if this Character is in Shooter Mode, in which a held Weapon is always aimed while idle or walking, is lowered while sprinting, and the aim input aims down sights instead of raising the Weapon</summary>
    public void SetWeaponShooterMode(bool weaponShooterMode) => Call(SetWeaponShooterModeSlot, 1, weaponShooterMode);

    /// <summary>Stops an Animation Montage on this character</summary>
    public void StopAnimation(string? animationAsset = null) => Call(StopAnimationSlot, 0, animationAsset);

    /// <summary>AI: Stops the movement, optionally zeroing the velocity as well</summary>
    public void StopMovement(bool? stopsVelocity = null) => Call(StopMovementSlot, 0, stopsVelocity);

    /// <summary>UnGrabs/Drops the Prop the Character is holding</summary>
    public void UnGrabProp() => Call(UnGrabPropSlot, 0);

    /// <summary>Unhides a bone of this Pawn</summary>
    public void UnHideBone(string boneName) => Call(UnHideBoneSlot, 1, boneName);

    /// <summary>Subscribes to the Attack event of this entity only.</summary>
    public ScriptSubscription SubscribeAttack(AttackHandler handler) => Subscribe("Attack", handler);

    /// <summary>Subscribes to the AttemptEnterVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptEnterVehicle(AttemptEnterVehicleHandler handler) => Subscribe("AttemptEnterVehicle", handler);

    /// <summary>Subscribes to the AttemptLeaveVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptLeaveVehicle(AttemptLeaveVehicleHandler handler) => Subscribe("AttemptLeaveVehicle", handler);

    /// <summary>Subscribes to the AttemptReload event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptReload(AttemptReloadHandler handler) => Subscribe("AttemptReload", handler);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Death event of this entity only.</summary>
    public ScriptSubscription SubscribeDeath(DeathHandler handler) => Subscribe("Death", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Drop event of this entity only.</summary>
    public ScriptSubscription SubscribeDrop(DropHandler handler) => Subscribe("Drop", handler);

    /// <summary>Subscribes to the EnterVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterVehicle(EnterVehicleHandler handler) => Subscribe("EnterVehicle", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the GrabProp event of this entity only.</summary>
    public ScriptSubscription SubscribeGrabProp(GrabPropHandler handler) => Subscribe("GrabProp", handler);

    /// <summary>Subscribes to the HealthChange event of this entity only.</summary>
    public ScriptSubscription SubscribeHealthChange(HealthChangeHandler handler) => Subscribe("HealthChange", handler);

    /// <summary>Subscribes to the Highlight event of this entity only.</summary>
    public ScriptSubscription SubscribeHighlight(HighlightHandler handler) => Subscribe("Highlight", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the LeaveVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveVehicle(LeaveVehicleHandler handler) => Subscribe("LeaveVehicle", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the MoveComplete event of this entity only.</summary>
    public ScriptSubscription SubscribeMoveComplete(MoveCompleteHandler handler) => Subscribe("MoveComplete", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the PickUp event of this entity only.</summary>
    public ScriptSubscription SubscribePickUp(PickUpHandler handler) => Subscribe("PickUp", handler);

    /// <summary>Subscribes to the Possess event of this entity only.</summary>
    public ScriptSubscription SubscribePossess(PossessHandler handler) => Subscribe("Possess", handler);

    /// <summary>Subscribes to the PullUse event of this entity only.</summary>
    public ScriptSubscription SubscribePullUse(PullUseHandler handler) => Subscribe("PullUse", handler);

    /// <summary>Subscribes to the Punch event of this entity only.</summary>
    public ScriptSubscription SubscribePunch(PunchHandler handler) => Subscribe("Punch", handler);

    /// <summary>Subscribes to the RagdollModeChange event of this entity only.</summary>
    public ScriptSubscription SubscribeRagdollModeChange(RagdollModeChangeHandler handler) => Subscribe("RagdollModeChange", handler);

    /// <summary>Subscribes to the ReleaseUse event of this entity only.</summary>
    public ScriptSubscription SubscribeReleaseUse(ReleaseUseHandler handler) => Subscribe("ReleaseUse", handler);

    /// <summary>Subscribes to the Reload event of this entity only.</summary>
    public ScriptSubscription SubscribeReload(ReloadHandler handler) => Subscribe("Reload", handler);

    /// <summary>Subscribes to the Respawn event of this entity only.</summary>
    public ScriptSubscription SubscribeRespawn(RespawnHandler handler) => Subscribe("Respawn", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

    /// <summary>Subscribes to the UnGrabProp event of this entity only.</summary>
    public ScriptSubscription SubscribeUnGrabProp(UnGrabPropHandler handler) => Subscribe("UnGrabProp", handler);

    /// <summary>Subscribes to the UnPossess event of this entity only.</summary>
    public ScriptSubscription SubscribeUnPossess(UnPossessHandler handler) => Subscribe("UnPossess", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Attack event of this entity when a subclass overrides it.</summary>
    [EventHook("Attack")]
    protected virtual void HandleAttack(Melee? melee)
    {
    }

    /// <summary>Called for the AttemptEnterVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptEnterVehicle")]
    protected virtual bool HandleAttemptEnterVehicle(Entity? vehicle, int seatIndex) => true;

    /// <summary>Called for the AttemptLeaveVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptLeaveVehicle")]
    protected virtual bool HandleAttemptLeaveVehicle(Entity? vehicle) => true;

    /// <summary>Called for the AttemptReload event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptReload")]
    protected virtual bool HandleAttemptReload(Weapon? weapon) => true;

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

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Drop event of this entity when a subclass overrides it.</summary>
    [EventHook("Drop")]
    protected virtual void HandleDrop(Entity? @object, bool triggeredByPlayer)
    {
    }

    /// <summary>Called for the EnterVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterVehicle")]
    protected virtual void HandleEnterVehicle(Entity? vehicle, int seatIndex)
    {
    }

    /// <summary>Called for the EnterWater event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterWater")]
    protected virtual void HandleEnterWater()
    {
    }

    /// <summary>Called for the GrabProp event of this entity when a subclass overrides it.</summary>
    [EventHook("GrabProp")]
    protected virtual void HandleGrabProp(Prop? prop)
    {
    }

    /// <summary>Called for the HealthChange event of this entity when a subclass overrides it.</summary>
    [EventHook("HealthChange")]
    protected virtual void HandleHealthChange(int oldHealth, int newHealth)
    {
    }

    /// <summary>Called for the Highlight event of this entity when a subclass overrides it.</summary>
    [EventHook("Highlight")]
    protected virtual void HandleHighlight(bool isHighlighted, Prop? @object)
    {
    }

    /// <summary>Called for the Interact event of this entity when a subclass overrides it.</summary>
    [EventHook("Interact")]
    protected virtual bool HandleInteract(Prop? @object) => true;

    /// <summary>Called for the LeaveVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveVehicle")]
    protected virtual void HandleLeaveVehicle(Entity? vehicle, int seatIndex)
    {
    }

    /// <summary>Called for the LeaveWater event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveWater")]
    protected virtual void HandleLeaveWater()
    {
    }

    /// <summary>Called for the MoveComplete event of this entity when a subclass overrides it.</summary>
    [EventHook("MoveComplete")]
    protected virtual void HandleMoveComplete(bool succeeded)
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Called for the PickUp event of this entity when a subclass overrides it.</summary>
    [EventHook("PickUp")]
    protected virtual void HandlePickUp(Entity? @object)
    {
    }

    /// <summary>Called for the Possess event of this entity when a subclass overrides it.</summary>
    [EventHook("Possess")]
    protected virtual void HandlePossess(Player? player)
    {
    }

    /// <summary>Called for the PullUse event of this entity when a subclass overrides it.</summary>
    [EventHook("PullUse")]
    protected virtual void HandlePullUse(Entity? pickable)
    {
    }

    /// <summary>Called for the Punch event of this entity when a subclass overrides it.</summary>
    [EventHook("Punch")]
    protected virtual void HandlePunch()
    {
    }

    /// <summary>Called for the RagdollModeChange event of this entity when a subclass overrides it.</summary>
    [EventHook("RagdollModeChange")]
    protected virtual void HandleRagdollModeChange(bool oldState, bool newState)
    {
    }

    /// <summary>Called for the ReleaseUse event of this entity when a subclass overrides it.</summary>
    [EventHook("ReleaseUse")]
    protected virtual void HandleReleaseUse(Entity? pickable)
    {
    }

    /// <summary>Called for the Reload event of this entity when a subclass overrides it.</summary>
    [EventHook("Reload")]
    protected virtual void HandleReload(Weapon? weapon, int ammoToReload)
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

    /// <summary>Called for the UnGrabProp event of this entity when a subclass overrides it.</summary>
    [EventHook("UnGrabProp")]
    protected virtual void HandleUnGrabProp(Prop? prop)
    {
    }

    /// <summary>Called for the UnPossess event of this entity when a subclass overrides it.</summary>
    [EventHook("UnPossess")]
    protected virtual void HandleUnPossess(Player? oldPlayer)
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when the Character effectively attacks with a Melee</summary>
    public delegate void AttackHandler(Character self, Melee? melee);

    /// <summary>Triggered when a Character attempts to enter a vehicle</summary>
    public delegate bool AttemptEnterVehicleHandler(Character self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when this Character attempts to leave a vehicle</summary>
    public delegate bool AttemptLeaveVehicleHandler(Character self, Entity? vehicle);

    /// <summary>Triggered when this Character attempts to reload</summary>
    public delegate bool AttemptReloadHandler(Character self, Weapon? weapon);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>When Entity Dies</summary>
    public delegate void DeathHandler(Character self, int lastDamageTaken, string? lastBoneDamaged, DamageType damageTypeReason, Vector hitFromDirection, Player? instigator, Actor? causer);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Character self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Character self, int oldDimension, int newDimension);

    /// <summary>When Character drops the currently picked up Pickable</summary>
    public delegate void DropHandler(Character self, Entity? @object, bool triggeredByPlayer);

    /// <summary>When Character enters a vehicle</summary>
    public delegate void EnterVehicleHandler(Character self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Character self);

    /// <summary>When Character grabs up a Prop</summary>
    public delegate void GrabPropHandler(Character self, Prop? prop);

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public delegate void HealthChangeHandler(Character self, int oldHealth, int newHealth);

    /// <summary>When Character highlights/looks at a Prop or a Pickable</summary>
    public delegate void HighlightHandler(Character self, bool isHighlighted, Prop? @object);

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
    public delegate bool InteractHandler(Character self, Prop? @object);

    /// <summary>When Character leaves a vehicle</summary>
    public delegate void LeaveVehicleHandler(Character self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Character self);

    /// <summary>Called when AI reaches its destination, or when it fails</summary>
    public delegate void MoveCompleteHandler(Character self, bool succeeded);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Character self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>When Character picks up anything</summary>
    public delegate void PickUpHandler(Character self, Entity? @object);

    /// <summary>When this Pawn is possessed by a Player</summary>
    public delegate void PossessHandler(Character self, Player? player);

    /// <summary>Triggered when a Character presses the use button for a Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public delegate void PullUseHandler(Character self, Entity? pickable);

    /// <summary>When Character punches</summary>
    public delegate void PunchHandler(Character self);

    /// <summary>When Character enters or leaves ragdoll</summary>
    public delegate void RagdollModeChangeHandler(Character self, bool oldState, bool newState);

    /// <summary>Triggered when a Character releases the use button for a Pickable (i.e. releases left mouse button with this equipped)</summary>
    public delegate void ReleaseUseHandler(Character self, Entity? pickable);

    /// <summary>When Character reloads a weapon</summary>
    public delegate void ReloadHandler(Character self, Weapon? weapon, int ammoToReload);

    /// <summary>When Entity Respawns</summary>
    public delegate void RespawnHandler(Character self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Character self);

    /// <summary>Triggered when this Entity takes damage</summary>
    public delegate void TakeDamageHandler(Character self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer);

    /// <summary>When Character drops a Prop</summary>
    public delegate void UnGrabPropHandler(Character self, Prop? prop);

    /// <summary>When this Pawn is unpossessed by a Player</summary>
    public delegate void UnPossessHandler(Character self, Player? oldPlayer);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Character self, string? key, object? value);
}
