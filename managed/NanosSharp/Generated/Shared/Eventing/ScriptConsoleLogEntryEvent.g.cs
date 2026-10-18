#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a log is received</summary>
public sealed class ScriptConsoleLogEntryEvent : ScriptEvent
{
    /// <summary>Log Message</summary>
    public string? Text { get; init; }

    /// <summary>Type of the log</summary>
    public LogType Type { get; init; }
}
