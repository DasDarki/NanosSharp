#nullable enable

namespace NanosSharp.Enums;

/// <summary>Collision profiles of an Actor</summary>
public enum CollisionType
{
    /// <summary>Blocks All</summary>
    Normal = 0,
    /// <summary>Only Blocks Static objects</summary>
    StaticOnly = 1,
    /// <summary>Doesn't Block anything</summary>
    NoCollision = 2,
    /// <summary>Blocks everything but Pawns (Characters)</summary>
    IgnoreOnlyPawn = 3,
    /// <summary>Automatically selects - usually will be Normal. On Props it will switch between Normal and IgnoreOnlyPawn depending on the Prop size</summary>
    Auto = 4,
}
