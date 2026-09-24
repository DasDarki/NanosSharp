#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When this Pawn is possessed by a Player</summary>
public sealed class CharacterSimplePossessEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;

    /// <summary>The Player</summary>
    public Player? Player { get; init; }
}
