#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this page is fully loaded</summary>
public sealed class WebUIReadyEvent : ScriptEvent
{
    /// <summary>The Web UI that triggered the event</summary>
    public WebUI WebUI { get; init; } = null!;
}
