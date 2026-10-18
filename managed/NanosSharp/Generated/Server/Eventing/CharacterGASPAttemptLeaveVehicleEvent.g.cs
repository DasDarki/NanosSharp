#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Character attempts to leave a vehicle</summary>
public sealed class CharacterGASPAttemptLeaveVehicleEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Vehicle</summary>
    public Entity? Vehicle { get; init; }
}
