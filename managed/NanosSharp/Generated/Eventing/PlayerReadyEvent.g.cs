#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when Player is ready (the client fully joined, loaded the map and all entities and is ready to play)</summary>
public sealed class PlayerReadyEvent : ScriptEvent
{
    /// <summary>The Player that triggered the event</summary>
    public Player Player { get; init; } = null!;
}
