#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is spawned/created</summary>
public sealed class CharacterGASPSpawnEvent : ScriptEvent
{
    /// <summary>The Entity that was spawned</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;
}
