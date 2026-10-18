#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the game is focused/unfocused</summary>
public sealed class ClientWindowFocusChangeEvent : ScriptEvent
{
    /// <summary>If it's focused</summary>
    public bool IsFocused { get; init; }
}
