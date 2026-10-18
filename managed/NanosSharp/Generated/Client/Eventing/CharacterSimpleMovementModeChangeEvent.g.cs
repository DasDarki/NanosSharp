#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the Character movement mode changes</summary>
public sealed class CharacterSimpleMovementModeChangeEvent : ScriptEvent
{
    /// <summary>The CharacterSimple that changed movement mode</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;

    /// <summary>Previous movement mode</summary>
    public int OldMode { get; init; }

    /// <summary>New movement mode</summary>
    public int NewMode { get; init; }
}
