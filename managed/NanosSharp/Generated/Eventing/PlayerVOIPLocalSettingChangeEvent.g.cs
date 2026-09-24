#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the local VOIP setting changes for this player</summary>
public sealed class PlayerVOIPLocalSettingChangeEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;

    /// <summary>The previous setting</summary>
    public VOIPSetting OldSetting { get; init; }

    /// <summary>The new setting</summary>
    public VOIPSetting NewSetting { get; init; }
}
