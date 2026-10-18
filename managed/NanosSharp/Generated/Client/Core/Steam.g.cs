#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Interact with Steam APIs.</summary>
public static partial class Steam
{
    private static readonly StaticMethodSlot SetRichPresenceSlot = new("Steam", "SetRichPresence");
    private static readonly StaticMethodSlot TriggerScreenshotSlot = new("Steam", "TriggerScreenshot");

    /// <summary>Sets Steam Rich Presence text</summary>
    public static void SetRichPresence(string text) => ScriptApi.Call(SetRichPresenceSlot, 1, text);

    /// <summary>Triggers a Steam screenshot</summary>
    public static void TriggerScreenshot() => ScriptApi.Call(TriggerScreenshotSlot, 0);
}
