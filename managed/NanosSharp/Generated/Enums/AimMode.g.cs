#nullable enable

namespace NanosSharp.Enums;

/// <summary>Used by Characters to change the Aiming Mode while holding Weapons</summary>
public enum AimMode
{
    /// <summary>Not aiming</summary>
    None = 0,
    /// <summary>Aiming down sights</summary>
    ADS = 1,
    /// <summary>Aiming through a scope, zoomed in</summary>
    ZoomedZoom = 2,
    /// <summary>Third person zoomed aim</summary>
    Zoomed = 3,
    /// <summary>Third person zoomed aim, farther away</summary>
    ZoomedFar = 4,
}
