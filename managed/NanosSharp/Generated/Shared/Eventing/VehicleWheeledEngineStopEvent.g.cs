#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the engine stops</summary>
public sealed class VehicleWheeledEngineStopEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;
}
