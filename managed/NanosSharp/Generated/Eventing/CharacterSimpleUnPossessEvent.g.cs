#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When this Pawn is unpossessed by a Player</summary>
public sealed class CharacterSimpleUnPossessEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;

    /// <summary>The Player that was possessing this Pawn</summary>
    public Player? OldPlayer { get; init; }
}
