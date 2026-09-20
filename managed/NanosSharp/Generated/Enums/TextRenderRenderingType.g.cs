#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a TextRender is rendered</summary>
public enum TextRenderRenderingType
{
    /// <summary>Affected by the scene lighting</summary>
    Lit = 0,
    /// <summary>Ignores lighting, always fully bright</summary>
    Unlit = 1,
    /// <summary>Unlit and always facing the camera</summary>
    UnlitCameraAligned = 2,
    /// <summary>Unlit, facing the camera and occluded by the scene</summary>
    UnlitCameraAlignedDepth = 3,
}
