#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character drops a Prop</summary>
public sealed class CharacterUnGrabPropEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public Character Character { get; init; } = null!;

    /// <summary>The Prop</summary>
    public Prop? Prop { get; init; }
}
