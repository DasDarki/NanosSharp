#nullable enable

namespace NanosSharp.Enums;

/// <summary>The movement speed state of a Character</summary>
public enum GaitMode
{
    /// <summary>Not moving</summary>
    None = 0,
    /// <summary>Walking or running</summary>
    Walking = 1,
    /// <summary>Sprinting</summary>
    Sprinting = 2,
}
