#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when AI reaches its destination, or when it fails</summary>
public sealed class CharacterGASPMoveCompleteEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>Whether it reached the destination</summary>
    public bool Succeeded { get; init; }
}
