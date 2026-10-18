#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Entity
{
    private static readonly MethodSlot BroadcastRemoteEventSlot = new("BroadcastRemoteEvent");
    private static readonly MethodSlot BroadcastRemoteInRadiusEventSlot = new("BroadcastRemoteInRadiusEvent");
    private static readonly MethodSlot CallRemoteEventSlot = new("CallRemoteEvent");
    private static readonly MethodSlot CallRemotePlayersEventSlot = new("CallRemotePlayersEvent");

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
}
