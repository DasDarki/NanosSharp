#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Actor enters a water body</summary>
public sealed class InstancedStaticMeshEnterWaterEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public InstancedStaticMesh InstancedStaticMesh { get; init; } = null!;
}
