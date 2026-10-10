#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character drops the currently picked up Pickable</summary>
public sealed class CharacterGASPDropEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Pickable being dropped</summary>
    public Entity? Object { get; init; }

    /// <summary>Whether the Player dropped it by input, instead of by scripting</summary>
    public bool TriggeredByPlayer { get; init; }
}
