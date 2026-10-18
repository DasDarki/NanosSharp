#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When a Weapon is reloaded, optionally by a Character</summary>
public sealed class WeaponReloadEvent : ScriptEvent
{
    /// <summary>The Weapon that triggered the event</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>The Character</summary>
    public Character? Character { get; init; }

    /// <summary>How many bullets were reloaded</summary>
    public int AmmoToReload { get; init; }
}
