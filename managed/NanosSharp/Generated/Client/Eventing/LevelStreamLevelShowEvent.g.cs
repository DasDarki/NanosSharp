#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a Stream Level is shown</summary>
public sealed class LevelStreamLevelShowEvent : ScriptEvent
{
    /// <summary>Name of the Stream Level</summary>
    public string? LevelName { get; init; }
}
