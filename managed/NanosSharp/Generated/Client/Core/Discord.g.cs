#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Initialize and set Discord activity.</summary>
public static partial class Discord
{
    private static readonly StaticMethodSlot InitializeSlot = new("Discord", "Initialize");
    private static readonly StaticMethodSlot SetActivitySlot = new("Discord", "SetActivity");

    /// <summary>Initializes the Discord Integration with your custom client_id</summary>
    public static void Initialize(ulong clientId) => ScriptApi.Call(InitializeSlot, 1, clientId);

    /// <summary>Communicates with Discord and sets a custom user status</summary>
    public static void SetActivity(
        string state,
        string details,
        string largeImage,
        string largeText,
        bool? resetTime = null) =>
        ScriptApi.Call(SetActivitySlot, 4, state, details, largeImage, largeText, resetTime);
}
