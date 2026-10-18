#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when a Character releases the use button for a Pickable (i.e. releases left mouse button with this equipped)</summary>
public sealed class CharacterGASPReleaseUseEvent : ScriptEvent
{
    /// <summary>The Character that stopped using it</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Pickable which has just stopped being used</summary>
    public Entity? Pickable { get; init; }
}
