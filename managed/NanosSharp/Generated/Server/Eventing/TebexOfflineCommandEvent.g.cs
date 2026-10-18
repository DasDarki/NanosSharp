#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered for a queued Tebex command that does not require the player to be online</summary>
public sealed class TebexOfflineCommandEvent : ScriptEvent
{
    /// <summary>The Tebex command identifier</summary>
    public long CommandId { get; init; }

    /// <summary>The Tebex payment identifier</summary>
    public long PaymentId { get; init; }

    /// <summary>The Tebex package identifier</summary>
    public long PackageId { get; init; }

    /// <summary>The command configured in Tebex</summary>
    public string? Command { get; init; }

    /// <summary>The Tebex player identifier</summary>
    public string? PlayerId { get; init; }

    /// <summary>The player name</summary>
    public string? PlayerName { get; init; }

    /// <summary>The Steam ID of the player</summary>
    public string? PlayerSteamId { get; init; }
}
