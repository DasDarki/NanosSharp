#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
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
        Invoke("AddSkeletalMeshAttached", ScriptArguments.Trim([id, skeletalMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, useBaseLeaderPoseComponent, animationPath, attachableId], 2));

    /// <summary>Attaches a Static Mesh to this entity</summary>
    public void AddStaticMeshAttached(
        string id,
        string staticMeshPath,
        string? socket = null,
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? useParentBounds = null,
        string? attachableId = null) =>
        Invoke("AddStaticMeshAttached", ScriptArguments.Trim([id, staticMeshPath, socket, relativeLocation, relativeRotation, useParentBounds, attachableId], 2));

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        Invoke<uint>("ApplyDamage", ScriptArguments.Trim([damage, boneName, damageType, fromDirection, instigator, causer], 1))!;

    /// <summary>Clear all Morph Target that are set to this Mesh</summary>
    public void ClearMorphTargets() => Invoke("ClearMorphTargets");

    /// <summary>Drops any Pickable the Character is holding</summary>
    public void Drop() => Invoke("Drop");

    /// <summary>Enters the Vehicle at Seat (0 - Driver)</summary>
    public void EnterVehicle(Entity vehicle, byte? seat = null) =>
        Invoke("EnterVehicle", ScriptArguments.Trim([vehicle, seat], 1));

    /// <summary>AI: Makes this Pawn follow another actor</summary>
    public void Follow(
        Entity actor,
        float? acceptanceRadius = null,
        bool? stopOnSucceed = null,
        bool? stopOnFail = null,
        float? updateRate = null) =>
        Invoke("Follow", ScriptArguments.Trim([actor, acceptanceRadius, stopOnSucceed, stopOnFail, updateRate], 1));

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>("GetAllSkeletalMeshAttached")!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>("GetAllStaticMeshAttached")!;

    /// <summary>Gets the camera mode</summary>
    public CameraMode GetCameraMode() => Invoke<CameraMode>("GetCameraMode")!;

    /// <summary>Gets if this Character is allowed to aim</summary>
    public bool GetCanAim() => Invoke<bool>("GetCanAim")!;

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool GetCanCrouch() => Invoke<bool>("GetCanCrouch")!;

    /// <summary>Gets if this Character is allowed to deploy the Parachute</summary>
    public bool GetCanDeployParachute() => Invoke<bool>("GetCanDeployParachute")!;

    /// <summary>Gets if this Character is allowed to Dive</summary>
    public bool GetCanDive() => Invoke<bool>("GetCanDive")!;

    /// <summary>Gets if this Character is allowed to drop the picked up item</summary>
    public bool GetCanDrop() => Invoke<bool>("GetCanDrop")!;

    /// <summary>Gets if this Character is allowed to grab Props</summary>
    public bool GetCanGrabProps() => Invoke<bool>("GetCanGrabProps")!;

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool GetCanJump() => Invoke<bool>("GetCanJump")!;

    /// <summary>Gets if this Character is allowed to pick up Pickables (Weapon, Grenade, Melee...)</summary>
    public bool GetCanPickupPickables() => Invoke<bool>("GetCanPickupPickables")!;

    /// <summary>Gets if this Character is allowed to punch</summary>
    public bool GetCanPunch() => Invoke<bool>("GetCanPunch")!;

    /// <summary>Gets if this Character is allowed to sprint</summary>
    public bool GetCanSprint() => Invoke<bool>("GetCanSprint")!;

    /// <summary>Gets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public bool GetCanUsePickables() => Invoke<bool>("GetCanUsePickables")!;

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> GetCapsuleSize() =>
        Invoke<IReadOnlyDictionary<string, float>>("GetCapsuleSize")!;

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator GetControlRotation() => Invoke<Rotator>("GetControlRotation")!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) => Invoke<float>("GetDamageMultiplier", boneName)!;

    /// <summary>Gets the Fall Damage multiplier</summary>
    public ushort GetFallDamageTaken() => Invoke<ushort>("GetFallDamageTaken")!;

    /// <summary>Gets the current Falling Mode</summary>
    public FallingMode GetFallingMode() => Invoke<FallingMode>("GetFallingMode")!;

    /// <summary>Gets if it's in Flying mode</summary>
    public bool GetFlyingMode() => Invoke<bool>("GetFlyingMode")!;

    /// <summary>Gets the Footstep Volume multiplier</summary>
    public float GetFootstepVolumeMultiplier() => Invoke<float>("GetFootstepVolumeMultiplier")!;

    /// <summary>Gets the current Gait Mode</summary>
    public GaitMode GetGaitMode() => Invoke<GaitMode>("GetGaitMode")!;

    /// <summary>Gets the Grabbed Prop</summary>
    public Prop? GetGrabbedProp() => Invoke<Prop?>("GetGrabbedProp");

    /// <summary>Gets the gravity scale</summary>
    public float GetGravityScale() => Invoke<float>("GetGravityScale")!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => Invoke<uint>("GetHealth")!;

    /// <summary>Gets the Impact Damage taken when being run over</summary>
    public ushort GetImpactDamageTaken() => Invoke<ushort>("GetImpactDamageTaken")!;

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    public ushort GetJumpZVelocity() => Invoke<ushort>("GetJumpZVelocity")!;

    /// <summary>Gets a Color parameter from this Actor's material</summary>
    public Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Color?>("GetMaterialColorParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    public float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<float?>("GetMaterialScalarParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    public string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<string?>("GetMaterialTextureParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    public Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        Invoke<Vector?>("GetMaterialVectorParameter", ScriptArguments.Trim([parameterName, index, attachableId], 1));

    /// <summary>Gets the Max Health</summary>
    public uint GetMaxHealth() => Invoke<uint>("GetMaxHealth")!;

    /// <summary>Gets the base Mesh Asset</summary>
    public string GetMesh() => Invoke<string>("GetMesh")!;

    /// <summary>Returns the value of a Morph Target</summary>
    public float GetMorphTarget(string name) => Invoke<float>("GetMorphTarget", name)!;

    /// <summary>Gets the Moving To location</summary>
    public Vector GetMovingTo() => Invoke<Vector>("GetMovingTo")!;

    /// <summary>Gets the Pickable if picking up</summary>
    public object? GetPicked() => Invoke<object?>("GetPicked");

    /// <summary>Gets the possessing Player</summary>
    public Player? GetPlayer() => Invoke<Player?>("GetPlayer");

    /// <summary>Gets the damage dealt by this Character's punches</summary>
    public ushort GetPunchDamage() => Invoke<ushort>("GetPunchDamage")!;

    /// <summary>Gets the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public short GetRadialDamageToRagdoll() => Invoke<short>("GetRadialDamageToRagdoll")!;

    /// <summary>Gets the movement speed multiplier</summary>
    public float GetSpeedMultiplier() => Invoke<float>("GetSpeedMultiplier")!;

    /// <summary>Gets the Stance Mode</summary>
    public StanceMode GetStanceMode() => Invoke<StanceMode>("GetStanceMode")!;

    /// <summary>Gets the Swimming Mode</summary>
    public SwimmingMode GetSwimmingMode() => Invoke<SwimmingMode>("GetSwimmingMode")!;

    /// <summary>Gets the Team of this Character</summary>
    public uint GetTeam() => Invoke<uint>("GetTeam")!;

    /// <summary>Gets the entered Vehicle</summary>
    public object? GetVehicle() => Invoke<object?>("GetVehicle");

    /// <summary>Gets the entered Vehicle seat</summary>
    public long GetVehicleSeat() => Invoke<long>("GetVehicleSeat")!;

    /// <summary>Gets the View Mode</summary>
    public ViewMode GetViewMode() => Invoke<ViewMode>("GetViewMode")!;

    /// <summary>Gets the Weapon Aim Mode</summary>
    public AimMode GetWeaponAimMode() => Invoke<AimMode>("GetWeaponAimMode")!;

    /// <summary>Gets if this Character is in Shooter Mode</summary>
    public bool GetWeaponShooterMode() => Invoke<bool>("GetWeaponShooterMode")!;

    /// <summary>Gives a Prop to the Character</summary>
    public void GrabProp(Prop prop) => Invoke("GrabProp", prop);

    /// <summary>Hides a bone of this Pawn</summary>
    public void HideBone(string boneName) => Invoke("HideBone", boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => Invoke<bool>("IsBoneHidden", boneName)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => Invoke<bool>("IsDead")!;

    /// <summary>Gets if Character is in ragdoll mode</summary>
    public bool IsInRagdollMode() => Invoke<bool>("IsInRagdollMode")!;

    /// <summary>Gets if this Character has input enabled</summary>
    public bool IsInputEnabled() => Invoke<bool>("IsInputEnabled")!;

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool IsInvulnerable() => Invoke<bool>("IsInvulnerable")!;

    /// <summary>Triggers this Pawn to jump</summary>
    public void Jump() => Invoke("Jump");

    /// <summary>Leaves the current Vehicle</summary>
    public void LeaveVehicle() => Invoke("LeaveVehicle");

    /// <summary>AI: Tries to make this Character to look at Location</summary>
    public void LookAt(Vector location) => Invoke("LookAt", location);

    /// <summary>AI: Makes this Pawn walk to the Location</summary>
    public void MoveTo(Vector location, float? acceptanceRadius = null) =>
        Invoke("MoveTo", ScriptArguments.Trim([location, acceptanceRadius], 1));

    /// <summary>Gives a Melee/Grenade/Weapon (Pickable) to the Character</summary>
    public void PickUp(Entity pickable) => Invoke("PickUp", pickable);

    /// <summary>Plays an Animation Montage on this character</summary>
    public void PlayAnimation(
        string animationPath,
        AnimationSlotType? slotType = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Invoke("PlayAnimation", ScriptArguments.Trim([animationPath, slotType, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages], 1));

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Invoke("RemoveAllSkeletalMeshesAttached");

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Invoke("RemoveAllStaticMeshesAttached");

    /// <summary>Removes, if it exists, a SkeletalMesh from this entity given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Invoke("RemoveSkeletalMeshAttached", id);

    /// <summary>Removes, if it exists, a StaticMesh from this entity given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Invoke("RemoveStaticMeshAttached", id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke("ResetMaterial", ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Resets all Physical Animation settings</summary>
    public void ResetPhysicalAnimationSettings() => Invoke("ResetPhysicalAnimationSettings");

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) =>
        Invoke("Respawn", ScriptArguments.Trim([location, rotation], 0));

    /// <summary>AI: Configures the RVO (Reciprocal Velocity Obstacles) Avoidance system used by the AIs when moving</summary>
    public void SetAIAvoidanceSettings(bool enabled, ushort? avoidanceConsiderationRadius = null) =>
        Invoke("SetAIAvoidanceSettings", ScriptArguments.Trim([enabled, avoidanceConsiderationRadius], 1));

    /// <summary>Sets the Movement Max Acceleration of this Character</summary>
    public void SetAccelerationSettings(
        ushort? walking = null,
        ushort? parachuting = null,
        ushort? skydiving = null,
        ushort? falling = null,
        ushort? swimming = null,
        ushort? swimmingSurface = null,
        ushort? flying = null) =>
        Invoke("SetAccelerationSettings", ScriptArguments.Trim([walking, parachuting, skydiving, falling, swimming, swimmingSurface, flying], 0));

    /// <summary>Sets the amount of movement control allowed when it is in air</summary>
    public void SetAirControl(
        float? airControl = null,
        float? boostMultiplier = null,
        float? boostVelocityThreshold = null) =>
        Invoke("SetAirControl", ScriptArguments.Trim([airControl, boostMultiplier, boostVelocityThreshold], 0));

    /// <summary>Sets the Blend Space (2D) Animation for CrouchingHorizontal Axis stands for Speed X and Vertical Axis for Speed Y</summary>
    public void SetAnimationIdleWalkRunCrouching(string blendSpacePath, bool? enableTurnInPlace = null) =>
        Invoke("SetAnimationIdleWalkRunCrouching", ScriptArguments.Trim([blendSpacePath, enableTurnInPlace], 1));

    /// <summary>Sets the Blend Space (2D) Animation for ProningHorizontal Axis stands for Speed X and Vertical Axis for Speed Y</summary>
    public void SetAnimationIdleWalkRunProning(string blendSpacePath) =>
        Invoke("SetAnimationIdleWalkRunProning", blendSpacePath);

    /// <summary>Sets the Blend Space (2D) Animation for Standing</summary>
    public void SetAnimationIdleWalkRunStanding(string blendSpacePath, bool? enableTurnInPlace = null) =>
        Invoke("SetAnimationIdleWalkRunStanding", ScriptArguments.Trim([blendSpacePath, enableTurnInPlace], 1));

    /// <summary>Sets the Transition Animation between Crouching and Proning</summary>
    public void SetAnimationsTransitionCrouchingProning(string crouchingToProning, string proningToCrouching) =>
        Invoke("SetAnimationsTransitionCrouchingProning", crouchingToProning, proningToCrouching);

    /// <summary>Sets the Transition Animation between Standing and Crouching</summary>
    public void SetAnimationsTransitionStandingCrouching(string standingToCrouching, string crouchingToStanding) =>
        Invoke("SetAnimationsTransitionStandingCrouching", standingToCrouching, crouchingToStanding);

    /// <summary>Sets the Movement Braking Settings of this Pawn</summary>
    public void SetBrakingSettings(
        float? groundFriction = null,
        float? brakingFrictionFactor = null,
        ushort? brakingWalking = null,
        ushort? brakingFlying = null,
        ushort? brakingSwimming = null,
        ushort? brakingFalling = null) =>
        Invoke("SetBrakingSettings", ScriptArguments.Trim([groundFriction, brakingFrictionFactor, brakingWalking, brakingFlying, brakingSwimming, brakingFalling], 0));

    /// <summary>Sets the Camera Mode</summary>
    public void SetCameraMode(CameraMode cameraMode) => Invoke("SetCameraMode", cameraMode);

    /// <summary>Sets if this Character is allowed to Aim</summary>
    public void SetCanAim(bool canAim) => Invoke("SetCanAim", canAim);

    /// <summary>Sets if this Pawn is allowed to Crouch</summary>
    public void SetCanCrouch(bool canCrouch) => Invoke("SetCanCrouch", canCrouch);

    /// <summary>Sets if this Character is allowed to deploy the Parachute</summary>
    public void SetCanDeployParachute(bool canDeployParachute) => Invoke("SetCanDeployParachute", canDeployParachute);

    /// <summary>Sets if this Character is allowed to Dive</summary>
    public void SetCanDive(bool canDive) => Invoke("SetCanDive", canDive);

    /// <summary>Sets if this Character is allowed to Drop the Picked up item</summary>
    public void SetCanDrop(bool canDrop) => Invoke("SetCanDrop", canDrop);

    /// <summary>Sets if this Character is allowed to Grab any Prop</summary>
    public void SetCanGrabProps(bool canGrabProps) => Invoke("SetCanGrabProps", canGrabProps);

    /// <summary>Sets if this Pawn is allowed to Jump</summary>
    public void SetCanJump(bool canJump) => Invoke("SetCanJump", canJump);

    /// <summary>Sets if this Character is allowed to Pick up any Pickable (Weapon, Grenade, Melee...)</summary>
    public void SetCanPickupPickables(bool canPickup) => Invoke("SetCanPickupPickables", canPickup);

    /// <summary>Sets if this Character is allowed to Punch</summary>
    public void SetCanPunch(bool canPunch) => Invoke("SetCanPunch", canPunch);

    /// <summary>Sets if this Character is allowed to Sprint</summary>
    public void SetCanSprint(bool canSprint) => Invoke("SetCanSprint", canSprint);

    /// <summary>Sets if this Character is allowed to use Pickables (Weapon, Grenade, Melee...)</summary>
    public void SetCanUsePickables(bool canUse) => Invoke("SetCanUsePickables", canUse);

    /// <summary>Sets this Pawn's Capsule size</summary>
    public void SetCapsuleSize(ushort? radius = null, ushort? halfHeight = null, ushort? crouchedHalfHeight = null) =>
        Invoke("SetCapsuleSize", ScriptArguments.Trim([radius, halfHeight, crouchedHalfHeight], 0));

    /// <summary>Sets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public void SetControlRotation(Rotator controlRotation) => Invoke("SetControlRotation", controlRotation);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Invoke("SetDamageMultiplier", boneName, multiplier);

    /// <summary>Changes the Death sound when Character dies</summary>
    public void SetDeathSound(string soundAsset) => Invoke("SetDeathSound", soundAsset);

    /// <summary>Sets the Field of View multiplier</summary>
    public void SetFOVMultiplier(float multiplier) => Invoke("SetFOVMultiplier", multiplier);

    /// <summary>Set the Fall Damage multiplier taken when falling from High places</summary>
    public void SetFallDamageTaken(ushort damage) => Invoke("SetFallDamageTaken", damage);

    /// <summary>Enables or disables the Flying Mode, which removes gravity and allows moving in all directions</summary>
    public void SetFlyingMode(bool flyingMode) => Invoke("SetFlyingMode", flyingMode);

    /// <summary>Set the Footstep Volume multiplier</summary>
    public void SetFootstepVolumeMultiplier(float volumeMultiplier) =>
        Invoke("SetFootstepVolumeMultiplier", volumeMultiplier);

    /// <summary>Sets the Gait Mode (walking or sprinting)</summary>
    public void SetGaitMode(GaitMode gaitMode) => Invoke("SetGaitMode", gaitMode);

    /// <summary>Changes the Gravity Scale</summary>
    public void SetGravityScale(float gravityScale) => Invoke("SetGravityScale", gravityScale);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Invoke("SetHealth", newHealth);

    /// <summary>Sets time to transition to HighFalling state</summary>
    public void SetHighFallingTime(float time) => Invoke("SetHighFallingTime", time);

    /// <summary>Sets if the character will react to damage by applying small dynamic ragdoll effect to local bones damaged</summary>
    public void SetHitReactionEnabled(bool isEnabled) => Invoke("SetHitReactionEnabled", isEnabled);

    /// <summary>Set the Impact Damage taken when being run over by things</summary>
    public void SetImpactDamageTaken(ushort impactDamage) => Invoke("SetImpactDamageTaken", impactDamage);

    /// <summary>Enables/Disables Character's Input</summary>
    public void SetInputEnabled(bool isEnabled) => Invoke("SetInputEnabled", isEnabled);

    /// <summary>Sets if the Character can receive any damage</summary>
    public void SetInvulnerable(bool isInvulnerable) => Invoke("SetInvulnerable", isInvulnerable);

    /// <summary>Sets the velocity of the jump</summary>
    public void SetJumpZVelocity(ushort jumpZVelocity) => Invoke("SetJumpZVelocity", jumpZVelocity);

    /// <summary>Sets the material at the specified index of this Actor</summary>
    public void SetMaterial(string materialPath, int? index = null, string? attachableId = null) =>
        Invoke("SetMaterial", ScriptArguments.Trim([materialPath, index, attachableId], 1));

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    public void SetMaterialColorParameter(
        string parameterName,
        Color color,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialColorParameter", ScriptArguments.Trim([parameterName, color, index, attachableId], 2));

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    public void SetMaterialScalarParameter(
        string parameterName,
        float scalar,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialScalarParameter", ScriptArguments.Trim([parameterName, scalar, index, attachableId], 2));

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    public void SetMaterialTextureParameter(
        string parameterName,
        string texturePath,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialTextureParameter", ScriptArguments.Trim([parameterName, texturePath, index, attachableId], 2));

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    public void SetMaterialVectorParameter(
        string parameterName,
        Vector vector,
        int? index = null,
        string? attachableId = null) =>
        Invoke("SetMaterialVectorParameter", ScriptArguments.Trim([parameterName, vector, index, attachableId], 2));

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Invoke("SetMaxHealth", maxHealth);

    /// <summary>Changes the Character Mesh at runtime</summary>
    public void SetMesh(string skeletalMeshAsset) => Invoke("SetMesh", skeletalMeshAsset);

    /// <summary>Configures the mesh attachment settings and visibility</summary>
    public void SetMeshSettings(
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? isVisible = null) =>
        Invoke("SetMeshSettings", ScriptArguments.Trim([relativeLocation, relativeRotation, isVisible], 0));

    /// <summary>Sets the value of a Morph Target</summary>
    public void SetMorphTarget(string name, float value) => Invoke("SetMorphTarget", name, value);

    /// <summary>Changes the Pain sound when Character takes damage</summary>
    public void SetPainSound(string soundAsset) => Invoke("SetPainSound", soundAsset);

    /// <summary>Changes the texture of the Character's parachute</summary>
    public void SetParachuteTexture(string texture) => Invoke("SetParachuteTexture", texture);

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
        Invoke("SetPhysicalAnimationSettings", ScriptArguments.Trim([bone, includeSelf, isLocalSimulation, orientationStrength, angularVelocityStrength, positionStrength, velocityStrength, maxLinearForce, maxAngularForce], 3));

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) => Invoke("SetPhysicalMaterial", physicalMaterialPath);

    /// <summary>Sets the Physics Asset for the Character</summary>
    public void SetPhysicsAsset(string physicsAsset) => Invoke("SetPhysicsAsset", physicsAsset);

    /// <summary>Set the Punch Damage this Character will apply on others</summary>
    public void SetPunchDamage(ushort punchDamage) => Invoke("SetPunchDamage", punchDamage);

    /// <summary>Set the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public void SetRadialDamageToRagdoll(short damage) => Invoke("SetRadialDamageToRagdoll", damage);

    /// <summary>Enables or disables the Ragdoll Mode, making the Character fall as a physics body</summary>
    public void SetRagdollMode(bool ragdollEnabled) => Invoke("SetRagdollMode", ragdollEnabled);

    /// <summary>Sets if the character will enter ragdoll mode when it dies. You can set it to false to apply custom Death animations</summary>
    public void SetRagdollOnDeathEnabled(bool isEnabled) => Invoke("SetRagdollOnDeathEnabled", isEnabled);

    /// <summary>Sets if the character will enter ragdoll mode when being hit by an object or falling from a high place</summary>
    public void SetRagdollOnHitEnabled(bool isEnabled) => Invoke("SetRagdollOnHitEnabled", isEnabled);

    /// <summary>Sets the cooldown time (in seconds) before this Character can stand up from ragdoll</summary>
    public void SetRagdollStandUpCooldown(float time) => Invoke("SetRagdollStandUpCooldown", time);

    /// <summary>Sets the Character Capsule to simulate physics</summary>
    public void SetSimulatePhysics(bool simulatePhysics) => Invoke("SetSimulatePhysics", simulatePhysics);

    /// <summary>Sets the multiplier applied to all movement speeds</summary>
    public void SetSpeedMultiplier(float speedMultiplier) => Invoke("SetSpeedMultiplier", speedMultiplier);

    /// <summary>Sets the Stance Mode (standing, crouching or proning)</summary>
    public void SetStanceMode(StanceMode stanceMode) => Invoke("SetStanceMode", stanceMode);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke("SetStaticMeshAttachedTransform", id, relativeLocation, relativeRotation);

    /// <summary>Sets a Team which will disable damaging same Team Members</summary>
    public void SetTeam(uint team) => Invoke("SetTeam", team);

    /// <summary>Sets the View Mode (first person, third person or top down)</summary>
    public void SetViewMode(ViewMode viewMode) => Invoke("SetViewMode", viewMode);

    /// <summary>Sets the Weapon's Aim Mode</summary>
    public void SetWeaponAimMode(AimMode aimMode) => Invoke("SetWeaponAimMode", aimMode);

    /// <summary>Sets if this Character is in Shooter Mode, in which a held Weapon is always aimed while idle or walking, is lowered while sprinting, and the aim input aims down sights instead of raising the Weapon</summary>
    public void SetWeaponShooterMode(bool weaponShooterMode) => Invoke("SetWeaponShooterMode", weaponShooterMode);

    /// <summary>Stops an Animation Montage on this character</summary>
    public void StopAnimation(string? animationAsset = null) =>
        Invoke("StopAnimation", ScriptArguments.Trim([animationAsset], 0));

    /// <summary>AI: Stops the movement, optionally zeroing the velocity as well</summary>
    public void StopMovement(bool? stopsVelocity = null) =>
        Invoke("StopMovement", ScriptArguments.Trim([stopsVelocity], 0));

    /// <summary>UnGrabs/Drops the Prop the Character is holding</summary>
    public void UnGrabProp() => Invoke("UnGrabProp");

    /// <summary>Unhides a bone of this Pawn</summary>
    public void UnHideBone(string boneName) => Invoke("UnHideBone", boneName);

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
