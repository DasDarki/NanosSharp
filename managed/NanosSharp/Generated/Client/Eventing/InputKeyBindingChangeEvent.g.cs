#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>A key binding has been changed through the settings</summary>
public sealed class InputKeyBindingChangeEvent : ScriptEvent
{
    /// <summary>The keybinding id</summary>
    public string? BindingName { get; init; }

    /// <summary>The new key associated</summary>
    public string? Key { get; init; }

    /// <summary>The scale input (usually 1 or -1)</summary>
    public float Scale { get; init; }
}
