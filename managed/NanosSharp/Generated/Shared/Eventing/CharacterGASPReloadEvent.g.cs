#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When Character reloads a weapon</summary>
public sealed class CharacterGASPReloadEvent : ScriptEvent
{
    /// <summary>The Character that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>The Weapon</summary>
    public Weapon? Weapon { get; init; }

    /// <summary>How many bullets are being reloaded</summary>
    public int AmmoToReload { get; init; }
}
