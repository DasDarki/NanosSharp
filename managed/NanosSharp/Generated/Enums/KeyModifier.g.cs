#nullable enable

namespace NanosSharp.Enums;

/// <summary>Modifier keys held during a key event</summary>
[Flags]
public enum KeyModifier
{
    None = 0,
    LeftShiftDown = 1,
    RightShiftDown = 2,
    LeftControlDown = 4,
    RightControlDown = 8,
    LeftAltDown = 16,
    RightAltDown = 32,
    LeftCommandDown = 64,
    RightCommandDown = 128,
    CapsLocked = 256,
}
