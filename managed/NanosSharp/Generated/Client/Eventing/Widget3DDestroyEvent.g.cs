#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when an Entity is destroyed</summary>
public sealed class Widget3DDestroyEvent : ScriptEvent
{
    /// <summary>The Entity which that was destroyed</summary>
    public Widget3D Widget3D { get; init; } = null!;
}
