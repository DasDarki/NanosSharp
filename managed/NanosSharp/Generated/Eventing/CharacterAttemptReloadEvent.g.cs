#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Character attempts to reload</summary>
public sealed class CharacterAttemptReloadEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The Weapon</summary>
    public Weapon? Weapon { get; init; }
}
