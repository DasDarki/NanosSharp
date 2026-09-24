#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered When a Character picks this up</summary>
public sealed class GrenadePickUpEvent : ScriptEvent
{
    /// <summary>The Pickable that just got picked up</summary>
    public Grenade Grenade { get; init; } = null!;

    /// <summary>The Character that picked it up</summary>
    public Character? Character { get; init; }
}
