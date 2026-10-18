#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called upon landing when falling</summary>
public sealed class CharacterSimpleLandEvent : ScriptEvent
{
    /// <summary>The CharacterSimple that landed</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;
}
