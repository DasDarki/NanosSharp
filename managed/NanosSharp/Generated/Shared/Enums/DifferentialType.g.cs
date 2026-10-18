#nullable enable

namespace NanosSharp.Enums;

/// <summary>Which wheels receive the engine torque</summary>
public enum DifferentialType
{
    /// <summary>Not configured</summary>
    Undefined = 0,
    /// <summary>All wheels</summary>
    AllWheelDrive = 1,
    /// <summary>Only the front wheels</summary>
    FrontWheelDrive = 2,
    /// <summary>Only the rear wheels</summary>
    RearWheelDrive = 3,
}
