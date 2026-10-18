#nullable enable

namespace NanosSharp.Enums;

/// <summary>Whether a Prop can be grabbed by Characters</summary>
public enum GrabMode
{
    /// <summary>Disables grabbing</summary>
    Disabled = 0,
    /// <summary>Automatically enables/disables depending on the Prop size</summary>
    Auto = 1,
    /// <summary>Enables grabbing</summary>
    Enabled = 2,
}
