#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity Respawns</summary>
public sealed class VehicleWaterRespawnEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public VehicleWater VehicleWater { get; init; } = null!;
}
