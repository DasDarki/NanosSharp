#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
public sealed class Text3DNetworkAuthorityChangeEvent : ScriptEvent
{
    /// <summary>The Actor that triggered the event</summary>
    public Text3D Text3D { get; init; } = null!;

    /// <summary>Whether the local Player is now the Network Authority of this Actor</summary>
    public bool IsNetworkAuthority { get; init; }
}
