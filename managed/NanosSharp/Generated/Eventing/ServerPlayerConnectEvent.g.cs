#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a player is attempting to connect to the server</summary>
public sealed class ServerPlayerConnectEvent : ScriptEvent
{
    /// <summary>IP of the player</summary>
    public string? Ip { get; init; }

    /// <summary>Account ID of the player</summary>
    public string? PlayerAccountId { get; init; }

    /// <summary>Player name</summary>
    public string? PlayerName { get; init; }

    /// <summary>SteamID of the player</summary>
    public string? PlayerSteamId { get; init; }
}
