#nullable enable

namespace NanosSharp.Enums;

/// <summary>Whether a Player can talk and listen on a VOIP channel</summary>
public enum VOIPSetting
{
    /// <summary>Can neither talk nor listen</summary>
    None = 0,
    /// <summary>Can only listen</summary>
    ListenOnly = 1,
    /// <summary>Can only talk</summary>
    SpeakOnly = 2,
    /// <summary>Can talk and listen</summary>
    Both = 3,
}
