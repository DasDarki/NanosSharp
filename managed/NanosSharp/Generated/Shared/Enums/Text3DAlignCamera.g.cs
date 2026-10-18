#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a Text3D rotates relative to the camera</summary>
public enum Text3DAlignCamera
{
    /// <summary>Keeps its own rotation</summary>
    Unaligned = 0,
    /// <summary>Rotates to match the camera rotation</summary>
    AlignCameraRotation = 1,
    /// <summary>Always faces the camera location</summary>
    FaceCamera = 2,
}
