#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a physics constraint axis can move</summary>
public enum ConstraintMotion
{
    /// <summary>No constraint around this axis.</summary>
    Free = 0,
    /// <summary>Limited freedom around this axis. The limit for each Motion is controlled individually by a correspondingly named Limit property.</summary>
    Limited = 1,
    /// <summary>Full constraint around this axis</summary>
    Locked = 2,
}
