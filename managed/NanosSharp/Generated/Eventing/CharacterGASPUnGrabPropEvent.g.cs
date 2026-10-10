#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character drops a Prop</summary>
public sealed class CharacterGASPUnGrabPropEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Prop</summary>
    public Prop? Prop { get; init; }
}
