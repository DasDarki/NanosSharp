#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When this Pawn is possessed by a Player</summary>
public sealed class CharacterPossessEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The Player</summary>
    public Player? Player { get; init; }
}
