#nullable enable

namespace NanosSharp.Enums;

/// <summary>Types of keyboard events sent to a WebUI</summary>
public enum WebUIKeyType
{
    /// <summary>Key pressed</summary>
    Down = 0,
    /// <summary>Key released</summary>
    Up = 1,
    /// <summary>Character typed</summary>
    Char = 2,
}
