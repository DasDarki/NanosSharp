#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when AI reaches its destination, or when it fails</summary>
public sealed class CharacterSimpleMoveCompleteEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;

    /// <summary>Whether it reached the destination</summary>
    public bool Succeeded { get; init; }
}
