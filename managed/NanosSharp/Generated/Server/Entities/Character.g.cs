#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Character : IDamageable, IPawn
{
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

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
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

    private static readonly ClassEvent<AttemptEnterVehicleHandler> AttemptEnterVehicleEvent = new("Character", "AttemptEnterVehicle");
    private static readonly ClassEvent<AttemptLeaveVehicleHandler> AttemptLeaveVehicleEvent = new("Character", "AttemptLeaveVehicle");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Character", "DimensionChange");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Character", "Interact");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Character", "NetworkAuthorityChange");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot DropSlot = new("Drop");
    private static readonly MethodSlot EnterVehicleSlot = new("EnterVehicle");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot GrabPropSlot = new("GrabProp");
    private static readonly MethodSlot LeaveVehicleSlot = new("LeaveVehicle");
    private static readonly MethodSlot LookAtSlot = new("LookAt");
    private static readonly MethodSlot PickUpSlot = new("PickUp");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
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
    private static readonly MethodSlot SetCanDeployParachuteSlot = new("SetCanDeployParachute");
    private static readonly MethodSlot SetCanDiveSlot = new("SetCanDive");
    private static readonly MethodSlot SetCanDropSlot = new("SetCanDrop");
    private static readonly MethodSlot SetCanGrabPropsSlot = new("SetCanGrabProps");
    private static readonly MethodSlot SetCanPickupPickablesSlot = new("SetCanPickupPickables");
    private static readonly MethodSlot SetCanPunchSlot = new("SetCanPunch");
    private static readonly MethodSlot SetCanSprintSlot = new("SetCanSprint");
    private static readonly MethodSlot SetCanUsePickablesSlot = new("SetCanUsePickables");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetDeathSoundSlot = new("SetDeathSound");
    private static readonly MethodSlot SetFOVMultiplierSlot = new("SetFOVMultiplier");
    private static readonly MethodSlot SetFallDamageTakenSlot = new("SetFallDamageTaken");
    private static readonly MethodSlot SetFootstepVolumeMultiplierSlot = new("SetFootstepVolumeMultiplier");
    private static readonly MethodSlot SetGaitModeSlot = new("SetGaitMode");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetHighFallingTimeSlot = new("SetHighFallingTime");
    private static readonly MethodSlot SetImpactDamageTakenSlot = new("SetImpactDamageTaken");
    private static readonly MethodSlot SetInputEnabledSlot = new("SetInputEnabled");
    private static readonly MethodSlot SetInvulnerableSlot = new("SetInvulnerable");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetMeshSlot = new("SetMesh");
    private static readonly MethodSlot SetMovementEnabledSlot = new("SetMovementEnabled");
    private static readonly MethodSlot SetPainSoundSlot = new("SetPainSound");
    private static readonly MethodSlot SetParachuteTextureSlot = new("SetParachuteTexture");
    private static readonly MethodSlot SetPhysicsAssetSlot = new("SetPhysicsAsset");
    private static readonly MethodSlot SetPunchDamageSlot = new("SetPunchDamage");
    private static readonly MethodSlot SetRagdollModeSlot = new("SetRagdollMode");
    private static readonly MethodSlot SetRagdollOnHitEnabledSlot = new("SetRagdollOnHitEnabled");
    private static readonly MethodSlot SetSimulatePhysicsSlot = new("SetSimulatePhysics");
    private static readonly MethodSlot SetSpeedMultiplierSlot = new("SetSpeedMultiplier");
    private static readonly MethodSlot SetStanceModeSlot = new("SetStanceMode");
    private static readonly MethodSlot SetWeaponShooterModeSlot = new("SetWeaponShooterMode");
    private static readonly MethodSlot UnGrabPropSlot = new("UnGrabProp");

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

    /// <summary>Gets the Fall Damage multiplier</summary>
    public ushort FallDamageTaken
    {
        get => GetFallDamageTaken();
        set => SetFallDamageTaken(value);
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

    /// <summary>Gets the damage dealt by this Character's punches</summary>
    public ushort PunchDamage
    {
        get => GetPunchDamage();
        set => SetPunchDamage(value);
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

    /// <summary>Gets if this Character is in Shooter Mode</summary>
    public bool WeaponShooterMode
    {
        get => GetWeaponShooterMode();
        set => SetWeaponShooterMode(value);
    }

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

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        CallResult<uint, uint, string?, DamageType?, Vector?, Player?, Entity?>(ApplyDamageSlot, 1, damage, boneName, damageType, fromDirection, instigator, causer)!;

    /// <summary>Drops any Pickable the Character is holding</summary>
    public void Drop() => Call(DropSlot, 0);

    /// <summary>Enters the Vehicle at Seat (0 - Driver)</summary>
    public void EnterVehicle(Entity vehicle, byte? seat = null) => Call(EnterVehicleSlot, 1, vehicle, seat);

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) =>
        CallResult<float, string>(GetDamageMultiplierSlot, 1, boneName)!;

    /// <summary>Gives a Prop to the Character</summary>
    public void GrabProp(Prop prop) => Call(GrabPropSlot, 1, prop);

    /// <summary>Leaves the current Vehicle</summary>
    public void LeaveVehicle() => Call(LeaveVehicleSlot, 0);

    /// <summary>AI: Tries to make this Character to look at Location</summary>
    public void LookAt(Vector location) => Call(LookAtSlot, 1, location);

    /// <summary>Gives a Melee/Grenade/Weapon (Pickable) to the Character</summary>
    public void PickUp(Entity pickable) => Call(PickUpSlot, 1, pickable);

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) => Call(RespawnSlot, 0, location, rotation);

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

    /// <summary>Sets if this Character is allowed to deploy the Parachute</summary>
    public void SetCanDeployParachute(bool canDeployParachute) =>
        Call(SetCanDeployParachuteSlot, 1, canDeployParachute);

    /// <summary>Sets if this Character is allowed to Dive</summary>
    public void SetCanDive(bool canDive) => Call(SetCanDiveSlot, 1, canDive);

    /// <summary>Sets if this Character is allowed to Drop the Picked up item</summary>
    public void SetCanDrop(bool canDrop) => Call(SetCanDropSlot, 1, canDrop);

    /// <summary>Sets if this Character is allowed to Grab any Prop</summary>
    public void SetCanGrabProps(bool canGrabProps) => Call(SetCanGrabPropsSlot, 1, canGrabProps);

    /// <summary>Sets if this Character is allowed to Pick up any Pickable (Weapon, Grenade, Melee...)</summary>
    public void SetCanPickupPickables(bool canPickup) => Call(SetCanPickupPickablesSlot, 1, canPickup);

    /// <summary>Sets if this Character is allowed to Punch</summary>
    public void SetCanPunch(bool canPunch) => Call(SetCanPunchSlot, 1, canPunch);

    /// <summary>Sets if this Character is allowed to Sprint</summary>
    public void SetCanSprint(bool canSprint) => Call(SetCanSprintSlot, 1, canSprint);

    /// <summary>Sets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public void SetCanUsePickables(bool canUse) => Call(SetCanUsePickablesSlot, 1, canUse);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Call(SetDamageMultiplierSlot, 2, boneName, multiplier);

    /// <summary>Changes the Death sound when Character dies</summary>
    public void SetDeathSound(string soundAsset) => Call(SetDeathSoundSlot, 1, soundAsset);

    /// <summary>Sets the Field of View multiplier</summary>
    public void SetFOVMultiplier(float multiplier) => Call(SetFOVMultiplierSlot, 1, multiplier);

    /// <summary>Set the Fall Damage multiplier taken when falling from High places</summary>
    public void SetFallDamageTaken(ushort damage) => Call(SetFallDamageTakenSlot, 1, damage);

    /// <summary>Set the Footstep Volume multiplier</summary>
    public void SetFootstepVolumeMultiplier(float volumeMultiplier) =>
        Call(SetFootstepVolumeMultiplierSlot, 1, volumeMultiplier);

    /// <summary>Sets the Gait Mode (walking or sprinting)</summary>
    public void SetGaitMode(GaitMode gaitMode) => Call(SetGaitModeSlot, 1, gaitMode);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Call(SetHealthSlot, 1, newHealth);

    /// <summary>Sets time to transition to HighFalling state</summary>
    public void SetHighFallingTime(float time) => Call(SetHighFallingTimeSlot, 1, time);

    /// <summary>Set the Impact Damage taken when being run over by things</summary>
    public void SetImpactDamageTaken(ushort impactDamage) => Call(SetImpactDamageTakenSlot, 1, impactDamage);

    /// <summary>Enables/Disables Character's Input</summary>
    public void SetInputEnabled(bool isEnabled) => Call(SetInputEnabledSlot, 1, isEnabled);

    /// <summary>Sets if the Character can receive any damage</summary>
    public void SetInvulnerable(bool isInvulnerable) => Call(SetInvulnerableSlot, 1, isInvulnerable);

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Call(SetMaxHealthSlot, 1, maxHealth);

    /// <summary>Changes the Character Mesh at runtime</summary>
    public void SetMesh(string skeletalMeshAsset) => Call(SetMeshSlot, 1, skeletalMeshAsset);

    /// <summary>Enables or disables the movement of the Character</summary>
    public void SetMovementEnabled(bool enabled) => Call(SetMovementEnabledSlot, 1, enabled);

    /// <summary>Changes the Pain sound when Character takes damage</summary>
    public void SetPainSound(string soundAsset) => Call(SetPainSoundSlot, 1, soundAsset);

    /// <summary>Changes the texture of the Character's parachute</summary>
    public void SetParachuteTexture(string texture) => Call(SetParachuteTextureSlot, 1, texture);

    /// <summary>Sets the Physics Asset for the Character</summary>
    public void SetPhysicsAsset(string physicsAsset) => Call(SetPhysicsAssetSlot, 1, physicsAsset);

    /// <summary>Set the Punch Damage this Character will apply on others</summary>
    public void SetPunchDamage(ushort punchDamage) => Call(SetPunchDamageSlot, 1, punchDamage);

    /// <summary>Enables or disables the Ragdoll Mode, making the Character fall as a physics body</summary>
    public void SetRagdollMode(bool ragdollEnabled) => Call(SetRagdollModeSlot, 1, ragdollEnabled);

    /// <summary>Sets if the character will enter ragdoll mode when being hit by an object or falling from a high place</summary>
    public void SetRagdollOnHitEnabled(bool isEnabled) => Call(SetRagdollOnHitEnabledSlot, 1, isEnabled);

    /// <summary>Sets the Character Capsule to simulate physics</summary>
    public void SetSimulatePhysics(bool simulatePhysics) => Call(SetSimulatePhysicsSlot, 1, simulatePhysics);

    /// <summary>Sets the multiplier applied to all movement speeds</summary>
    public void SetSpeedMultiplier(float speedMultiplier) => Call(SetSpeedMultiplierSlot, 1, speedMultiplier);

    /// <summary>Sets the Stance Mode (standing, crouching or proning)</summary>
    public void SetStanceMode(StanceMode stanceMode) => Call(SetStanceModeSlot, 1, stanceMode);

    /// <summary>Sets if this Character is in Shooter Mode, in which a held Weapon is always aimed while idle or walking, is lowered while sprinting, and the aim input aims down sights instead of raising the Weapon</summary>
    public void SetWeaponShooterMode(bool weaponShooterMode) => Call(SetWeaponShooterModeSlot, 1, weaponShooterMode);

    /// <summary>UnGrabs/Drops the Prop the Character is holding</summary>
    public void UnGrabProp() => Call(UnGrabPropSlot, 0);

    /// <summary>Subscribes to the AttemptEnterVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptEnterVehicle(AttemptEnterVehicleHandler handler) => Subscribe("AttemptEnterVehicle", handler);

    /// <summary>Subscribes to the AttemptLeaveVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptLeaveVehicle(AttemptLeaveVehicleHandler handler) => Subscribe("AttemptLeaveVehicle", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the AttemptEnterVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptEnterVehicle")]
    protected virtual bool HandleAttemptEnterVehicle(Entity? vehicle, int seatIndex) => true;

    /// <summary>Called for the AttemptLeaveVehicle event of this entity when a subclass overrides it.</summary>
    [EventHook("AttemptLeaveVehicle")]
    protected virtual bool HandleAttemptLeaveVehicle(Entity? vehicle) => true;

    /// <summary>Called for the DimensionChange event of this entity when a subclass overrides it.</summary>
    [EventHook("DimensionChange")]
    protected virtual void HandleDimensionChange(int oldDimension, int newDimension)
    {
    }

    /// <summary>Called for the Interact event of this entity when a subclass overrides it.</summary>
    [EventHook("Interact")]
    protected virtual bool HandleInteract(Prop? @object) => true;

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(Player? oldNetworkAuthority, Player? newNetworkAuthority)
    {
    }

    /// <summary>Triggered when a Character attempts to enter a vehicle</summary>
    public delegate bool AttemptEnterVehicleHandler(Character self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when this Character attempts to leave a vehicle</summary>
    public delegate bool AttemptLeaveVehicleHandler(Character self, Entity? vehicle);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Character self, int oldDimension, int newDimension);

    /// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
    public delegate bool InteractHandler(Character self, Prop? @object);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Character self, Player? oldNetworkAuthority, Player? newNetworkAuthority);
}
