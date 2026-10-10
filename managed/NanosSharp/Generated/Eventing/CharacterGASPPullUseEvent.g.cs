#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character presses the use button for a Pickable (i.e. clicks left mouse button with this equipped)</summary>
public sealed class CharacterGASPPullUseEvent : ScriptEvent
{
    /// <summary>The Character that used it</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Pickable which has just been used</summary>
    public Entity? Pickable { get; init; }
}
