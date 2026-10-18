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
    private static readonly StaticMethodSlot OpenCheckoutOverlaySlot = new("Tebex", "OpenCheckoutOverlay");

    /// <summary>Opens the Tebex checkout for a package on the client of a player</summary>
    public static void OpenCheckoutOverlay(Player player, ulong packageId) =>
        ScriptApi.Call(OpenCheckoutOverlaySlot, 2, player, packageId);
}
