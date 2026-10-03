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
    private static readonly MethodSlot BroadcastRemoteEventSlot = new("BroadcastRemoteEvent");
    private static readonly MethodSlot BroadcastRemoteInRadiusEventSlot = new("BroadcastRemoteInRadiusEvent");
    private static readonly MethodSlot CallRemoteEventSlot = new("CallRemoteEvent");
    private static readonly MethodSlot CallRemotePlayersEventSlot = new("CallRemotePlayersEvent");
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

    /// <summary>Calls a custom remote event directly on this entity to all Players</summary>
    public void BroadcastRemoteEvent(string eventName, Reliability? reliability = null, params object?[] args) =>
        InvokeVariadic(BroadcastRemoteEventSlot, [eventName, reliability], args);

    /// <summary>Calls a custom remote event directly on this entity to all Players in a radius</summary>
    public void BroadcastRemoteInRadiusEvent(
        string eventName,
        double radius,
        Reliability? reliability = null,
        params object?[] args) =>
        InvokeVariadic(BroadcastRemoteInRadiusEventSlot, [eventName, radius, reliability], args);

    /// <summary>Calls a custom remote event directly on this entity to a specific Player</summary>
    public void CallRemoteEvent(
        string eventName,
        Player player,
        Reliability? reliability = null,
        params object?[] args) =>
        InvokeVariadic(CallRemoteEventSlot, [eventName, player, reliability], args);

    /// <summary>Calls a custom remote event directly on this entity to a list of Players</summary>
    public void CallRemotePlayersEvent(
        string eventName,
        IEnumerable<Player> players,
        Reliability? reliability = null,
        params object?[] args) =>
        InvokeVariadic(CallRemotePlayersEventSlot, [eventName, players, reliability], args);

    /// <summary>Destroys this Entity</summary>
    public void Destroy() => Invoke(DestroySlot);

    /// <summary>Finish the spawning process and send the entity to clients if it was spawned with a deferred <c>spawn_mode</c></summary>
    public void FinishSpawn() => Invoke(FinishSpawnSlot);

    /// <summary>Gets a list of all values keys</summary>
    public IReadOnlyList<string> GetAllValuesKeys() => Invoke<IReadOnlyList<string>>(GetAllValuesKeysSlot)!;

    /// <summary>Gets the universal network ID of this Entity (same on both client and server)</summary>
    public uint GetID() => Invoke<uint>(GetIDSlot)!;

    /// <summary>Gets a Value stored on this Entity at the given key</summary>
    public object? GetValue(string key, object? fallback = null) =>
        Invoke<object?>(GetValueSlot, ScriptArguments.Trim([key, fallback], 1));

    /// <summary>Returns true if this Entity is being destroyed</summary>
    public bool IsBeingDestroyed() => Invoke<bool>(IsBeingDestroyedSlot)!;

    /// <summary>Gets if this Entity finished spawning</summary>
    public bool IsSpawned() => Invoke<bool>(IsSpawnedSlot)!;

    /// <summary>Overrides when this Entity finishes spawning, from the Constructor of an Inherited Class</summary>
    public void SetSpawnMode(SpawnMode spawnMode) => Invoke(SetSpawnModeSlot, spawnMode);

    /// <summary>Sets a Value in this Entity</summary>
    public void SetValue(string key, object? value, bool? syncOnClients = null) =>
        Invoke(SetValueSlot, ScriptArguments.Trim([key, value, syncOnClients], 2));
}
