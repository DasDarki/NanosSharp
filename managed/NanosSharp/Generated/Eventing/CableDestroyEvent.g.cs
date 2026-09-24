#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is destroyed</summary>
public sealed class CableDestroyEvent : ScriptEvent
{
    /// <summary>The Entity which that was destroyed</summary>
    public Cable Cable { get; init; } = null!;
}
