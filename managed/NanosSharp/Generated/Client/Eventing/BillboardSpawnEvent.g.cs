#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is spawned/created</summary>
public sealed class BillboardSpawnEvent : ScriptEvent
{
    /// <summary>The Entity that was spawned</summary>
    public Billboard Billboard { get; init; } = null!;
}
