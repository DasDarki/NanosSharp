#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Tebex store integration, configured in the server Config.toml.</summary>
public static partial class Tebex
{
    /// <summary>Triggered for a queued Tebex command that does not require the player to be online</summary>
    public static event OfflineCommandHandler? OnOfflineCommand
    {
        add => OfflineCommandEvent.Add(value);
        remove => OfflineCommandEvent.Remove(value);
    }

    /// <summary>Triggered for a queued Tebex command of a connected player</summary>
    public static event OnlineCommandHandler? OnOnlineCommand
    {
        add => OnlineCommandEvent.Add(value);
        remove => OnlineCommandEvent.Remove(value);
    }

    private static readonly ClassEvent<OfflineCommandHandler> OfflineCommandEvent = new("Tebex", "OfflineCommand");
    private static readonly ClassEvent<OnlineCommandHandler> OnlineCommandEvent = new("Tebex", "OnlineCommand");
    private static readonly StaticMethodSlot OpenCheckoutOverlaySlot = new("Tebex", "OpenCheckoutOverlay");

    /// <summary>Opens the Tebex checkout for a package on the client of a player</summary>
    public static void OpenCheckoutOverlay(Player player, ulong packageId) =>
        ScriptApi.Call(OpenCheckoutOverlaySlot, 2, player, packageId);

    /// <summary>Triggered for a queued Tebex command that does not require the player to be online</summary>
    public delegate void OfflineCommandHandler(long commandId, long paymentId, long packageId, string? command, string? playerId, string? playerName, string? playerSteamId);

    /// <summary>Triggered for a queued Tebex command of a connected player</summary>
    public delegate void OnlineCommandHandler(long commandId, long paymentId, long packageId, string? command, string? playerId, string? playerName, string? playerSteamId);
}
