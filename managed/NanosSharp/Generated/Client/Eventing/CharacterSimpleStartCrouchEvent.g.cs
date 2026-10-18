#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when Character crouches</summary>
public sealed class CharacterSimpleStartCrouchEvent : ScriptEvent
{
    /// <summary>The CharacterSimple that started crouching</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;
}
