#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Called Every Frame. Do not abuse</summary>
public sealed class ClientTickEvent : ScriptEvent
{
    /// <summary>Seconds since the last frame</summary>
    public float DeltaTime { get; init; }
}
