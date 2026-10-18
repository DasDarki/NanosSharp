#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Trace a ray against the world and get collided objects information.</summary>
public static partial class Trace
{
    private static readonly StaticMethodSlot BoxMultiSlot = new("Trace", "BoxMulti");
    private static readonly StaticMethodSlot BoxSingleSlot = new("Trace", "BoxSingle");
    private static readonly StaticMethodSlot CapsuleMultiSlot = new("Trace", "CapsuleMulti");
    private static readonly StaticMethodSlot CapsuleSingleSlot = new("Trace", "CapsuleSingle");
    private static readonly StaticMethodSlot LineMultiSlot = new("Trace", "LineMulti");
    private static readonly StaticMethodSlot LineSingleSlot = new("Trace", "LineSingle");
    private static readonly StaticMethodSlot SphereMultiSlot = new("Trace", "SphereMulti");
    private static readonly StaticMethodSlot SphereSingleSlot = new("Trace", "SphereSingle");

    /// <summary>Trace a box against the world using object types and return overlapping hits and then first blocking hit</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> BoxMulti(
        Vector startLocation,
        Vector endLocation,
        Vector halfSize,
        Rotator orientation,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, Vector, Vector, Vector, Rotator, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(BoxMultiSlot, 4, startLocation, endLocation, halfSize, orientation, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a box against the world and returns a table with the first blocking hit information</summary>
    public static IReadOnlyDictionary<string, object?> BoxSingle(
        Vector startLocation,
        Vector endLocation,
        Vector halfSize,
        Rotator orientation,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector, Vector, Vector, Rotator, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(BoxSingleSlot, 4, startLocation, endLocation, halfSize, orientation, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a capsule against the world using object types and return overlapping hits and then first blocking hit</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> CapsuleMulti(
        Vector startLocation,
        Vector endLocation,
        float radius,
        float halfHeight,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, Vector, Vector, float, float, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(CapsuleMultiSlot, 4, startLocation, endLocation, radius, halfHeight, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a capsule against the world and returns a table with the first blocking hit information</summary>
    public static IReadOnlyDictionary<string, object?> CapsuleSingle(
        Vector startLocation,
        Vector endLocation,
        float radius,
        float halfHeight,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector, Vector, float, float, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(CapsuleSingleSlot, 4, startLocation, endLocation, radius, halfHeight, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a ray against the world using object types and return overlapping hits and then first blocking hit</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> LineMulti(
        Vector startLocation,
        Vector endLocation,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, Vector, Vector, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(LineMultiSlot, 2, startLocation, endLocation, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a ray against the world and returns a table with the first blocking hit information</summary>
    public static IReadOnlyDictionary<string, object?> LineSingle(
        Vector startLocation,
        Vector endLocation,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector, Vector, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(LineSingleSlot, 2, startLocation, endLocation, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a sphere against the world using object types and return overlapping hits and then first blocking hit</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> SphereMulti(
        Vector startLocation,
        Vector endLocation,
        float radius,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, Vector, Vector, float, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(SphereMultiSlot, 3, startLocation, endLocation, radius, collisionChannel, traceMode, ignoredActors)!;

    /// <summary>Trace a sphere against the world and returns a table with the first blocking hit information</summary>
    public static IReadOnlyDictionary<string, object?> SphereSingle(
        Vector startLocation,
        Vector endLocation,
        float radius,
        CollisionChannel? collisionChannel = null,
        TraceMode? traceMode = null,
        IEnumerable<Entity>? ignoredActors = null) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector, Vector, float, CollisionChannel?, TraceMode?, IEnumerable<Entity>?>(SphereSingleSlot, 3, startLocation, endLocation, radius, collisionChannel, traceMode, ignoredActors)!;
}
