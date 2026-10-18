#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character highlights/looks at a Prop or a Pickable</summary>
public sealed class CharacterHighlightEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>Whether the object is being highlighted or not</summary>
    public bool IsHighlighted { get; init; }

    /// <summary>The Prop or Pickable being highlighted</summary>
    public Prop? Object { get; init; }
}
