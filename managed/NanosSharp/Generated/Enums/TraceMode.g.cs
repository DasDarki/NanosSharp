#nullable enable

namespace NanosSharp.Enums;

/// <summary>Options of a Trace, which define what it returns and how it is performed</summary>
[Flags]
public enum TraceMode
{
    /// <summary>Whether to draw a 3D line representing the ray</summary>
    DrawDebug = 1,
    /// <summary>Whether should trace against complex collision</summary>
    TraceComplex = 2,
    /// <summary>Whether want to include the physical material in the results (adds <c>SurfaceType</c> to result)</summary>
    ReturnPhysicalMaterial = 4,
    /// <summary>Whether want to return the Entity hit (adds <c>Entity</c> to result)</summary>
    ReturnEntity = 8,
    /// <summary>Whether should return names (adds <c>ActorName</c>, <c>ComponentName</c> and <c>BoneName</c> to result)</summary>
    ReturnNames = 16,
    /// <summary>Whether want to trace and return UV information (adds <c>UV</c> to result)</summary>
    ReturnUV = 32,
    /// <summary>Whether should only trace for objects that don't ignore visibility collision channel</summary>
    TraceOnlyVisibility = 64,
    /// <summary>Whether should return extra data about item that was hit (hit primitive specific - usually the instanced index) (adds <c>Item</c> to result)</summary>
    ReturnItem = 128,
}
