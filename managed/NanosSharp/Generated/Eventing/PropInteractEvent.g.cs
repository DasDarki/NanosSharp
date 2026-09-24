#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When a Character interacts with this Prop (i.e. try to Grab it)</summary>
public sealed class PropInteractEvent : CancellableEvent
{
    /// <summary>the object that undergoes an interaction</summary>
    public Prop Prop { get; init; } = null!;

    /// <summary>the character who interacts with the object</summary>
    public Character? Character { get; init; }
}
