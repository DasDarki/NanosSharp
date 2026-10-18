#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character interacts with a Prop or Pickable</summary>
public sealed class CharacterGASPInteractEvent : CancellableEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Prop or Pickable being interacted with</summary>
    public Prop? Object { get; init; }
}
