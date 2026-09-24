#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered every 33ms by default</summary>
public sealed class ServerTickEvent : ScriptEvent
{
    /// <summary>Seconds since the last tick</summary>
    public float DeltaTime { get; init; }
}
