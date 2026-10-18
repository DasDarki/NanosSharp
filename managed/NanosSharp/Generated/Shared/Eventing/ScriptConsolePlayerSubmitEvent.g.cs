#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when a console command is submitted</summary>
public sealed class ScriptConsolePlayerSubmitEvent : ScriptEvent
{
    /// <summary>Text submitted</summary>
    public string? Text { get; init; }
}
