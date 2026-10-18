#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the Character effectively attacks with a Melee</summary>
public sealed class CharacterGASPAttackEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Melee</summary>
    public Melee? Melee { get; init; }
}
