#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Character attempts to reload</summary>
public sealed class CharacterGASPAttemptReloadEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Weapon</summary>
    public Weapon? Weapon { get; init; }
}
