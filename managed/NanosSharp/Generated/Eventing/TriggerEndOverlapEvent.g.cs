#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when something leaves this Trigger</summary>
public sealed class TriggerEndOverlapEvent : ScriptEvent
{
    /// <summary>The Trigger entity</summary>
    public Trigger Trigger { get; init; } = null!;

    /// <summary>Any Actor which left the Trigger</summary>
    public Actor? Entity { get; init; }
}
