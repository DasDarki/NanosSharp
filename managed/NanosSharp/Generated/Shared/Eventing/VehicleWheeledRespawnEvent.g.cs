#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Entity Respawns</summary>
public sealed class VehicleWheeledRespawnEvent : ScriptEvent
{
    /// <summary>The Damageable that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;
}
