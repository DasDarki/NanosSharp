#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When the Ammo Clip is changed, by reloading or manually setting through scripting</summary>
public sealed class WeaponAmmoClipChangeEvent : ScriptEvent
{
    /// <summary>The Weapon that triggered the event</summary>
    public Weapon Weapon { get; init; } = null!;

    /// <summary>The previous clip ammo</summary>
    public int OldAmmoClip { get; init; }

    /// <summary>The new clip ammo</summary>
    public int NewAmmoClip { get; init; }
}
