#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the Character effectively attacks with this Melee</summary>
public sealed class MeleeAttackEvent : ScriptEvent
{
    /// <summary>The Melee that triggered the event</summary>
    public Melee Melee { get; init; } = null!;

    /// <summary>The Character attacking</summary>
    public Character? Handler { get; init; }
}
