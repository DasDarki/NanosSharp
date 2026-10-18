#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is spawned/created</summary>
public sealed class WeaponSpawnEvent : ScriptEvent
{
    /// <summary>The Entity that was spawned</summary>
    public Weapon Weapon { get; init; } = null!;
}
