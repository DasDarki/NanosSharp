#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Character animated with the Game Animation Sample Project locomotion.</summary>
[ScriptClass("CharacterGASP")]
public partial class CharacterGASP : Actor, IDamageable, IPaintable
{
    /// <summary>Triggered when the Character effectively attacks with a Melee</summary>
    public static event AttackHandler? OnAttack
    {
        add => AttackEvent.Add(value);
        remove => AttackEvent.Remove(value);
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

    private static readonly ClassEvent<AttackHandler> AttackEvent = new("CharacterGASP", "Attack");
    private static readonly ClassEvent<AttemptReloadHandler> AttemptReloadEvent = new("CharacterGASP", "AttemptReload");
    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("CharacterGASP", "ClassRegister");
    private static readonly ClassEvent<DeathHandler> DeathEvent = new("CharacterGASP", "Death");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("CharacterGASP", "Destroy");
    private static readonly ClassEvent<DropHandler> DropEvent = new("CharacterGASP", "Drop");
    private static readonly ClassEvent<EnterVehicleHandler> EnterVehicleEvent = new("CharacterGASP", "EnterVehicle");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("CharacterGASP", "EnterWater");
    private static readonly ClassEvent<GrabPropHandler> GrabPropEvent = new("CharacterGASP", "GrabProp");
    private static readonly ClassEvent<HealthChangeHandler> HealthChangeEvent = new("CharacterGASP", "HealthChange");
    private static readonly ClassEvent<HighlightHandler> HighlightEvent = new("CharacterGASP", "Highlight");
    private static readonly ClassEvent<LeaveVehicleHandler> LeaveVehicleEvent = new("CharacterGASP", "LeaveVehicle");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("CharacterGASP", "LeaveWater");
    private static readonly ClassEvent<MoveCompleteHandler> MoveCompleteEvent = new("CharacterGASP", "MoveComplete");
    private static readonly ClassEvent<PickUpHandler> PickUpEvent = new("CharacterGASP", "PickUp");
    private static readonly ClassEvent<PossessHandler> PossessEvent = new("CharacterGASP", "Possess");
    private static readonly ClassEvent<PullUseHandler> PullUseEvent = new("CharacterGASP", "PullUse");
    private static readonly ClassEvent<PunchHandler> PunchEvent = new("CharacterGASP", "Punch");
    private static readonly ClassEvent<RagdollModeChangeHandler> RagdollModeChangeEvent = new("CharacterGASP", "RagdollModeChange");
    private static readonly ClassEvent<ReleaseUseHandler> ReleaseUseEvent = new("CharacterGASP", "ReleaseUse");
    private static readonly ClassEvent<ReloadHandler> ReloadEvent = new("CharacterGASP", "Reload");
    private static readonly ClassEvent<RespawnHandler> RespawnEvent = new("CharacterGASP", "Respawn");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("CharacterGASP", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("CharacterGASP", "TakeDamage");
    private static readonly ClassEvent<UnGrabPropHandler> UnGrabPropEvent = new("CharacterGASP", "UnGrabProp");
    private static readonly ClassEvent<UnPossessHandler> UnPossessEvent = new("CharacterGASP", "UnPossess");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("CharacterGASP", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot FollowSlot = new("Follow");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetCanCrouchSlot = new("GetCanCrouch");
    private static readonly MethodSlot GetCanJumpSlot = new("GetCanJump");
    private static readonly MethodSlot GetCapsuleSizeSlot = new("GetCapsuleSize");
    private static readonly MethodSlot GetControlRotationSlot = new("GetControlRotation");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot GetFlyingModeSlot = new("GetFlyingMode");
    private static readonly MethodSlot GetGrabbedPropSlot = new("GetGrabbedProp");
    private static readonly MethodSlot GetGravityScaleSlot = new("GetGravityScale");
    private static readonly MethodSlot GetHealthSlot = new("GetHealth");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot GetJumpZVelocitySlot = new("GetJumpZVelocity");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMaxHealthSlot = new("GetMaxHealth");
    private static readonly MethodSlot GetMovingToSlot = new("GetMovingTo");
    private static readonly MethodSlot GetPlayerSlot = new("GetPlayer");
    private static readonly MethodSlot GetRadialDamageToRagdollSlot = new("GetRadialDamageToRagdoll");
    private static readonly MethodSlot GetTeamSlot = new("GetTeam");
    private static readonly MethodSlot GetVehicleSlot = new("GetVehicle");
    private static readonly MethodSlot GetVehicleSeatSlot = new("GetVehicleSeat");
    private static readonly MethodSlot HideBoneSlot = new("HideBone");
    private static readonly MethodSlot IsBoneHiddenSlot = new("IsBoneHidden");
    private static readonly MethodSlot IsDeadSlot = new("IsDead");
    private static readonly MethodSlot IsInvulnerableSlot = new("IsInvulnerable");
    private static readonly MethodSlot JumpSlot = new("Jump");
    private static readonly MethodSlot MoveToSlot = new("MoveTo");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
    private static readonly MethodSlot SetAIAvoidanceSettingsSlot = new("SetAIAvoidanceSettings");
    private static readonly MethodSlot SetCanCrouchSlot = new("SetCanCrouch");
    private static readonly MethodSlot SetCanJumpSlot = new("SetCanJump");
    private static readonly MethodSlot SetCapsuleSizeSlot = new("SetCapsuleSize");
    private static readonly MethodSlot SetControlRotationSlot = new("SetControlRotation");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetFlyingModeSlot = new("SetFlyingMode");
    private static readonly MethodSlot SetGravityScaleSlot = new("SetGravityScale");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetHitReactionEnabledSlot = new("SetHitReactionEnabled");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetInvulnerableSlot = new("SetInvulnerable");
    private static readonly MethodSlot SetJumpZVelocitySlot = new("SetJumpZVelocity");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetMeshSettingsSlot = new("SetMeshSettings");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetRadialDamageToRagdollSlot = new("SetRadialDamageToRagdoll");
    private static readonly MethodSlot SetRagdollOnDeathEnabledSlot = new("SetRagdollOnDeathEnabled");
    private static readonly MethodSlot SetRagdollStandUpCooldownSlot = new("SetRagdollStandUpCooldown");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot StopMovementSlot = new("StopMovement");
    private static readonly MethodSlot UnHideBoneSlot = new("UnHideBone");

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllSkeletalMeshAttached => GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> AllStaticMeshAttached => GetAllStaticMeshAttached();

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool CanCrouch
    {
        get => GetCanCrouch();
        set => SetCanCrouch(value);
    }

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool CanJump
    {
        get => GetCanJump();
        set => SetCanJump(value);
    }

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> CapsuleSize => GetCapsuleSize();

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator ControlRotation
    {
        get => GetControlRotation();
        set => SetControlRotation(value);
    }

    /// <summary>Gets if it's in Flying mode</summary>
    public bool FlyingMode
    {
        get => GetFlyingMode();
        set => SetFlyingMode(value);
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

    /// <summary>Gets the Moving To location</summary>
    public Vector MovingTo => GetMovingTo();

    /// <summary>Gets the possessing Player</summary>
    public Player? Player => GetPlayer();

    /// <summary>Gets the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public short RadialDamageToRagdoll
    {
        get => GetRadialDamageToRagdoll();
        set => SetRadialDamageToRagdoll(value);
    }

    /// <summary>Gets the entered Vehicle</summary>
    public object? Vehicle => GetVehicle();

    /// <summary>Gets the entered Vehicle seat</summary>
    public long VehicleSeat => GetVehicleSeat();

    /// <summary>Returns if the entity is dead</summary>
    public bool Dead => IsDead();

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool Invulnerable
    {
        get => IsInvulnerable();
        set => SetInvulnerable(value);
    }

    /// <summary>Spawns a new CharacterGASP.</summary>
    public CharacterGASP(
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
        SpawnSelf("CharacterGASP", ScriptArguments.Trim([location, rotation, skeletalMeshAsset, collisionType, gravityEnabled, maxHealth, deathSound, painSound, spawnMode], 3));
    }

    protected CharacterGASP()
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

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool GetCanCrouch() => CallResult<bool>(GetCanCrouchSlot, 0)!;

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool GetCanJump() => CallResult<bool>(GetCanJumpSlot, 0)!;

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> GetCapsuleSize() =>
        CallResult<IReadOnlyDictionary<string, float>>(GetCapsuleSizeSlot, 0)!;

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator GetControlRotation() => CallResult<Rotator>(GetControlRotationSlot, 0)!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) =>
        CallResult<float, string>(GetDamageMultiplierSlot, 1, boneName)!;

    /// <summary>Gets if it's in Flying mode</summary>
    public bool GetFlyingMode() => CallResult<bool>(GetFlyingModeSlot, 0)!;

    /// <summary>Gets the Grabbed Prop</summary>
    public Prop? GetGrabbedProp() => CallResult<Prop?>(GetGrabbedPropSlot, 0);

    /// <summary>Gets the gravity scale</summary>
    public float GetGravityScale() => CallResult<float>(GetGravityScaleSlot, 0)!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => CallResult<uint>(GetHealthSlot, 0)!;

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

    /// <summary>Gets the Moving To location</summary>
    public Vector GetMovingTo() => CallResult<Vector>(GetMovingToSlot, 0)!;

    /// <summary>Gets the possessing Player</summary>
    public Player? GetPlayer() => CallResult<Player?>(GetPlayerSlot, 0);

    /// <summary>Gets the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public short GetRadialDamageToRagdoll() => CallResult<short>(GetRadialDamageToRagdollSlot, 0)!;

    /// <summary>Gets the Team of this Character</summary>
    public uint GetTeam() => CallResult<uint>(GetTeamSlot, 0)!;

    /// <summary>Gets the entered Vehicle</summary>
    public object? GetVehicle() => CallResult<object?>(GetVehicleSlot, 0);

    /// <summary>Gets the entered Vehicle seat</summary>
    public long GetVehicleSeat() => CallResult<long>(GetVehicleSeatSlot, 0)!;

    /// <summary>Hides a bone of this Pawn</summary>
    public void HideBone(string boneName) => Call(HideBoneSlot, 1, boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => CallResult<bool, string>(IsBoneHiddenSlot, 1, boneName)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => CallResult<bool>(IsDeadSlot, 0)!;

    /// <summary>Gets if this Character is invulnerable</summary>
    public bool IsInvulnerable() => CallResult<bool>(IsInvulnerableSlot, 0)!;

    /// <summary>Triggers this Pawn to jump</summary>
    public void Jump() => Call(JumpSlot, 0);

    /// <summary>AI: Makes this Pawn walk to the Location</summary>
    public void MoveTo(Vector location, float? acceptanceRadius = null) =>
        Call(MoveToSlot, 1, location, acceptanceRadius);

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

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) => Call(RespawnSlot, 0, location, rotation);

    /// <summary>AI: Configures the RVO (Reciprocal Velocity Obstacles) Avoidance system used by the AIs when moving</summary>
    public void SetAIAvoidanceSettings(bool enabled, ushort? avoidanceConsiderationRadius = null) =>
        Call(SetAIAvoidanceSettingsSlot, 1, enabled, avoidanceConsiderationRadius);

    /// <summary>Sets if this Pawn is allowed to Crouch</summary>
    public void SetCanCrouch(bool canCrouch) => Call(SetCanCrouchSlot, 1, canCrouch);

    /// <summary>Sets if this Pawn is allowed to Jump</summary>
    public void SetCanJump(bool canJump) => Call(SetCanJumpSlot, 1, canJump);

    /// <summary>Sets this Pawn's Capsule size</summary>
    public void SetCapsuleSize(ushort? radius = null, ushort? halfHeight = null, ushort? crouchedHalfHeight = null) =>
        Call(SetCapsuleSizeSlot, 0, radius, halfHeight, crouchedHalfHeight);

    /// <summary>Sets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public void SetControlRotation(Rotator controlRotation) => Call(SetControlRotationSlot, 1, controlRotation);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Call(SetDamageMultiplierSlot, 2, boneName, multiplier);

    /// <summary>Enables or disables the Flying Mode, which removes gravity and allows moving in all directions</summary>
    public void SetFlyingMode(bool flyingMode) => Call(SetFlyingModeSlot, 1, flyingMode);

    /// <summary>Changes the Gravity Scale</summary>
    public void SetGravityScale(float gravityScale) => Call(SetGravityScaleSlot, 1, gravityScale);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Call(SetHealthSlot, 1, newHealth);

    /// <summary>Sets if the character will react to damage by applying small dynamic ragdoll effect to local bones damaged</summary>
    public void SetHitReactionEnabled(bool isEnabled) => Call(SetHitReactionEnabledSlot, 1, isEnabled);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

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

    /// <summary>Configures the mesh attachment settings and visibility</summary>
    public void SetMeshSettings(
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? isVisible = null) =>
        Call(SetMeshSettingsSlot, 0, relativeLocation, relativeRotation, isVisible);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Call(SetPhysicalMaterialSlot, 1, physicalMaterialPath);

    /// <summary>Set the minimum radial damage (e.g. explosions) taken to enter in ragdoll mode</summary>
    public void SetRadialDamageToRagdoll(short damage) => Call(SetRadialDamageToRagdollSlot, 1, damage);

    /// <summary>Sets if the character will enter ragdoll mode when it dies. You can set it to false to apply custom Death animations</summary>
    public void SetRagdollOnDeathEnabled(bool isEnabled) => Call(SetRagdollOnDeathEnabledSlot, 1, isEnabled);

    /// <summary>Sets the cooldown time (in seconds) before this Character can stand up from ragdoll</summary>
    public void SetRagdollStandUpCooldown(float time) => Call(SetRagdollStandUpCooldownSlot, 1, time);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Call(SetStaticMeshAttachedTransformSlot, 3, id, relativeLocation, relativeRotation);

    /// <summary>AI: Stops the movement, optionally zeroing the velocity as well</summary>
    public void StopMovement(bool? stopsVelocity = null) => Call(StopMovementSlot, 0, stopsVelocity);

    /// <summary>Unhides a bone of this Pawn</summary>
    public void UnHideBone(string boneName) => Call(UnHideBoneSlot, 1, boneName);

    /// <summary>Subscribes to the Attack event of this entity only.</summary>
    public ScriptSubscription SubscribeAttack(AttackHandler handler) => Subscribe("Attack", handler);

    /// <summary>Subscribes to the AttemptReload event of this entity only.</summary>
    public ScriptSubscription SubscribeAttemptReload(AttemptReloadHandler handler) => Subscribe("AttemptReload", handler);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Death event of this entity only.</summary>
    public ScriptSubscription SubscribeDeath(DeathHandler handler) => Subscribe("Death", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

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

    /// <summary>Subscribes to the LeaveVehicle event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveVehicle(LeaveVehicleHandler handler) => Subscribe("LeaveVehicle", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the MoveComplete event of this entity only.</summary>
    public ScriptSubscription SubscribeMoveComplete(MoveCompleteHandler handler) => Subscribe("MoveComplete", handler);

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
    public delegate void AttackHandler(CharacterGASP self, Melee? melee);

    /// <summary>Triggered when this Character attempts to reload</summary>
    public delegate bool AttemptReloadHandler(CharacterGASP self, Weapon? weapon);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>When Entity Dies</summary>
    public delegate void DeathHandler(CharacterGASP self, int lastDamageTaken, string? lastBoneDamaged, DamageType damageTypeReason, Vector hitFromDirection, Player? instigator, Actor? causer);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(CharacterGASP self);

    /// <summary>When Character drops the currently picked up Pickable</summary>
    public delegate void DropHandler(CharacterGASP self, Entity? @object, bool triggeredByPlayer);

    /// <summary>When Character enters a vehicle</summary>
    public delegate void EnterVehicleHandler(CharacterGASP self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(CharacterGASP self);

    /// <summary>When Character grabs up a Prop</summary>
    public delegate void GrabPropHandler(CharacterGASP self, Prop? prop);

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public delegate void HealthChangeHandler(CharacterGASP self, int oldHealth, int newHealth);

    /// <summary>When Character highlights/looks at a Prop or a Pickable</summary>
    public delegate void HighlightHandler(CharacterGASP self, bool isHighlighted, Prop? @object);

    /// <summary>When Character leaves a vehicle</summary>
    public delegate void LeaveVehicleHandler(CharacterGASP self, Entity? vehicle, int seatIndex);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(CharacterGASP self);

    /// <summary>Called when AI reaches its destination, or when it fails</summary>
    public delegate void MoveCompleteHandler(CharacterGASP self, bool succeeded);

    /// <summary>When Character picks up anything</summary>
    public delegate void PickUpHandler(CharacterGASP self, Entity? @object);

    /// <summary>When this Pawn is possessed by a Player</summary>
    public delegate void PossessHandler(CharacterGASP self, Player? player);

    /// <summary>Triggered when a Character presses the use button for a Pickable (i.e. clicks left mouse button with this equipped)</summary>
    public delegate void PullUseHandler(CharacterGASP self, Entity? pickable);

    /// <summary>When Character punches</summary>
    public delegate void PunchHandler(CharacterGASP self);

    /// <summary>When Character enters or leaves ragdoll</summary>
    public delegate void RagdollModeChangeHandler(CharacterGASP self, bool oldState, bool newState);

    /// <summary>Triggered when a Character releases the use button for a Pickable (i.e. releases left mouse button with this equipped)</summary>
    public delegate void ReleaseUseHandler(CharacterGASP self, Entity? pickable);

    /// <summary>When Character reloads a weapon</summary>
    public delegate void ReloadHandler(CharacterGASP self, Weapon? weapon, int ammoToReload);

    /// <summary>When Entity Respawns</summary>
    public delegate void RespawnHandler(CharacterGASP self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(CharacterGASP self);

    /// <summary>Triggered when this Entity takes damage</summary>
    public delegate void TakeDamageHandler(CharacterGASP self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer);

    /// <summary>When Character drops a Prop</summary>
    public delegate void UnGrabPropHandler(CharacterGASP self, Prop? prop);

    /// <summary>When this Pawn is unpossessed by a Player</summary>
    public delegate void UnPossessHandler(CharacterGASP self, Player? oldPlayer);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(CharacterGASP self, string? key, object? value);
}
