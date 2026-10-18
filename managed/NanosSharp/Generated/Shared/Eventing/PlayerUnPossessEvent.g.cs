#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>A Pawn (Character) was unpossessed from the Player</summary>
public sealed class PlayerUnPossessEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;

    /// <summary>The Pawn being unpossessed</summary>
    public Entity? Pawn { get; init; }
}
