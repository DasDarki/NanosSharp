#nullable enable

namespace NanosSharp.Enums;

/// <summary>Cloud and sky rendering presets</summary>
public enum SkyMode
{
    /// <summary>Uses a 3D Volumetric Cloud layer</summary>
    VolumetricClouds = 0,
    /// <summary>Designed to mimic the look of the volumetric clouds but as a static cloud texture. Much lower performance cost than volumetric clouds.</summary>
    StaticClouds = 1,
    /// <summary>Uses simple 2D dynamic clouds</summary>
    DynamicClouds2D = 2,
    /// <summary>Disables all forms of dynamic clouds</summary>
    NoClouds = 3,
    /// <summary>Trades clouds for a full 3D volumetric aurora, much more convincing than the 2D effect.</summary>
    VolumetricAurora = 4,
    /// <summary>Turns off all sky color, atmospheric scattering, clouds, etc. Specifically for environments which are set in outer space with no planet/ground beneath the camera.</summary>
    Space = 5,
}
