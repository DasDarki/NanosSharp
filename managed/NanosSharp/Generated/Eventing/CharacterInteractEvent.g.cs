#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
public sealed class CharacterInteractEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The Prop or Pickable being interacted with</summary>
    public Prop? Object { get; init; }
}
