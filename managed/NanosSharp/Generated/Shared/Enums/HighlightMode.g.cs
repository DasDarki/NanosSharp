#nullable enable

namespace NanosSharp.Enums;

/// <summary>When an Actor highlight is visible</summary>
public enum HighlightMode
{
    /// <summary>Always visible</summary>
    Always = 0,
    /// <summary>Only when occluded by other objects (x-ray)</summary>
    OnlyHidden = 1,
    /// <summary>Only when not occluded</summary>
    OnlyVisible = 2,
}
