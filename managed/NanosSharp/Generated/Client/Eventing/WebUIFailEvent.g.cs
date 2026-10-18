#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this page fails to load</summary>
public sealed class WebUIFailEvent : ScriptEvent
{
    /// <summary>The Web UI that triggered the event</summary>
    public WebUI WebUI { get; init; } = null!;

    /// <summary>The CEF error code</summary>
    public int ErrorCode { get; init; }

    /// <summary>The error message</summary>
    public string? Message { get; init; }
}
