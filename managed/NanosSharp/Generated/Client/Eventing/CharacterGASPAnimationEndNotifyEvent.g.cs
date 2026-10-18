#nullable enable

using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Mathematics;

namespace NanosSharp.Eventing;

/// <summary>When an Animation Montage Notify ends</summary>
public sealed class CharacterGASPAnimationEndNotifyEvent : ScriptEvent
{
    /// <summary>The Pawn that triggered the event</summary>
    public CharacterGASP CharacterGASP { get; init; } = null!;

    /// <summary>Name of the Animation Notify</summary>
    public string? NotifyName { get; init; }

    /// <summary>Name of the Animation Montage that fired the notify</summary>
    public string? AnimationName { get; init; }

    /// <summary>Time in the animation when the notify begins, in seconds</summary>
    public double TriggerBeginTime { get; init; }

    /// <summary>Time in the animation when the notify ends, in seconds</summary>
    public double TriggerEndTime { get; init; }
}
