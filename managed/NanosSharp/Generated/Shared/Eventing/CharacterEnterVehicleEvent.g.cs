#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character enters a vehicle</summary>
public sealed class CharacterEnterVehicleEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The Vehicle</summary>
    public Entity? Vehicle { get; init; }

    /// <summary>The seat entered, 0 is the driver seat</summary>
    public int SeatIndex { get; init; }
}
