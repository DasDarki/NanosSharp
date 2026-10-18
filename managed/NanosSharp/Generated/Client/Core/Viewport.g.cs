#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Work with screen properties and effects.</summary>
public static partial class Viewport
{
    /// <summary>Called when the screen is resized</summary>
    public static event ResizeHandler? OnResize
    {
        add => ResizeEvent.Add(value);
        remove => ResizeEvent.Remove(value);
    }

    private static readonly ClassEvent<ResizeHandler> ResizeEvent = new("Viewport", "Resize");
    private static readonly StaticMethodSlot DeprojectScreenToWorldSlot = new("Viewport", "DeprojectScreenToWorld");
    private static readonly StaticMethodSlot GetMousePositionSlot = new("Viewport", "GetMousePosition");
    private static readonly StaticMethodSlot GetViewportScaleSlot = new("Viewport", "GetViewportScale");
    private static readonly StaticMethodSlot GetViewportSizeSlot = new("Viewport", "GetViewportSize");
    private static readonly StaticMethodSlot ProjectWorldToScreenSlot = new("Viewport", "ProjectWorldToScreen");
    private static readonly StaticMethodSlot SetBloodScreenEnabledSlot = new("Viewport", "SetBloodScreenEnabled");
    private static readonly StaticMethodSlot SetBloodScreenIntensitySlot = new("Viewport", "SetBloodScreenIntensity");
    private static readonly StaticMethodSlot SetBrandingBarSettingsSlot = new("Viewport", "SetBrandingBarSettings");
    private static readonly StaticMethodSlot SetCrosshairEnabledSlot = new("Viewport", "SetCrosshairEnabled");
    private static readonly StaticMethodSlot SetHardwareCursorSlot = new("Viewport", "SetHardwareCursor");
    private static readonly StaticMethodSlot SetInteractionToolTipEnabledSlot = new("Viewport", "SetInteractionToolTipEnabled");
    private static readonly StaticMethodSlot SetMousePositionSlot = new("Viewport", "SetMousePosition");

    /// <summary>Gets the current mouse screen location</summary>
    public static Vector2D MousePosition
    {
        get => GetMousePosition();
        set => SetMousePosition(value);
    }

    /// <summary>Gets the scale of viewport</summary>
    public static float ViewportScale => GetViewportScale();

    /// <summary>Gets the size of viewport</summary>
    public static Vector2D ViewportSize => GetViewportSize();

    /// <summary>Transforms a 2D screen coordinates into 3D world-space location</summary>
    public static IReadOnlyDictionary<string, object?> DeprojectScreenToWorld(Vector2D screenPosition) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, Vector2D>(DeprojectScreenToWorldSlot, 1, screenPosition)!;

    /// <summary>Gets the current mouse screen location</summary>
    public static Vector2D GetMousePosition() => ScriptApi.CallResult<Vector2D>(GetMousePositionSlot, 0)!;

    /// <summary>Gets the scale of viewport</summary>
    public static float GetViewportScale() => ScriptApi.CallResult<float>(GetViewportScaleSlot, 0)!;

    /// <summary>Gets the size of viewport</summary>
    public static Vector2D GetViewportSize() => ScriptApi.CallResult<Vector2D>(GetViewportSizeSlot, 0)!;

    /// <summary>Transforms a 3D world-space vector into 2D screen coordinates</summary>
    public static Vector2D ProjectWorldToScreen(Vector worldPosition) =>
        ScriptApi.CallResult<Vector2D, Vector>(ProjectWorldToScreenSlot, 1, worldPosition)!;

    /// <summary>Enables/Disables the Blood Screen effect</summary>
    public static void SetBloodScreenEnabled(bool isEnabled) => ScriptApi.Call(SetBloodScreenEnabledSlot, 1, isEnabled);

    /// <summary>Manually sets the Blood Screen intensity</summary>
    public static void SetBloodScreenIntensity(float intensity) =>
        ScriptApi.Call(SetBloodScreenIntensitySlot, 1, intensity);

    /// <summary>Sets the Branding Bar settings (the bar with 'nanos world - closed alpha ver...')</summary>
    public static void SetBrandingBarSettings(
        WidgetHorizontalAlignment horizontalAlignment,
        WidgetVerticalAlignment verticalAlignment,
        bool? isLean = null) =>
        ScriptApi.Call(SetBrandingBarSettingsSlot, 2, horizontalAlignment, verticalAlignment, isLean);

    /// <summary>Enables/Disables the Crosshair</summary>
    public static void SetCrosshairEnabled(bool isEnabled) => ScriptApi.Call(SetCrosshairEnabledSlot, 1, isEnabled);

    /// <summary>Loads and sets a hardware cursor in the game from a PNG image</summary>
    public static bool SetHardwareCursor(CursorType cursorShape, string cursorPath, Vector2D? hotspot = null) =>
        ScriptApi.CallResult<bool, CursorType, string, Vector2D?>(SetHardwareCursorSlot, 2, cursorShape, cursorPath, hotspot)!;

    /// <summary>Enables/Disables the Interaction ToolTip</summary>
    public static void SetInteractionToolTipEnabled(bool isEnabled) =>
        ScriptApi.Call(SetInteractionToolTipEnabledSlot, 1, isEnabled);

    /// <summary>Sets the mouse position</summary>
    public static void SetMousePosition(Vector2D newPosition) => ScriptApi.Call(SetMousePositionSlot, 1, newPosition);

    /// <summary>Called when the screen is resized</summary>
    public delegate void ResizeHandler(Vector2D newSize);
}
