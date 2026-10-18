#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the Client language changes</summary>
public sealed class ClientLanguageChangeEvent : ScriptEvent
{
    /// <summary>The new language code</summary>
    public string? Language { get; init; }
}
