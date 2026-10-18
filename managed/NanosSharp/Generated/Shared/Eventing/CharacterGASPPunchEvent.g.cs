#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character punches</summary>
public sealed class CharacterGASPPunchEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;
}
