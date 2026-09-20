#nullable enable

namespace NanosSharp.Enums;

/// <summary>How the front wheels steer</summary>
public enum SteeringType
{
    /// <summary>Both wheels steer by the same amount</summary>
    SingleAngle = 0,
    /// <summary>Outer wheels on corner steer less than the inner ones by set ratio</summary>
    AngleRatio = 1,
    /// <summary>Ackermann steering principle is applied</summary>
    Ackermann = 2,
}
