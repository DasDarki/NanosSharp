#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Actor enters a water body</summary>
public sealed class StaticMeshEnterWaterEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public StaticMesh StaticMesh { get; init; } = null!;
}
