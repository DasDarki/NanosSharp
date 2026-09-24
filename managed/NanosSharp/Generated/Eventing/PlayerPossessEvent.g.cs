#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when Player starts controlling a Pawn (Character)</summary>
public sealed class PlayerPossessEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;

    /// <summary>The Pawn being possessed</summary>
    public Entity? Pawn { get; init; }
}
