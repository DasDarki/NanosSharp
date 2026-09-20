#nullable enable

namespace NanosSharp.Enums;

/// <summary>The airborne state of a Character</summary>
public enum FallingMode
{
    /// <summary>Not falling</summary>
    None = 0,
    /// <summary>Going up after a jump</summary>
    Jumping = 1,
    /// <summary>Falling</summary>
    Falling = 4,
    /// <summary>Falling from a height that will cause damage</summary>
    HighFalling = 5,
    /// <summary>Parachute deployed</summary>
    Parachuting = 6,
    /// <summary>Skydiving before opening the parachute</summary>
    SkyDiving = 7,
}
