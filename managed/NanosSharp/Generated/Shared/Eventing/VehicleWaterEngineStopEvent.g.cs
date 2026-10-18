#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the engine stops</summary>
public sealed class VehicleWaterEngineStopEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWater VehicleWater { get; init; } = null!;
}
