#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Draws Debug shapes in the world.</summary>
public static partial class Debug
{
    private static readonly StaticMethodSlot DrawBoxSlot = new("Debug", "DrawBox");
    private static readonly StaticMethodSlot DrawCapsuleSlot = new("Debug", "DrawCapsule");
    private static readonly StaticMethodSlot DrawCoordinateSystemSlot = new("Debug", "DrawCoordinateSystem");
    private static readonly StaticMethodSlot DrawCrosshairsSlot = new("Debug", "DrawCrosshairs");
    private static readonly StaticMethodSlot DrawCylinderSlot = new("Debug", "DrawCylinder");
    private static readonly StaticMethodSlot DrawDirectionalArrowSlot = new("Debug", "DrawDirectionalArrow");
    private static readonly StaticMethodSlot DrawLineSlot = new("Debug", "DrawLine");
    private static readonly StaticMethodSlot DrawPointSlot = new("Debug", "DrawPoint");
    private static readonly StaticMethodSlot DrawSphereSlot = new("Debug", "DrawSphere");
    private static readonly StaticMethodSlot DrawStringSlot = new("Debug", "DrawString");

    /// <summary>Draws a Debug box in the world</summary>
    public static void DrawBox(
        Vector location,
        Vector extent,
        Rotator rotation,
        Color color,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawBoxSlot, 4, location, extent, rotation, color, lifeTime, thickness);

    /// <summary>Draws a Debug capsule in the World</summary>
    public static void DrawCapsule(
        Vector center,
        float halfHeight,
        float radius,
        Rotator rotation,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawCapsuleSlot, 4, center, halfHeight, radius, rotation, color, lifeTime, thickness);

    /// <summary>Draws a Debug coordinate system in the world</summary>
    public static void DrawCoordinateSystem(
        Vector location,
        Rotator rotation,
        float? size = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawCoordinateSystemSlot, 2, location, rotation, size, lifeTime, thickness);

    /// <summary>Draws a Debug crosshair in the world</summary>
    public static void DrawCrosshairs(
        Vector location,
        Rotator rotation,
        float? size = null,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawCrosshairsSlot, 2, location, rotation, size, color, lifeTime, thickness);

    /// <summary>Draws a Debug cylinder in the world</summary>
    public static void DrawCylinder(
        Vector startLocation,
        Vector endLocation,
        float radius,
        int segments,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawCylinderSlot, 4, startLocation, endLocation, radius, segments, color, lifeTime, thickness);

    /// <summary>Draws a Debug directional arrow in the world</summary>
    public static void DrawDirectionalArrow(
        Vector startLocation,
        Vector endLocation,
        float? arrowSize = null,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawDirectionalArrowSlot, 2, startLocation, endLocation, arrowSize, color, lifeTime, thickness);

    /// <summary>Draws a Debug Line in the World</summary>
    public static void DrawLine(
        Vector startPosition,
        Vector endPosition,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawLineSlot, 2, startPosition, endPosition, color, lifeTime, thickness);

    /// <summary>Draws a Debug Point in the World</summary>
    public static void DrawPoint(
        Vector startPosition,
        Color? color = null,
        float? lifeTime = null,
        float? size = null) =>
        ScriptApi.Call(DrawPointSlot, 1, startPosition, color, lifeTime, size);

    /// <summary>Draws a Debug sphere in the World</summary>
    public static void DrawSphere(
        Vector startLocation,
        float radius,
        int segments,
        Color? color = null,
        float? lifeTime = null,
        float? thickness = null) =>
        ScriptApi.Call(DrawSphereSlot, 3, startLocation, radius, segments, color, lifeTime, thickness);

    /// <summary>Draws a Debug string in the world</summary>
    public static void DrawString(
        Vector location,
        string text,
        Color? color = null,
        float? lifeTime = null,
        bool? drawShadow = null,
        float? fontScale = null) =>
        ScriptApi.Call(DrawStringSlot, 2, location, text, color, lifeTime, drawShadow, fontScale);
}
