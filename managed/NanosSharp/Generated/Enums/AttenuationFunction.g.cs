#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a 3D Sound volume fades with distance</summary>
public enum AttenuationFunction
{
    /// <summary>Fades evenly with distance</summary>
    Linear = 0,
    /// <summary>Fades quickly when close and slowly when far</summary>
    Logarithmic = 1,
    /// <summary>Fades very quickly near the source</summary>
    Inverse = 2,
    /// <summary>Fades slowly when close and quickly when far</summary>
    LogReverse = 3,
    /// <summary>Physically based falloff, the most realistic</summary>
    NaturalSound = 4,
}
