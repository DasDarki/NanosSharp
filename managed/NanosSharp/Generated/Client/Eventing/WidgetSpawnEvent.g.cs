#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is spawned/created</summary>
public sealed class WidgetSpawnEvent : ScriptEvent
{
    /// <summary>The Entity that was spawned</summary>
    public Widget Widget { get; init; } = null!;
}
