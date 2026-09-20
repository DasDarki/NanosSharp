#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a Character holds a Pickable, which defines the animations used</summary>
public enum HandlingMode
{
    SingleHandedWeapon = 0,
    DoubleHandedWeapon = 1,
    SingleHandedMelee = 2,
    DoubleHandedMelee = 3,
    /// <summary>Grenades and other throwables</summary>
    Throwable = 4,
    /// <summary>A light source held in one hand</summary>
    Torch = 5,
    /// <summary>Heavy objects carried in front with both hands</summary>
    Barrel = 6,
    /// <summary>Boxes carried with both hands</summary>
    Box = 7,
}
