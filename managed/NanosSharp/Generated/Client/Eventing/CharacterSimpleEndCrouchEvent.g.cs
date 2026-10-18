#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when Character stops crouching</summary>
public sealed class CharacterSimpleEndCrouchEvent : ScriptEvent
{
    /// <summary>The CharacterSimple that stopped crouching</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;
}
