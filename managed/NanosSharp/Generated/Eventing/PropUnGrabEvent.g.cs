#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this Prop is ungrabbed</summary>
public sealed class PropUnGrabEvent : ScriptEvent
{
    /// <summary>The prop which has been ungrabbed</summary>
    public Prop Prop { get; init; } = null!;

    /// <summary>The old Grabber</summary>
    public Character? Character { get; init; }
}
