#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Actor leaves a water body</summary>
public sealed class LightLeaveWaterEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public Light Light { get; init; } = null!;
}
