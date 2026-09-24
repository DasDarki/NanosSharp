#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity has its Health changed, or because took damage or manually set through scripting or respawning</summary>
public sealed class CharacterSimpleHealthChangeEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;

    /// <summary>Health before the change</summary>
    public int OldHealth { get; init; }

    /// <summary>Health after the change</summary>
    public int NewHealth { get; init; }
}
