#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>Triggered when this SceneCapture does an update/renders a frame</summary>
public sealed class SceneCaptureCaptureEvent : ScriptEvent
{
    /// <summary>The Scene Capture that triggered the event</summary>
    public SceneCapture SceneCapture { get; init; } = null!;
}
