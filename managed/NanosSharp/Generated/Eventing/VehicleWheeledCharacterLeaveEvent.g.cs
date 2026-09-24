#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character fully leaves the Vehicle</summary>
public sealed class VehicleWheeledCharacterLeaveEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;

    /// <summary>The Character</summary>
    public Character? Character { get; init; }

    /// <summary>The seat left, 0 is the driver</summary>
    public int SeatIndex { get; init; }
}
