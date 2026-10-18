#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Entity is the base for all Classes, and all those entities share the same Methods and Events described in this page.</summary>
public abstract partial class Entity
{
    private static readonly MethodSlot DestroySlot = new("Destroy");
    private static readonly MethodSlot FinishSpawnSlot = new("FinishSpawn");
    private static readonly MethodSlot GetAllValuesKeysSlot = new("GetAllValuesKeys");
    private static readonly MethodSlot GetIDSlot = new("GetID");
    private static readonly MethodSlot GetValueSlot = new("GetValue");
    private static readonly MethodSlot IsBeingDestroyedSlot = new("IsBeingDestroyed");
    private static readonly MethodSlot IsSpawnedSlot = new("IsSpawned");
    private static readonly MethodSlot SetSpawnModeSlot = new("SetSpawnMode");
    private static readonly MethodSlot SetValueSlot = new("SetValue");

    /// <summary>Gets a list of all values keys</summary>
    public IReadOnlyList<string> AllValuesKeys => GetAllValuesKeys();

    /// <summary>Gets the universal network ID of this Entity (same on both client and server)</summary>
    public uint ID => GetID();

    /// <summary>Returns true if this Entity is being destroyed</summary>
    public bool BeingDestroyed => IsBeingDestroyed();

    /// <summary>Gets if this Entity finished spawning</summary>
    public bool Spawned => IsSpawned();

    /// <summary>Destroys this Entity</summary>
    public void Destroy() => Call(DestroySlot, 0);

    /// <summary>Finish the spawning process and send the entity to clients if it was spawned with a deferred <c>spawn_mode</c></summary>
    public void FinishSpawn() => Call(FinishSpawnSlot, 0);

    /// <summary>Gets a list of all values keys</summary>
    public IReadOnlyList<string> GetAllValuesKeys() => CallResult<IReadOnlyList<string>>(GetAllValuesKeysSlot, 0)!;

    /// <summary>Gets the universal network ID of this Entity (same on both client and server)</summary>
    public uint GetID() => CallResult<uint>(GetIDSlot, 0)!;

    /// <summary>Gets a Value stored on this Entity at the given key</summary>
    public object? GetValue(string key, object? fallback = null) =>
        CallResult<object?, string, object?>(GetValueSlot, 1, key, fallback);

    /// <summary>Returns true if this Entity is being destroyed</summary>
    public bool IsBeingDestroyed() => CallResult<bool>(IsBeingDestroyedSlot, 0)!;

    /// <summary>Gets if this Entity finished spawning</summary>
    public bool IsSpawned() => CallResult<bool>(IsSpawnedSlot, 0)!;

    /// <summary>Overrides when this Entity finishes spawning, from the Constructor of an Inherited Class</summary>
    public void SetSpawnMode(SpawnMode spawnMode) => Call(SetSpawnModeSlot, 1, spawnMode);

    /// <summary>Sets a Value in this Entity</summary>
    public void SetValue(string key, object? value, bool? syncOnClients = null) =>
        Call(SetValueSlot, 2, key, value, syncOnClients);
}
