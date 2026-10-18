#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When this Pawn is unpossessed by a Player</summary>
public sealed class CharacterGASPUnPossessEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Player that was possessing this Pawn</summary>
    public Player? OldPlayer { get; init; }
}
