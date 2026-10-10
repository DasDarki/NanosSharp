#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character leaves a vehicle</summary>
public sealed class CharacterGASPLeaveVehicleEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Vehicle</summary>
    public Entity? Vehicle { get; init; }

    /// <summary>The seat it left, 0 is the driver seat</summary>
    public int SeatIndex { get; init; }
}
