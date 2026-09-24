#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity Respawns</summary>
public sealed class CharacterRespawnEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public Character Character { get; init; } = null!;
}
