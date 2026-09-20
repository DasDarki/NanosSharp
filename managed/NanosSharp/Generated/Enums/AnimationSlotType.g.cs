#nullable enable

namespace NanosSharp.Enums;

/// <summary>Which part of the Character body plays an Animation</summary>
public enum AnimationSlotType
{
    /// <summary>The whole body</summary>
    FullBody = 0,
    /// <summary>From the waist up, keeping the legs animation</summary>
    UpperBody = 1,
    /// <summary>Only the head</summary>
    Head = 2,
}
