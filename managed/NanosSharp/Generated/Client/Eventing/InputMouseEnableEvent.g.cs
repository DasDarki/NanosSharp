#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When mouse cursor is displayed/hidden</summary>
public sealed class InputMouseEnableEvent : ScriptEvent
{
    /// <summary>Whether the cursor is now displayed</summary>
    public bool IsEnabled { get; init; }
}
