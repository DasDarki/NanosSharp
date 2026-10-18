#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character releases the use button for this Pickable (i.e. releases left mouse button with this equipped)</summary>
public sealed class MeleeReleaseUseEvent : ScriptEvent
{
    /// <summary>The Pickable which has just stopped being used</summary>
    public Melee Melee { get; init; } = null!;

    /// <summary>The Character that stopped using it</summary>
    public Character? Character { get; init; }
}
