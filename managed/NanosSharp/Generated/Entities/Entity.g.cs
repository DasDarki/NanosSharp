#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Entity is the base for all Classes, and all those entities share the same Methods and Events described in this page.</summary>
public abstract partial class Entity
{
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
        Invoke("BroadcastRemoteEvent", [eventName, reliability, .. args]);

    /// <summary>Calls a custom remote event directly on this entity to all Players in a radius</summary>
    public void BroadcastRemoteInRadiusEvent(
        string eventName,
        double radius,
        Reliability? reliability = null,
        params object?[] args) =>
        Invoke("BroadcastRemoteInRadiusEvent", [eventName, radius, reliability, .. args]);

    /// <summary>Calls a custom remote event directly on this entity to a specific Player</summary>
    public void CallRemoteEvent(
        string eventName,
        Player player,
        Reliability? reliability = null,
        params object?[] args) =>
        Invoke("CallRemoteEvent", [eventName, player, reliability, .. args]);

    /// <summary>Calls a custom remote event directly on this entity to a list of Players</summary>
    public void CallRemotePlayersEvent(
        string eventName,
        IEnumerable<Player> players,
        Reliability? reliability = null,
        params object?[] args) =>
        Invoke("CallRemotePlayersEvent", [eventName, players, reliability, .. args]);

    /// <summary>Destroys this Entity</summary>
    public void Destroy() => Invoke("Destroy");

    /// <summary>Finish the spawning process and send the entity to clients if it was spawned with a deferred <c>spawn_mode</c></summary>
    public void FinishSpawn() => Invoke("FinishSpawn");

    /// <summary>Gets a list of all values keys</summary>
    public IReadOnlyList<string> GetAllValuesKeys() => Invoke<IReadOnlyList<string>>("GetAllValuesKeys")!;

    /// <summary>Gets the universal network ID of this Entity (same on both client and server)</summary>
    public uint GetID() => Invoke<uint>("GetID")!;

    /// <summary>Gets a Value stored on this Entity at the given key</summary>
    public object? GetValue(string key, object? fallback = null) =>
        Invoke<object?>("GetValue", ScriptArguments.Trim([key, fallback], 1));

    /// <summary>Returns true if this Entity is being destroyed</summary>
    public bool IsBeingDestroyed() => Invoke<bool>("IsBeingDestroyed")!;

    /// <summary>Gets if this Entity finished spawning</summary>
    public bool IsSpawned() => Invoke<bool>("IsSpawned")!;

    /// <summary>Overrides when this Entity finishes spawning, from the Constructor of an Inherited Class</summary>
    public void SetSpawnMode(SpawnMode spawnMode) => Invoke("SetSpawnMode", spawnMode);

    /// <summary>Sets a Value in this Entity</summary>
    public void SetValue(string key, object? value, bool? syncOnClients = null) =>
        Invoke("SetValue", ScriptArguments.Trim([key, value, syncOnClients], 2));
}
