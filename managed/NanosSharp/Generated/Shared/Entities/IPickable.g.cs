#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Pickables are special Actors which can be grabbed, held and used by Characters.</summary>
public partial interface IPickable
{

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllSkeletalMeshAttached { get; }

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> AllStaticMeshAttached { get; }

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    Character? Handler { get; }

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    string Mesh { get; }

    /// <summary>Attaches a Skeletal Mesh as master pose to this entity</summary>
    void AddSkeletalMeshAttached(string id, string skeletalMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, bool? useBaseLeaderPoseComponent = null, string? animationPath = null, string? attachableId = null);

    /// <summary>Attaches a Static Mesh to this entity</summary>
    void AddStaticMeshAttached(string id, string staticMeshPath, string? socket = null, Vector? relativeLocation = null, Rotator? relativeRotation = null, bool? useParentBounds = null, string? attachableId = null);

    /// <summary>Gets all Skeletal Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllSkeletalMeshAttached();

    /// <summary>Gets all Static Meshes attached to this entity</summary>
    IReadOnlyDictionary<string, string> GetAllStaticMeshAttached();

    /// <summary>Gets the Character, if it exists, that's holding this Pickable</summary>
    Character? GetHandler();

    /// <summary>Gets the Handling Mode of this Pickable</summary>
    HandlingMode GetHandlingMode();

    /// <summary>Gets the name of the asset this Pickable uses</summary>
    string GetMesh();

    /// <summary>Removes all SkeletalMeshes attached</summary>
    void RemoveAllSkeletalMeshesAttached();

    /// <summary>Removes all StaticMeshes attached</summary>
    void RemoveAllStaticMeshesAttached();

    /// <summary>Removes, if it exists, a SkeletalMesh from this Pickable given its custom ID</summary>
    void RemoveSkeletalMeshAttached(string id);

    /// <summary>Removes, if it exists, a StaticMesh from this Pickable given its custom ID</summary>
    void RemoveStaticMeshAttached(string id);

    /// <summary>Sets the Attachment Settings for this Pickable (how it attaches to the Character when Picking up)</summary>
    void SetAttachmentSettings(Vector relativeLocation, Rotator? relativeRotation = null, string? socket = null);

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null);

    /// <summary>Sets how this entity behaves on Water</summary>
    void SetBuoyancySettings(bool? enabled = null, float? buoyancyCoefficient = null, float? buoyancyDamp = null, float? maxBuoyantForce = null);

    /// <summary>Sets if this Pickable can be used by the player when held</summary>
    void SetCanUse(bool canUse);

    /// <summary>Sets the crosshair material for this Pickable</summary>
    void SetCrosshairMaterial(string materialAsset);

    /// <summary>Sets a Static Mesh Attached location and rotation</summary>
    void SetStaticMeshAttachedTransform(string id, Vector relativeLocation, Rotator relativeRotation);
}
