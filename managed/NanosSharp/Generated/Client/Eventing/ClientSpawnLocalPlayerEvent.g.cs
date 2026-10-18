#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the local player spawns (just after the game has loaded)</summary>
public sealed class ClientSpawnLocalPlayerEvent : ScriptEvent
{
    /// <summary>The local Player</summary>
    public Player? LocalPlayer { get; init; }
}
