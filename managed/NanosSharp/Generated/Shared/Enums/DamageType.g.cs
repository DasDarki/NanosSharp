#nullable enable

namespace NanosSharp.Enums;

/// <summary>What caused some damage</summary>
public enum DamageType
{
    /// <summary>Shot by a Weapon</summary>
    Shot = 0,
    /// <summary>An explosion, such as a Grenade</summary>
    Explosion = 1,
    /// <summary>A punch</summary>
    Punch = 2,
    /// <summary>Falling from a high place</summary>
    Fall = 3,
    /// <summary>Hit by a Prop</summary>
    RunOverProp = 4,
    /// <summary>Run over by a Vehicle</summary>
    RunOverVehicle = 5,
    /// <summary>Hit by a Melee</summary>
    Melee = 6,
    /// <summary>Anything else, such as scripted damage</summary>
    Unknown = 7,
}
