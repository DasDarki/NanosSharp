#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Base class for all Vehicle entities.</summary>
public partial interface IVehicle
{

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllSkeletalMeshAttached { get; }

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllStaticMeshAttached { get; }

    /// <summary>Gets all configured Doors</summary>
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Doors { get; }

    /// <summary>Gets the Asset name</summary>
    string Mesh { get; }

    /// <summary>Gets all passengers</summary>
    IReadOnlyList<object?> Passengers { get; }

    /// <summary>Checks if the engine is started</summary>
    bool EngineStarted { get; }

    /// <summary>Attaches a Skeletal Mesh as master pose to this entity</summary>
    void AddSkeletalMeshAttached(string id, string skeletalMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, bool? useBaseLeaderPoseComponent = null, string? animationPath = null, string? attachableId = null);

    /// <summary>Attaches a Static Mesh to this entity</summary>
    void AddStaticMeshAttached(string id, string staticMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, string? attachableId = null);

    /// <summary>Assigns and Binds an Animation Blueprint Event Dispatcher</summary>
    ScriptFunction BindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate callback);

    /// <summary>Calls an Animation Blueprint Event or Function</summary>
    object?[] CallAnimationBlueprintEvent(string eventName, params object?[] arguments);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllStaticMeshAttached();

    /// <summary>Gets an Animation Blueprint Property/Variable value directly</summary>
    object? GetAnimationBlueprintPropertyValue(string propertyName);

    /// <summary>Gets all configured Doors</summary>
    IReadOnlyList<IReadOnlyDictionary<string, object?>> GetDoors();

    /// <summary>Gets the Asset name</summary>
    string GetMesh();

    /// <summary>Gets a passenger from a seat</summary>
    Character? GetPassenger(int seat);

    /// <summary>Gets all passengers</summary>
    IReadOnlyList<object?> GetPassengers();

    /// <summary>Hides a bone of this Vehicle</summary>
    void HideBone(string boneName);

    /// <summary>Gets if a bone is hidden</summary>
    bool IsBoneHidden(string boneName);

    /// <summary>Checks if the engine is started</summary>
    bool IsEngineStarted();

    /// <summary>Plays an Animation on this Vehicle</summary>
    void PlayAnimation(string animationPath, string? slotName = null, bool? loopIndefinitely = null, float? blendInTime = null, float? blendOutTime = null, float? playRate = null, bool? stopAllMontages = null);

    /// <summary>Removes all SkeletalMeshes attached</summary>
    void RemoveAllSkeletalMeshesAttached();

    /// <summary>Removes all StaticMeshes attached</summary>
    void RemoveAllStaticMeshesAttached();

    /// <summary>Removes, if it exists, a SkeletalMesh from this Vehicle given its custom ID</summary>
    void RemoveSkeletalMeshAttached(string id);

    /// <summary>Removes, if it exists, a StaticMesh from this Vehicle given its custom ID</summary>
    void RemoveStaticMeshAttached(string id);

    /// <summary>Sets an Animation Blueprint Property/Variable value directly</summary>
    void SetAnimationBlueprintPropertyValue(string propertyName, object? value);

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null);

    /// <summary>Sets how this entity behaves on Water</summary>
    void SetBuoyancySettings(bool? enabled = null, float? buoyancyCoefficient = null, float? buoyancyDamp = null, float? maxBuoyantForce = null);

    /// <summary>Sets how far the camera stays from this Vehicle while driving it</summary>
    void SetCameraArmLength(ushort armLength);

    /// <summary>Sets the Vehicle Camera Offset</summary>
    void SetCameraOffset(Vector offset);

    /// <summary>Adds a Door to the Vehicle</summary>
    void SetDoor(byte seatIndex, Vector offsetLocation, Vector seatLocation, Rotator seatRotation, ushort triggerRadius, ushort leaveLateralOffset);

    /// <summary>Configures the Explosion Settings when health reaches zero</summary>
    void SetExplosionSettings(Vector engineRelativeLocation, IEnumerable<byte> materialsIndexBurntOverride, IEnumerable<byte> materialsIndexBurntInvisible);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation);

    /// <summary>Stops an Animation Montage on this Vehicle</summary>
    void StopAnimation(string? animationAsset = null);

    /// <summary>UnHide a bone of this Vehicle</summary>
    void UnHideBone(string boneName);

    /// <summary>Unbinds an Animation Blueprint Event Dispatcher</summary>
    void UnbindAnimationBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null);
}
