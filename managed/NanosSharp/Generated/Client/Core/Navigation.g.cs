#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Creates a reachable path into navigable space.</summary>
public static partial class Navigation
{
    private static readonly StaticMethodSlot FindPathToLocationSlot = new("Navigation", "FindPathToLocation");
    private static readonly StaticMethodSlot GetRandomPointInNavigableRadiusSlot = new("Navigation", "GetRandomPointInNavigableRadius");
    private static readonly StaticMethodSlot GetRandomReachablePointInRadiusSlot = new("Navigation", "GetRandomReachablePointInRadius");

    /// <summary>Finds a Path given Start and End location</summary>
    public static IReadOnlyDictionary<string, object?> FindPathToLocation(Vector startLocation, Vector endLocation) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector, Vector>(FindPathToLocationSlot, 2, startLocation, endLocation)!;

    /// <summary>Finds random, point in navigable space restricted to Radius around Origin. Resulting location is not tested for reachability from the Origin</summary>
    public static Vector GetRandomPointInNavigableRadius(Vector origin, float radius) =>
        ScriptApi.CallResult<Vector, Vector, float>(GetRandomPointInNavigableRadiusSlot, 2, origin, radius)!;

    /// <summary>Finds random, reachable point in navigable space restricted to radius around origin</summary>
    public static Vector GetRandomReachablePointInRadius(Vector origin, float radius) =>
        ScriptApi.CallResult<Vector, Vector, float>(GetRandomReachablePointInRadiusSlot, 2, origin, radius)!;
}
