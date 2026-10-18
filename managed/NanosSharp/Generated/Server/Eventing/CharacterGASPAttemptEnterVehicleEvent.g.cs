#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character attempts to enter a vehicle</summary>
public sealed class CharacterGASPAttemptEnterVehicleEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Vehicle</summary>
    public Entity? Vehicle { get; init; }

    /// <summary>The seat it is trying to enter, 0 is the driver seat</summary>
    public int SeatIndex { get; init; }
}
