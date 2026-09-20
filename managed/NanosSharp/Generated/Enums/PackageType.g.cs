#nullable enable

namespace NanosSharp.Enums;

/// <summary>Types of packages</summary>
public enum PackageType
{
    /// <summary>Script package</summary>
    Script = 1,
    /// <summary>Game-mode package, only one can be loaded</summary>
    GameMode = 2,
    /// <summary>Loading screen package</summary>
    LoadingScreen = 4,
    /// <summary>Map package</summary>
    Map = 16,
    /// <summary>C Module package</summary>
    CModule = 32,
    /// <summary>All types</summary>
    All = -1,
}
