#nullable enable

namespace NanosSharp.Enums;

/// <summary>The swimming state of a Character</summary>
public enum SwimmingMode
{
    /// <summary>Not swimming</summary>
    None = 0,
    /// <summary>Swimming on the surface</summary>
    Surface = 1,
    /// <summary>Diving underwater</summary>
    Underwater = 2,
}
