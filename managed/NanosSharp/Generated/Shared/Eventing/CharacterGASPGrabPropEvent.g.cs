#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character grabs up a Prop</summary>
public sealed class CharacterGASPGrabPropEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Prop</summary>
    public Prop? Prop { get; init; }
}
