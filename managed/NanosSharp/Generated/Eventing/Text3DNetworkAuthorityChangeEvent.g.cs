#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Actor changes its network authority Player</summary>
public sealed class Text3DNetworkAuthorityChangeEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public Text3D Text3D { get; init; } = null!;

    /// <summary>The old Network Authority Player, if any</summary>
    public Player? OldNetworkAuthority { get; init; }

    /// <summary>The new Network Authority Player, if any</summary>
    public Player? NewNetworkAuthority { get; init; }
}
