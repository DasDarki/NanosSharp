#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a global VOIP channel setting changes for this player</summary>
public sealed class PlayerVOIPGlobalChannelSettingChangeEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;

    /// <summary>The channel that changed (1-63)</summary>
    public int Channel { get; init; }

    /// <summary>The previous setting</summary>
    public VOIPSetting OldSetting { get; init; }

    /// <summary>The new setting</summary>
    public VOIPSetting NewSetting { get; init; }
}
