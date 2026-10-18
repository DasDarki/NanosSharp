#nullable enable

namespace NanosSharp.Enums;

/// <summary>Continuous Collision Detection mode, which prevents fast objects from passing through others</summary>
public enum CCDMode
{
    /// <summary>Automatically enables/disables depending on the Object size</summary>
    Auto = 0,
    /// <summary>Disables CCD</summary>
    Disabled = 1,
    /// <summary>Enables CCD</summary>
    Enabled = 2,
}
