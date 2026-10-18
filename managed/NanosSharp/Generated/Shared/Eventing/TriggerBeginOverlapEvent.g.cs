#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when something overlaps this Trigger</summary>
public sealed class TriggerBeginOverlapEvent : ScriptEvent
{
    /// <summary>The Trigger entity</summary>
    public Trigger Trigger { get; init; } = null!;

    /// <summary>Any Actor which overlaps</summary>
    public Actor? Entity { get; init; }
}
