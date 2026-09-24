#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character attempts to leave the Vehicle</summary>
public sealed class VehicleWheeledCharacterAttemptLeaveEvent : CancellableEvent
{
    /// <summary>The Vehicle that triggered the event</summary>
    public VehicleWheeled VehicleWheeled { get; init; } = null!;

    /// <summary>The Character</summary>
    public Character? Character { get; init; }
}
