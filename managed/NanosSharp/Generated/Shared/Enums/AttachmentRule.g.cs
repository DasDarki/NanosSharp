#nullable enable

namespace NanosSharp.Enums;

/// <summary>How an Actor transform is handled when attaching it to another Actor</summary>
public enum AttachmentRule
{
    /// <summary>Will keep the current relative position/rotation if already attached.</summary>
    KeepRelative = 0,
    /// <summary>Will calculate the new relative position/rotation so the Actor stays at the same position after being attached.</summary>
    KeepWorld = 1,
    /// <summary>Will set the Actor to the same position/rotation as the parent actor (or at the bone location) and reset its relative position/rotation to zero.</summary>
    SnapToTarget = 2,
}
