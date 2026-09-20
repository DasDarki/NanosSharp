#nullable enable

namespace NanosSharp.Enums;

/// <summary>How a drawing blends with what is already drawn</summary>
public enum BlendMode
{
    /// <summary>Fully replaces the pixels, ignoring alpha</summary>
    Opaque = 0,
    /// <summary>Pixels are either fully visible or fully hidden</summary>
    Masked = 1,
    /// <summary>Blends using the alpha</summary>
    Translucent = 2,
    /// <summary>Adds its color to the existing pixels</summary>
    Additive = 3,
    /// <summary>Multiplies its color with the existing pixels</summary>
    Modulate = 4,
    MaskedDistanceField = 5,
    MaskedDistanceFieldShadowed = 6,
    TranslucentDistanceField = 7,
    TranslucentDistanceFieldShadowed = 8,
    AlphaComposite = 9,
    AlphaHoldout = 10,
    /// <summary>Standard alpha blending, recommended for images with transparency</summary>
    AlphaBlend = 11,
    TranslucentAlphaOnly = 12,
    TranslucentAlphaOnlyWriteAlpha = 13,
}
