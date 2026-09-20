#nullable enable

namespace NanosSharp.Enums;

/// <summary>Which camera views a Player can switch between</summary>
public enum CameraMode
{
    /// <summary>Can switch between first and third person</summary>
    FPSTPS = 0,
    /// <summary>First person only</summary>
    FPSOnly = 1,
    /// <summary>Third person only</summary>
    TPSOnly = 2,
}
