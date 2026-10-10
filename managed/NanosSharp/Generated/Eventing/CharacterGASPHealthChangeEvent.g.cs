#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
public sealed class CharacterGASPHealthChangeEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>Health before the change</summary>
    public int OldHealth { get; init; }

    /// <summary>Health after the change</summary>
    public int NewHealth { get; init; }
}
