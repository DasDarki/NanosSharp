#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the grenade has been thrown</summary>
public sealed class GrenadeThrowEvent : ScriptEvent
{
    /// <summary>The grenade which exploded</summary>
    public Grenade Grenade { get; init; } = null!;

    /// <summary>The Character which has thrown it</summary>
    public Character? Handler { get; init; }
}
