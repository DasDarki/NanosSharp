#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character interacts with this Pickable (i.e. tries to pick it up)</summary>
public sealed class MeleeInteractEvent : CancellableEvent
{
    /// <summary>The Pickable that just got interacted with</summary>
    public Melee Melee { get; init; } = null!;

    /// <summary>The Character that interacted with it</summary>
    public Character? Character { get; init; }
}
