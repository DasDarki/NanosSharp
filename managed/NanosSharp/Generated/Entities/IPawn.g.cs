#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class for all Pawn entities (Character and CharacterSimple).</summary>
public partial interface IPawn
{

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllSkeletalMeshAttached { get; }

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllStaticMeshAttached { get; }

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    bool CanCrouch { get; set; }

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    bool CanJump { get; set; }

    /// <summary>Gets the Capsule Size</summary>
    IReadOnlyDictionary<string, float> CapsuleSize { get; }

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    Rotator ControlRotation { get; set; }

    /// <summary>Gets if it's in Flying mode</summary>
    bool FlyingMode { get; set; }

    /// <summary>Gets the gravity scale</summary>
    float GravityScale { get; set; }

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    ushort JumpZVelocity { get; set; }

    /// <summary>Gets the base Mesh Asset</summary>
    string Mesh { get; }

    /// <summary>Gets the Moving To location</summary>
    Vector MovingTo { get; }

    /// <summary>Gets the possessing Player</summary>
    Player? Player { get; }

    /// <summary>Attaches a Skeletal Mesh as master pose to this entity</summary>
    void AddSkeletalMeshAttached(string id, string skeletalMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, bool? useBaseLeaderPoseComponent = null, string? animationPath = null, string? attachableId = null);

    /// <summary>Attaches a Static Mesh to this entity</summary>
    void AddStaticMeshAttached(string id, string staticMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, string? attachableId = null);

    /// <summary>AI: Makes this Pawn follow another actor</summary>
    void Follow(Entity actor, float? acceptanceRadius = null, bool? stopOnSucceed = null, bool? stopOnFail = null, float? updateRate = null);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllStaticMeshAttached();

    /// <summary>Gets if this Pawn is allowed to Crouch</summary>
    bool GetCanCrouch();

    /// <summary>Gets if this Pawn is allowed to Jump</summary>
    bool GetCanJump();

    /// <summary>Gets the Capsule Size</summary>
    IReadOnlyDictionary<string, float> GetCapsuleSize();

    /// <summary>Gets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    Rotator GetControlRotation();

    /// <summary>Gets if it's in Flying mode</summary>
    bool GetFlyingMode();

    /// <summary>Gets the gravity scale</summary>
    float GetGravityScale();

    /// <summary>Gets the initial vertical velocity when jumping, which defines the jump height</summary>
    ushort GetJumpZVelocity();

    /// <summary>Gets the base Mesh Asset</summary>
    string GetMesh();

    /// <summary>Gets the Moving To location</summary>
    Vector GetMovingTo();

    /// <summary>Gets the possessing Player</summary>
    Player? GetPlayer();

    /// <summary>Hides a bone of this Pawn</summary>
    void HideBone(string boneName);

    /// <summary>Gets if a bone is hidden</summary>
    bool IsBoneHidden(string boneName);

    /// <summary>Triggers this Pawn to jump</summary>
    void Jump();

    /// <summary>AI: Makes this Pawn walk to the Location</summary>
    void MoveTo(Vector location, float? acceptanceRadius = null);

    /// <summary>Removes all SkeletalMeshes attached</summary>
    void RemoveAllSkeletalMeshesAttached();

    /// <summary>Removes all StaticMeshes attached</summary>
    void RemoveAllStaticMeshesAttached();

    /// <summary>Removes, if it exists, a SkeletalMesh from this entity given its custom ID</summary>
    void RemoveSkeletalMeshAttached(string id);

    /// <summary>Removes, if it exists, a StaticMesh from this entity given its custom ID</summary>
    void RemoveStaticMeshAttached(string id);

    /// <summary>AI: Configures the RVO (Reciprocal Velocity Obstacles) Avoidance system used by the AIs when moving</summary>
    void SetAIAvoidanceSettings(bool enabled, ushort? avoidanceConsiderationRadius = null);

    /// <summary>Sets the Movement Braking Settings of this Pawn</summary>
    void SetBrakingSettings(float? groundFriction = null, float? brakingFrictionFactor = null, ushort? brakingWalking = null, ushort? brakingFlying = null, ushort? brakingSwimming = null, ushort? brakingFalling = null);

    /// <summary>Sets if this Pawn is allowed to Crouch</summary>
    void SetCanCrouch(bool canCrouch);

    /// <summary>Sets if this Pawn is allowed to Jump</summary>
    void SetCanJump(bool canJump);

    /// <summary>Sets this Pawn's Capsule size</summary>
    void SetCapsuleSize(ushort? radius = null, ushort? halfHeight = null, ushort? crouchedHalfHeight = null);

    /// <summary>Sets the Control Rotation, which is where this Pawn is looking or aiming</summary>
    void SetControlRotation(Rotator controlRotation);

    /// <summary>Enables or disables the Flying Mode, which removes gravity and allows moving in all directions</summary>
    void SetFlyingMode(bool flyingMode);

    /// <summary>Changes the Gravity Scale</summary>
    void SetGravityScale(float gravityScale);

    /// <summary>Sets if the character will react to damage by applying small dynamic ragdoll effect to local bones damaged</summary>
    void SetHitReactionEnabled(bool isEnabled);

    /// <summary>Sets the velocity of the jump</summary>
    void SetJumpZVelocity(ushort jumpZVelocity);

    /// <summary>Configures the mesh attachment settings and visibility</summary>
    void SetMeshSettings(Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? isVisible = null);

    /// <summary>Sets if the character will enter ragdoll mode when it dies. You can set it to false to apply custom Death animations</summary>
    void SetRagdollOnDeathEnabled(bool isEnabled);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation);

    /// <summary>Stops an Animation Montage on this character</summary>
    void StopAnimation(string? animationAsset = null);

    /// <summary>AI: Stops the movement, optionally zeroing the velocity as well</summary>
    void StopMovement(bool? stopsVelocity = null);

    /// <summary>Unhides a bone of this Pawn</summary>
    void UnHideBone(string boneName);
}
