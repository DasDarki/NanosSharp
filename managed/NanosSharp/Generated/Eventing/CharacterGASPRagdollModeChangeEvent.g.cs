#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character enters or leaves ragdoll</summary>
public sealed class CharacterGASPRagdollModeChangeEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The previous boolean</summary>
    public bool OldState { get; init; }

    /// <summary>The new boolean</summary>
    public bool NewState { get; init; }
}
