#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity Respawns</summary>
public sealed class CharacterSimpleRespawnEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public CharacterSimple CharacterSimple { get; init; } = null!;
}
