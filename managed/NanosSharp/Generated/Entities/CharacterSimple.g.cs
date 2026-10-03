#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>CharacterSimple is a simpler Character implementation with basic Movement implementation. Aimed for custom NPCs or basic Pawns.</summary>
[ScriptClass("CharacterSimple")]
public partial class CharacterSimple : Actor, IDamageable, IPaintable, IPawn
{
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

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public static event HealthChangeHandler? OnHealthChange
    {
        add => HealthChangeEvent.Add(value);
        remove => HealthChangeEvent.Remove(value);
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

    /// <summary>When this Pawn is possessed by a Player</summary>
    public static event PossessHandler? OnPossess
    {
        add => PossessEvent.Add(value);
        remove => PossessEvent.Remove(value);
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

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("CharacterSimple", "ClassRegister");
    private static readonly ClassEvent<DeathHandler> DeathEvent = new("CharacterSimple", "Death");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("CharacterSimple", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("CharacterSimple", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("CharacterSimple", "EnterWater");
    private static readonly ClassEvent<HealthChangeHandler> HealthChangeEvent = new("CharacterSimple", "HealthChange");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("CharacterSimple", "LeaveWater");
    private static readonly ClassEvent<MoveCompleteHandler> MoveCompleteEvent = new("CharacterSimple", "MoveComplete");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("CharacterSimple", "NetworkAuthorityChange");
    private static readonly ClassEvent<PossessHandler> PossessEvent = new("CharacterSimple", "Possess");
    private static readonly ClassEvent<RespawnHandler> RespawnEvent = new("CharacterSimple", "Respawn");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("CharacterSimple", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("CharacterSimple", "TakeDamage");
    private static readonly ClassEvent<UnPossessHandler> UnPossessEvent = new("CharacterSimple", "UnPossess");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("CharacterSimple", "ValueChange");
    private static readonly MethodSlot AddSkeletalMeshAttachedSlot = new("AddSkeletalMeshAttached");
    private static readonly MethodSlot AddStaticMeshAttachedSlot = new("AddStaticMeshAttached");
    private static readonly MethodSlot ApplyDamageSlot = new("ApplyDamage");
    private static readonly MethodSlot BindAnimationBlueprintEventDispatcherSlot = new("BindAnimationBlueprintEventDispatcher");
    private static readonly MethodSlot CallAnimationBlueprintEventSlot = new("CallAnimationBlueprintEvent");
    private static readonly MethodSlot FollowSlot = new("Follow");
    private static readonly MethodSlot GetAllSkeletalMeshAttachedSlot = new("GetAllSkeletalMeshAttached");
    private static readonly MethodSlot GetAllStaticMeshAttachedSlot = new("GetAllStaticMeshAttached");
    private static readonly MethodSlot GetAnimationBlueprintPropertyValueSlot = new("GetAnimationBlueprintPropertyValue");
    private static readonly MethodSlot GetCanCrouchSlot = new("GetCanCrouch");
    private static readonly MethodSlot GetCanJumpSlot = new("GetCanJump");
    private static readonly MethodSlot GetCapsuleSizeSlot = new("GetCapsuleSize");
    private static readonly MethodSlot GetControlRotationSlot = new("GetControlRotation");
    private static readonly MethodSlot GetDamageMultiplierSlot = new("GetDamageMultiplier");
    private static readonly MethodSlot GetFlyingModeSlot = new("GetFlyingMode");
    private static readonly MethodSlot GetGravityScaleSlot = new("GetGravityScale");
    private static readonly MethodSlot GetHealthSlot = new("GetHealth");
    private static readonly MethodSlot GetJumpZVelocitySlot = new("GetJumpZVelocity");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMaxHealthSlot = new("GetMaxHealth");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot GetMovingToSlot = new("GetMovingTo");
    private static readonly MethodSlot GetPlayerSlot = new("GetPlayer");
    private static readonly MethodSlot HideBoneSlot = new("HideBone");
    private static readonly MethodSlot IsBoneHiddenSlot = new("IsBoneHidden");
    private static readonly MethodSlot IsCrouchingSlot = new("IsCrouching");
    private static readonly MethodSlot IsDeadSlot = new("IsDead");
    private static readonly MethodSlot JumpSlot = new("Jump");
    private static readonly MethodSlot MoveToSlot = new("MoveTo");
    private static readonly MethodSlot PlayAnimationSlot = new("PlayAnimation");
    private static readonly MethodSlot RemoveAllSkeletalMeshesAttachedSlot = new("RemoveAllSkeletalMeshesAttached");
    private static readonly MethodSlot RemoveAllStaticMeshesAttachedSlot = new("RemoveAllStaticMeshesAttached");
    private static readonly MethodSlot RemoveSkeletalMeshAttachedSlot = new("RemoveSkeletalMeshAttached");
    private static readonly MethodSlot RemoveStaticMeshAttachedSlot = new("RemoveStaticMeshAttached");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot RespawnSlot = new("Respawn");
    private static readonly MethodSlot SetAIAvoidanceSettingsSlot = new("SetAIAvoidanceSettings");
    private static readonly MethodSlot SetAirControlSlot = new("SetAirControl");
    private static readonly MethodSlot SetAnimationBlueprintSlot = new("SetAnimationBlueprint");
    private static readonly MethodSlot SetAnimationBlueprintPropertyValueSlot = new("SetAnimationBlueprintPropertyValue");
    private static readonly MethodSlot SetBrakingSettingsSlot = new("SetBrakingSettings");
    private static readonly MethodSlot SetCanCrouchSlot = new("SetCanCrouch");
    private static readonly MethodSlot SetCanJumpSlot = new("SetCanJump");
    private static readonly MethodSlot SetCapsuleSizeSlot = new("SetCapsuleSize");
    private static readonly MethodSlot SetControlRotationSlot = new("SetControlRotation");
    private static readonly MethodSlot SetCrouchingSlot = new("SetCrouching");
    private static readonly MethodSlot SetDamageMultiplierSlot = new("SetDamageMultiplier");
    private static readonly MethodSlot SetFlyingModeSlot = new("SetFlyingMode");
    private static readonly MethodSlot SetGravityScaleSlot = new("SetGravityScale");
    private static readonly MethodSlot SetHealthSlot = new("SetHealth");
    private static readonly MethodSlot SetHitReactionEnabledSlot = new("SetHitReactionEnabled");
    private static readonly MethodSlot SetJumpZVelocitySlot = new("SetJumpZVelocity");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetMaxAccelerationSlot = new("SetMaxAcceleration");
    private static readonly MethodSlot SetMaxHealthSlot = new("SetMaxHealth");
    private static readonly MethodSlot SetMeshSlot = new("SetMesh");
    private static readonly MethodSlot SetMeshSettingsSlot = new("SetMeshSettings");
    private static readonly MethodSlot SetPawnSettingsSlot = new("SetPawnSettings");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPhysicsAssetSlot = new("SetPhysicsAsset");
    private static readonly MethodSlot SetRagdollOnDeathEnabledSlot = new("SetRagdollOnDeathEnabled");
    private static readonly MethodSlot SetRotationSettingsSlot = new("SetRotationSettings");
    private static readonly MethodSlot SetSpeedSettingsSlot = new("SetSpeedSettings");
    private static readonly MethodSlot SetSpringArmSettingsSlot = new("SetSpringArmSettings");
    private static readonly MethodSlot SetStaticMeshAttachedTransformSlot = new("SetStaticMeshAttachedTransform");
    private static readonly MethodSlot StopAnimationSlot = new("StopAnimation");
    private static readonly MethodSlot StopMovementSlot = new("StopMovement");
    private static readonly MethodSlot UnHideBoneSlot = new("UnHideBone");
    private static readonly MethodSlot UnbindAnimationBlueprintEventDispatcherSlot = new("UnbindAnimationBlueprintEventDispatcher");

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

    /// <summary>Gets the possessing Player</summary>
    public Player? Player => GetPlayer();

    /// <summary>Gets if this Character is crouching</summary>
    public bool Crouching
    {
        get => IsCrouching();
        set => SetCrouching(value);
    }

    /// <summary>Returns if the entity is dead</summary>
    public bool Dead => IsDead();

    /// <summary>Spawns a new CharacterSimple.</summary>
    public CharacterSimple(
        Vector location,
        Rotator rotation,
        string mesh,
        string? customAnimationBlueprint = null,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("CharacterSimple", ScriptArguments.Trim([location, rotation, mesh, customAnimationBlueprint, collisionType, gravityEnabled, spawnMode], 3));
    }

    protected CharacterSimple()
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

    /// <summary>Do damage to this entity</summary>
    public uint ApplyDamage(
        uint damage,
        string? boneName = null,
        DamageType? damageType = null,
        Vector? fromDirection = null,
        Player? instigator = null,
        Entity? causer = null) =>
        Invoke<uint>(ApplyDamageSlot, ScriptArguments.Trim([damage, boneName, damageType, fromDirection, instigator, causer], 1))!;

    /// <summary>Assigns and Binds an Animation Blueprint Event Dispatcher</summary>
    public ScriptFunction BindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate callback) =>
        Invoke<ScriptFunction>(BindAnimationBlueprintEventDispatcherSlot, dispatcherName, ScriptCallback.From(callback))!;

    /// <summary>Calls an Animation Blueprint Event or Function</summary>
    public object?[] CallAnimationBlueprintEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic<object?[]>(CallAnimationBlueprintEventSlot, [eventName], arguments)!;

    /// <summary>AI: Makes this Pawn follow another actor</summary>
    public void Follow(
        Entity actor,
        float? acceptanceRadius = null,
        bool? stopOnSucceed = null,
        bool? stopOnFail = null,
        float? updateRate = null) =>
        Invoke(FollowSlot, ScriptArguments.Trim([actor, acceptanceRadius, stopOnSucceed, stopOnFail, updateRate], 1));

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllSkeletalMeshAttachedSlot)!;

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    public IReadOnlyDictionary<string, string> GetAllStaticMeshAttached() =>
        Invoke<IReadOnlyDictionary<string, string>>(GetAllStaticMeshAttachedSlot)!;

    /// <summary>Gets an Animation Blueprint Property/Variable value directly</summary>
    public object? GetAnimationBlueprintPropertyValue(string propertyName) =>
        Invoke<object?>(GetAnimationBlueprintPropertyValueSlot, propertyName);

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    public bool GetCanCrouch() => Invoke<bool>(GetCanCrouchSlot)!;

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    public bool GetCanJump() => Invoke<bool>(GetCanJumpSlot)!;

    /// <summary>Gets the Capsule Size</summary>
    public IReadOnlyDictionary<string, float> GetCapsuleSize() =>
        Invoke<IReadOnlyDictionary<string, float>>(GetCapsuleSizeSlot)!;

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public Rotator GetControlRotation() => Invoke<Rotator>(GetControlRotationSlot)!;

    /// <summary>Gets the Damage Multiplier of a bone</summary>
    public float GetDamageMultiplier(string boneName) => Invoke<float>(GetDamageMultiplierSlot, boneName)!;

    /// <summary>Gets if it's in Flying mode</summary>
    public bool GetFlyingMode() => Invoke<bool>(GetFlyingModeSlot)!;

    /// <summary>Gets the gravity scale</summary>
    public float GetGravityScale() => Invoke<float>(GetGravityScaleSlot)!;

    /// <summary>Gets the current health</summary>
    public uint GetHealth() => Invoke<uint>(GetHealthSlot)!;

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    public ushort GetJumpZVelocity() => Invoke<ushort>(GetJumpZVelocitySlot)!;

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

    /// <summary>Gets the Max Health</summary>
    public uint GetMaxHealth() => Invoke<uint>(GetMaxHealthSlot)!;

    /// <summary>Gets the base Mesh Asset</summary>
    public string GetMesh() => Invoke<string>(GetMeshSlot)!;

    /// <summary>Gets the Moving To location</summary>
    public Vector GetMovingTo() => Invoke<Vector>(GetMovingToSlot)!;

    /// <summary>Gets the possessing Player</summary>
    public Player? GetPlayer() => Invoke<Player?>(GetPlayerSlot);

    /// <summary>Hides a bone of this Pawn</summary>
    public void HideBone(string boneName) => Invoke(HideBoneSlot, boneName);

    /// <summary>Gets if a bone is hidden</summary>
    public bool IsBoneHidden(string boneName) => Invoke<bool>(IsBoneHiddenSlot, boneName)!;

    /// <summary>Gets if this Character is crouching</summary>
    public bool IsCrouching() => Invoke<bool>(IsCrouchingSlot)!;

    /// <summary>Returns if the entity is dead</summary>
    public bool IsDead() => Invoke<bool>(IsDeadSlot)!;

    /// <summary>Triggers this Pawn to jump</summary>
    public void Jump() => Invoke(JumpSlot);

    /// <summary>AI: Makes this Pawn walk to the Location</summary>
    public void MoveTo(Vector location, float? acceptanceRadius = null) =>
        Invoke(MoveToSlot, ScriptArguments.Trim([location, acceptanceRadius], 1));

    /// <summary>Plays an Animation Montage on this character</summary>
    public void PlayAnimation(
        string animationPath,
        string? slotName = null,
        bool? loopIndefinitely = null,
        float? blendInTime = null,
        float? blendOutTime = null,
        float? playRate = null,
        bool? stopAllMontages = null) =>
        Invoke(PlayAnimationSlot, ScriptArguments.Trim([animationPath, slotName, loopIndefinitely, blendInTime, blendOutTime, playRate, stopAllMontages], 1));

    /// <summary>Removes all SkeletalMeshes attached</summary>
    public void RemoveAllSkeletalMeshesAttached() => Invoke(RemoveAllSkeletalMeshesAttachedSlot);

    /// <summary>Removes all StaticMeshes attached</summary>
    public void RemoveAllStaticMeshesAttached() => Invoke(RemoveAllStaticMeshesAttachedSlot);

    /// <summary>Removes, if it exists, a SkeletalMesh from this entity given its custom ID</summary>
    public void RemoveSkeletalMeshAttached(string id) => Invoke(RemoveSkeletalMeshAttachedSlot, id);

    /// <summary>Removes, if it exists, a StaticMesh from this entity given its custom ID</summary>
    public void RemoveStaticMeshAttached(string id) => Invoke(RemoveStaticMeshAttachedSlot, id);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke(ResetMaterialSlot, ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Respawns the Entity, filling its Health and moving it to its Initial Location</summary>
    public void Respawn(Vector? location = null, Rotator? rotation = null) =>
        Invoke(RespawnSlot, ScriptArguments.Trim([location, rotation], 0));

    /// <summary>AI: Configures the RVO (Reciprocal Velocity Obstacles) Avoidance system used by the AIs when moving</summary>
    public void SetAIAvoidanceSettings(bool enabled, ushort? avoidanceConsiderationRadius = null) =>
        Invoke(SetAIAvoidanceSettingsSlot, ScriptArguments.Trim([enabled, avoidanceConsiderationRadius], 1));

    /// <summary>Sets the amount of movement control allowed when it is in air</summary>
    public void SetAirControl(
        float? airControl = null,
        float? boostMultiplier = null,
        float? boostVelocityThreshold = null) =>
        Invoke(SetAirControlSlot, ScriptArguments.Trim([airControl, boostMultiplier, boostVelocityThreshold], 0));

    /// <summary>Sets the Animation Blueprint of this Character</summary>
    public void SetAnimationBlueprint(string customAnimationBlueprint) =>
        Invoke(SetAnimationBlueprintSlot, customAnimationBlueprint);

    /// <summary>Sets an Animation Blueprint Property/Variable value directly</summary>
    public void SetAnimationBlueprintPropertyValue(string propertyName, object? value) =>
        Invoke(SetAnimationBlueprintPropertyValueSlot, propertyName, value);

    /// <summary>Sets the Movement Braking Settings of this Pawn</summary>
    public void SetBrakingSettings(
        float? groundFriction = null,
        float? brakingFrictionFactor = null,
        ushort? brakingWalking = null,
        ushort? brakingFlying = null,
        ushort? brakingSwimming = null,
        ushort? brakingFalling = null) =>
        Invoke(SetBrakingSettingsSlot, ScriptArguments.Trim([groundFriction, brakingFrictionFactor, brakingWalking, brakingFlying, brakingSwimming, brakingFalling], 0));

    /// <summary>Sets if this Pawn is allowed to Crouch</summary>
    public void SetCanCrouch(bool canCrouch) => Invoke(SetCanCrouchSlot, canCrouch);

    /// <summary>Sets if this Pawn is allowed to Jump</summary>
    public void SetCanJump(bool canJump) => Invoke(SetCanJumpSlot, canJump);

    /// <summary>Sets this Pawn's Capsule size</summary>
    public void SetCapsuleSize(ushort? radius = null, ushort? halfHeight = null, ushort? crouchedHalfHeight = null) =>
        Invoke(SetCapsuleSizeSlot, ScriptArguments.Trim([radius, halfHeight, crouchedHalfHeight], 0));

    /// <summary>Sets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    public void SetControlRotation(Rotator controlRotation) => Invoke(SetControlRotationSlot, controlRotation);

    /// <summary>Sets if this Character is crouching</summary>
    public void SetCrouching(bool isCrouching) => Invoke(SetCrouchingSlot, isCrouching);

    /// <summary>Changes how much damage this entity takes on specific bones</summary>
    public void SetDamageMultiplier(string boneName, float multiplier) =>
        Invoke(SetDamageMultiplierSlot, boneName, multiplier);

    /// <summary>Enables or disables the Flying Mode, which removes gravity and allows moving in all directions</summary>
    public void SetFlyingMode(bool flyingMode) => Invoke(SetFlyingModeSlot, flyingMode);

    /// <summary>Changes the Gravity Scale</summary>
    public void SetGravityScale(float gravityScale) => Invoke(SetGravityScaleSlot, gravityScale);

    /// <summary>Sets the Health of this Entity</summary>
    public void SetHealth(uint newHealth) => Invoke(SetHealthSlot, newHealth);

    /// <summary>Sets if the character will react to damage by applying small dynamic ragdoll effect to local bones damaged</summary>
    public void SetHitReactionEnabled(bool isEnabled) => Invoke(SetHitReactionEnabledSlot, isEnabled);

    /// <summary>Sets the velocity of the jump</summary>
    public void SetJumpZVelocity(ushort jumpZVelocity) => Invoke(SetJumpZVelocitySlot, jumpZVelocity);

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

    /// <summary>Sets the max acceleration of the movement</summary>
    public void SetMaxAcceleration(ushort acceleration) => Invoke(SetMaxAccelerationSlot, acceleration);

    /// <summary>Sets the MaxHealth of this Entity</summary>
    public void SetMaxHealth(uint maxHealth) => Invoke(SetMaxHealthSlot, maxHealth);

    /// <summary>Changes the Character Mesh at runtime</summary>
    public void SetMesh(string meshAsset, bool? argument2 = null) =>
        Invoke(SetMeshSlot, ScriptArguments.Trim([meshAsset, argument2], 1));

    /// <summary>Configures the mesh attachment settings and visibility</summary>
    public void SetMeshSettings(
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        bool? isVisible = null) =>
        Invoke(SetMeshSettingsSlot, ScriptArguments.Trim([relativeLocation, relativeRotation, isVisible], 0));

    /// <summary>Sets the Pawn Settings of this Character</summary>
    public void SetPawnSettings(
        bool useControllerRotationPitch,
        bool useControllerRotationYaw,
        bool useControllerRotationRoll) =>
        Invoke(SetPawnSettingsSlot, useControllerRotationPitch, useControllerRotationYaw, useControllerRotationRoll);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Invoke(SetPhysicalMaterialSlot, physicalMaterialPath);

    /// <summary>Sets the Physics Asset for the Character</summary>
    public void SetPhysicsAsset(string physicsAsset) => Invoke(SetPhysicsAssetSlot, physicsAsset);

    /// <summary>Sets if the character will enter ragdoll mode when it dies. You can set it to false to apply custom Death animations</summary>
    public void SetRagdollOnDeathEnabled(bool isEnabled) => Invoke(SetRagdollOnDeathEnabledSlot, isEnabled);

    /// <summary>Sets the Rotation Settings of this Character</summary>
    public void SetRotationSettings(
        Rotator rotationRate,
        bool useControllerDesiredRotation,
        bool orientRotationToMovement) =>
        Invoke(SetRotationSettingsSlot, rotationRate, useControllerDesiredRotation, orientRotationToMovement);

    /// <summary>Sets the Speed Settings of this Character</summary>
    public void SetSpeedSettings(
        ushort? maxWalkSpeed = null,
        ushort? maxWalkSpeedCrouched = null,
        ushort? maxFlySpeed = null) =>
        Invoke(SetSpeedSettingsSlot, ScriptArguments.Trim([maxWalkSpeed, maxWalkSpeedCrouched, maxFlySpeed], 0));

    /// <summary>Sets the Spring Arm Settings of this Character</summary>
    public void SetSpringArmSettings(
        Vector? relativeLocation = null,
        float? targetArmLength = null,
        Vector? socketOffset = null,
        bool? enableCameraLag = null,
        float? cameraLagSpeed = null,
        float? cameraLagMaxDistance = null) =>
        Invoke(SetSpringArmSettingsSlot, ScriptArguments.Trim([relativeLocation, targetArmLength, socketOffset, enableCameraLag, cameraLagSpeed, cameraLagMaxDistance], 0));

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    public void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation) =>
        Invoke(SetStaticMeshAttachedTransformSlot, id, relativeLocation, relativeRotation);

    /// <summary>Stops an Animation Montage on this character</summary>
    public void StopAnimation(string? animationAsset = null) =>
        Invoke(StopAnimationSlot, ScriptArguments.Trim([animationAsset], 0));

    /// <summary>AI: Stops the movement, optionally zeroing the velocity as well</summary>
    public void StopMovement(bool? stopsVelocity = null) =>
        Invoke(StopMovementSlot, ScriptArguments.Trim([stopsVelocity], 0));

    /// <summary>Unhides a bone of this Pawn</summary>
    public void UnHideBone(string boneName) => Invoke(UnHideBoneSlot, boneName);

    /// <summary>Unbinds an Animation Blueprint Event Dispatcher</summary>
    public void UnbindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null) =>
        Invoke(UnbindAnimationBlueprintEventDispatcherSlot, ScriptArguments.Trim([dispatcherName, ScriptCallback.From(callback)], 1));

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Death event of this entity only.</summary>
    public ScriptSubscription SubscribeDeath(DeathHandler handler) => Subscribe("Death", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the HealthChange event of this entity only.</summary>
    public ScriptSubscription SubscribeHealthChange(HealthChangeHandler handler) => Subscribe("HealthChange", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the MoveComplete event of this entity only.</summary>
    public ScriptSubscription SubscribeMoveComplete(MoveCompleteHandler handler) => Subscribe("MoveComplete", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Possess event of this entity only.</summary>
    public ScriptSubscription SubscribePossess(PossessHandler handler) => Subscribe("Possess", handler);

    /// <summary>Subscribes to the Respawn event of this entity only.</summary>
    public ScriptSubscription SubscribeRespawn(RespawnHandler handler) => Subscribe("Respawn", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

    /// <summary>Subscribes to the UnPossess event of this entity only.</summary>
    public ScriptSubscription SubscribeUnPossess(UnPossessHandler handler) => Subscribe("UnPossess", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>When Entity Dies</summary>
    public delegate void DeathHandler(CharacterSimple self, int lastDamageTaken, string? lastBoneDamaged, DamageType damageTypeReason, Vector hitFromDirection, Player? instigator, Actor? causer);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(CharacterSimple self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(CharacterSimple self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(CharacterSimple self);

    /// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
    public delegate void HealthChangeHandler(CharacterSimple self, int oldHealth, int newHealth);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(CharacterSimple self);

    /// <summary>Called when AI reaches its destination, or when it fails</summary>
    public delegate void MoveCompleteHandler(CharacterSimple self, bool succeeded);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(CharacterSimple self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>When this Pawn is possessed by a Player</summary>
    public delegate void PossessHandler(CharacterSimple self, Player? player);

    /// <summary>When Entity Respawns</summary>
    public delegate void RespawnHandler(CharacterSimple self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(CharacterSimple self);

    /// <summary>Triggered when this Entity takes damage</summary>
    public delegate void TakeDamageHandler(CharacterSimple self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, object? causer);

    /// <summary>When this Pawn is unpossessed by a Player</summary>
    public delegate void UnPossessHandler(CharacterSimple self, Player? oldPlayer);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(CharacterSimple self, string? key, object? value);
}
