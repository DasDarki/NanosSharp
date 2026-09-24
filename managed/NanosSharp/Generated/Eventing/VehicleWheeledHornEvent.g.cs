#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when Vehicle honks</summary>
public sealed class VehicleWheeledHornEvent : ScriptEvent
{
    /// <summary>The Vehicle Wheeled that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;

    /// <summary>Whether it started (true) or stopped (false) honking</summary>
    public bool IsHonking { get; init; }
}
