#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the engine starts</summary>
public sealed class VehicleWheeledEngineStartEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;
}
