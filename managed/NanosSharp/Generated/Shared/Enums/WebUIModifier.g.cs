#nullable enable

namespace NanosSharp.Enums;

/// <summary>Modifier keys and flags of a WebUI input event</summary>
[Flags]
public enum WebUIModifier
{
    None = 0,
    CapsLockOn = 1,
    ShiftDown = 2,
    ControlDown = 4,
    AltDown = 8,
    LeftMouseButton = 16,
    MiddleMouseButton = 32,
    RightMouseButton = 64,
    CommandDown = 128,
    NumLockOn = 256,
    IsKeyPad = 512,
    IsLeft = 1024,
    IsRight = 2048,
    AltgrDown = 4096,
    IsRepeat = 8192,
}
