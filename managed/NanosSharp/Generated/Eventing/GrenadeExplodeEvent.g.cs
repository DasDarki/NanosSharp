#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the grenade has exploded</summary>
public sealed class GrenadeExplodeEvent : ScriptEvent
{
    /// <summary>The grenade which exploded</summary>
    public Grenade Grenade { get; init; } = null!;
}
