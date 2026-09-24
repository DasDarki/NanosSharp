#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character fully enters the Vehicle</summary>
public sealed class VehicleWheeledCharacterEnterEvent : ScriptEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;

    /// <summary>The Character</summary>
    public Character? Character { get; init; }

    /// <summary>The seat entered, 0 is the driver</summary>
    public int SeatIndex { get; init; }
}
