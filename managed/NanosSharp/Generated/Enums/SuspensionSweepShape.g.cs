#nullable enable

namespace NanosSharp.Enums;

/// <summary>Shape used to detect the ground under each wheel</summary>
public enum SuspensionSweepShape
{
    /// <summary>Use ray to determine suspension length to ground - fastest</summary>
    Raycast = 0,
    /// <summary>Use sphere to determine suspension length to ground</summary>
    Spherecast = 1,
    /// <summary>Use wheel collision shape to determine suspension length to ground - Slowest</summary>
    Shapecast = 2,
}
