#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Event fired when the character has just started jumping</summary>
public sealed class CharacterSimpleJumpEvent : ScriptEvent
{
    /// <summary>The CharacterSimple that jumped</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;
}
