#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called when the map changes</summary>
public sealed class ServerChangeMapEvent : ScriptEvent
{
    /// <summary>The previous map</summary>
    public string? OldMap { get; init; }

    /// <summary>The new map</summary>
    public string? NewMap { get; init; }
}
