#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is spawned/created</summary>
public sealed class Widget3DSpawnEvent : ScriptEvent
{
    /// <summary>The Entity that was spawned</summary>
    public Widget3D Widget3D { get; init; } = null!;
}
