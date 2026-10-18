#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a Stream Level is unloaded</summary>
public sealed class LevelStreamLevelUnloadEvent : ScriptEvent
{
    /// <summary>Name of the Stream Level</summary>
    public string? LevelName { get; init; }
}
