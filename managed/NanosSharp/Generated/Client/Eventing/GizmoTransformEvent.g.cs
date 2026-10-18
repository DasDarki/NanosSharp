#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when the Gizmo has its transform updated</summary>
public sealed class GizmoTransformEvent : ScriptEvent
{
    /// <summary>The Gizmo itself</summary>
    public Gizmo Gizmo { get; init; } = null!;

    /// <summary>New Gizmo location</summary>
    public Vector Location { get; init; }

    /// <summary>New Gizmo rotation</summary>
    public Rotator Rotation { get; init; }

    /// <summary>New Gizmo scale</summary>
    public Vector Scale { get; init; }
}
