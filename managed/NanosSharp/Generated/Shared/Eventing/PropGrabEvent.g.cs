#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when Character grabs a Prop</summary>
public sealed class PropGrabEvent : ScriptEvent
{
    /// <summary>The prop which was Grabbed</summary>
    public Prop Prop { get; init; } = null!;

    /// <summary>The new Grabber</summary>
    public Character? Character { get; init; }
}
