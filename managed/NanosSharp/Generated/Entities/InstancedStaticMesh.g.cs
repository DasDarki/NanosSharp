#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>An Instanced Static Mesh entity allows spawning a mesh that can have multiple instances efficiently rendered.</summary>
[ScriptClass("InstancedStaticMesh")]
public partial class InstancedStaticMesh : Actor, IPaintable
{
    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
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

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("InstancedStaticMesh", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("InstancedStaticMesh", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("InstancedStaticMesh", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("InstancedStaticMesh", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("InstancedStaticMesh", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("InstancedStaticMesh", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("InstancedStaticMesh", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("InstancedStaticMesh", "ValueChange");

    /// <summary>Returns the number of instances currently stored</summary>
    public ulong InstanceCount => GetInstanceCount();

    /// <summary>Gets the Asset path mesh used</summary>
    public string Mesh => GetMesh();

    /// <summary>Spawns a new InstancedStaticMesh.</summary>
    public InstancedStaticMesh(
        Vector location,
        Rotator rotation,
        string staticMeshAsset,
        CollisionType? collisionType = null,
        object? instances = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("InstancedStaticMesh", ScriptArguments.Trim([location, rotation, staticMeshAsset, collisionType, instances, spawnMode], 3));
    }

    protected InstancedStaticMesh()
    {
    }

    /// <summary>Add an instance to this component</summary>
    public uint AddInstance(Vector relativeLocation, Rotator? relativeRotation = null, Vector? scale = null) =>
        Invoke<uint>("AddInstance", ScriptArguments.Trim([relativeLocation, relativeRotation, scale], 1))!;

    /// <summary>Add multiple instances to this component. It is way efficient calling this once than AddInstance multiple times</summary>
    public void AddInstances(IEnumerable<IDictionary<string, object?>> instances) => Invoke("AddInstances", instances);

    /// <summary>Removes all instances.</summary>
    public void ClearInstances() => Invoke("ClearInstances");

    /// <summary>Returns the number of instances currently stored</summary>
    public ulong GetInstanceCount() => Invoke<ulong>("GetInstanceCount")!;

    /// <summary>Returns the current transform of the instance at the given index</summary>
    public object?[] GetInstanceTransform(uint index) => Invoke<object?[]>("GetInstanceTransform", index)!;

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

    /// <summary>Gets the Asset path mesh used</summary>
    public string GetMesh() => Invoke<string>("GetMesh")!;

    /// <summary>Removes the instance at the given index. This will move the last element to the provided index, reducing the count by 1</summary>
    public void RemoveInstance(uint index) => Invoke("RemoveInstance", index);

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Invoke("ResetMaterial", ScriptArguments.Trim([index, attachableId], 0));

    /// <summary>Changes the transform of an existing instance. You can pass all other parameters as nil to modify only a single property</summary>
    public void SetInstanceTransform(
        uint index,
        Vector? relativeLocation = null,
        Rotator? relativeRotation = null,
        Vector? scale = null) =>
        Invoke("SetInstanceTransform", ScriptArguments.Trim([index, relativeLocation, relativeRotation, scale], 1));

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

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) => Invoke("SetPhysicalMaterial", physicalMaterialPath);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(InstancedStaticMesh self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(InstancedStaticMesh self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(InstancedStaticMesh self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(InstancedStaticMesh self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(InstancedStaticMesh self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(InstancedStaticMesh self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(InstancedStaticMesh self, string? key, object? value);
}
