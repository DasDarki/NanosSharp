#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Canvas is an entity which you can draw onto it.</summary>
[ScriptClass("Canvas")]
public partial class Canvas : Entity
{
    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is destroyed</summary>
    public static event DestroyHandler? OnDestroy
    {
        add => DestroyEvent.Add(value);
        remove => DestroyEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Called when the Canvas needs to be paintedYou can only call <c>:Draw...()</c> methods from inside this event</summary>
    public static event UpdateHandler? OnUpdate
    {
        add => UpdateEvent.Add(value);
        remove => UpdateEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Canvas", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Canvas", "Destroy");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Canvas", "Spawn");
    private static readonly ClassEvent<UpdateHandler> UpdateEvent = new("Canvas", "Update");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Canvas", "ValueChange");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot ClearSlot = new("Clear");
    private static readonly MethodSlot DrawBoxSlot = new("DrawBox");
    private static readonly MethodSlot DrawLineSlot = new("DrawLine");
    private static readonly MethodSlot DrawMaterialSlot = new("DrawMaterial");
    private static readonly MethodSlot DrawMaterialFromSceneCaptureSlot = new("DrawMaterialFromSceneCapture");
    private static readonly MethodSlot DrawMaterialFromWebUISlot = new("DrawMaterialFromWebUI");
    private static readonly MethodSlot DrawPolygonSlot = new("DrawPolygon");
    private static readonly MethodSlot DrawRectSlot = new("DrawRect");
    private static readonly MethodSlot DrawTextSlot = new("DrawText");
    private static readonly MethodSlot DrawTextureSlot = new("DrawTexture");
    private static readonly MethodSlot GetSizeSlot = new("GetSize");
    private static readonly MethodSlot RepaintSlot = new("Repaint");
    private static readonly MethodSlot ResizeSlot = new("Resize");
    private static readonly MethodSlot SetAutoRepaintRateSlot = new("SetAutoRepaintRate");
    private static readonly MethodSlot SetAutoResizeSlot = new("SetAutoResize");
    private static readonly MethodSlot SetScreenPositionSlot = new("SetScreenPosition");
    private static readonly MethodSlot SetVisibilitySlot = new("SetVisibility");

    /// <summary>Gets the Canvas Size</summary>
    public Vector2D Size => GetSize();

    /// <summary>Spawns a new Canvas.</summary>
    public Canvas(
        bool? isVisible = null,
        Color? clearColor = null,
        float? autoRepaintRate = null,
        bool? shouldClearBeforeUpdate = null,
        bool? autoResize = null,
        int? width = null,
        int? height = null,
        Vector2D? screenPosition = null)
    {
        SpawnSelf("Canvas", ScriptArguments.Trim([isVisible, clearColor, autoRepaintRate, shouldClearBeforeUpdate, autoResize, width, height, screenPosition], 0));
    }

    protected Canvas()
    {
    }

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Clear the Canvas with a specific Color</summary>
    public void Clear(Color clearColor) => Call(ClearSlot, 1, clearColor);

    /// <summary>Draws an unfilled box on the Canvas</summary>
    public void DrawBox(
        Vector2D screenPosition,
        Vector2D screenSize,
        float thickness,
        Color renderColor,
        BlendMode? blendMode = null) =>
        Call(DrawBoxSlot, 4, screenPosition, screenSize, thickness, renderColor, blendMode);

    /// <summary>Draws a line on the Canvas</summary>
    public void DrawLine(
        Vector2D screenPositionA,
        Vector2D screenPositionB,
        float thickness,
        Color renderColor,
        BlendMode? blendMode = null) =>
        Call(DrawLineSlot, 4, screenPositionA, screenPositionB, thickness, renderColor, blendMode);

    /// <summary>Draws a Material on the Canvas</summary>
    public void DrawMaterial(
        string materialPath,
        Vector2D screenPosition,
        Vector2D screenSize,
        Vector2D coordinatePosition,
        Vector2D? coordinateSize = null,
        float? rotation = null,
        Vector2D? pivotPoint = null,
        BlendMode? blendMode = null) =>
        Call(DrawMaterialSlot, 4, materialPath, screenPosition, screenSize, coordinatePosition, coordinateSize, rotation, pivotPoint, blendMode);

    /// <summary>Draws a SceneCapture on the Canvas</summary>
    public void DrawMaterialFromSceneCapture(
        Entity scenecaptureEntity,
        Vector2D screenPosition,
        Vector2D screenSize,
        Vector2D coordinatePosition,
        Vector2D? coordinateSize = null,
        float? rotation = null,
        Vector2D? pivotPoint = null,
        BlendMode? blendMode = null) =>
        Call(DrawMaterialFromSceneCaptureSlot, 4, scenecaptureEntity, screenPosition, screenSize, coordinatePosition, coordinateSize, rotation, pivotPoint, blendMode);

    /// <summary>Draws a WebUI on the Canvas</summary>
    public void DrawMaterialFromWebUI(
        Entity webuiEntity,
        Vector2D screenPosition,
        Vector2D screenSize,
        Vector2D coordinatePosition,
        Vector2D? coordinateSize = null,
        float? rotation = null,
        Vector2D? pivotPoint = null,
        BlendMode? blendMode = null) =>
        Call(DrawMaterialFromWebUISlot, 4, webuiEntity, screenPosition, screenSize, coordinatePosition, coordinateSize, rotation, pivotPoint, blendMode);

    /// <summary>Draws a N-Polygon on the Canvas</summary>
    public void DrawPolygon(
        string texturePath,
        Vector2D screenPosition,
        Vector2D? radius = null,
        int? numberOfSides = null,
        Color? renderColor = null,
        BlendMode? blendMode = null) =>
        Call(DrawPolygonSlot, 2, texturePath, screenPosition, radius, numberOfSides, renderColor, blendMode);

    /// <summary>Draws a filled Rect on the Canvas</summary>
    public void DrawRect(
        string texturePath,
        Vector2D screenPosition,
        Vector2D screenSize,
        Color? renderColor = null,
        BlendMode? blendMode = null) =>
        Call(DrawRectSlot, 3, texturePath, screenPosition, screenSize, renderColor, blendMode);

    /// <summary>Draws a Text on the Canvas</summary>
    public void DrawText(
        string text,
        Vector2D screenPosition,
        FontType? fontType = null,
        int? fontSize = null,
        Color? textColor = null,
        float? kerning = null,
        bool? centerX = null,
        bool? centerY = null,
        Color? shadowColor = null,
        Vector2D? shadowOffset = null,
        bool? outlined = null,
        Color? outlineColor = null) =>
        Invoke(DrawTextSlot, ScriptArguments.Trim([text, screenPosition, fontType, fontSize, textColor, kerning, centerX, centerY, shadowColor, shadowOffset, outlined, outlineColor], 2));

    /// <summary>Draws a Texture on the Canvas</summary>
    public void DrawTexture(
        string texturePath,
        Vector2D screenPosition,
        Vector2D screenSize,
        Vector2D coordinatePosition,
        Vector2D? coordinateSize = null,
        Color? renderColor = null,
        BlendMode? blendMode = null,
        float? rotation = null,
        Vector2D? pivotPoint = null) =>
        Invoke(DrawTextureSlot, ScriptArguments.Trim([texturePath, screenPosition, screenSize, coordinatePosition, coordinateSize, renderColor, blendMode, rotation, pivotPoint], 4));

    /// <summary>Gets the Canvas Size</summary>
    public Vector2D GetSize() => CallResult<Vector2D>(GetSizeSlot, 0)!;

    /// <summary>Forces the repaint</summary>
    public void Repaint() => Call(RepaintSlot, 0);

    /// <summary>Resizes the Canvas if not using auto_resize</summary>
    public void Resize(uint width, uint height) => Call(ResizeSlot, 2, width, height);

    /// <summary>Sets the repaint rate</summary>
    public void SetAutoRepaintRate(float autoRepaintRate) => Call(SetAutoRepaintRateSlot, 1, autoRepaintRate);

    /// <summary>Sets if the canvas should auto resize to screen size</summary>
    public void SetAutoResize(bool autoResize) => Call(SetAutoResizeSlot, 1, autoResize);

    /// <summary>Sets the Canvas Screen Position offset</summary>
    public void SetScreenPosition(Vector2D screenPosition) => Call(SetScreenPositionSlot, 1, screenPosition);

    /// <summary>Sets if it's visible on screen</summary>
    public void SetVisibility(bool visible) => Call(SetVisibilitySlot, 1, visible);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the Update event of this entity only.</summary>
    public ScriptSubscription SubscribeUpdate(UpdateHandler handler) => Subscribe("Update", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
    {
    }

    /// <summary>Called for the Update event of this entity when a subclass overrides it.</summary>
    [EventHook("Update")]
    protected virtual void HandleUpdate(int width, int height)
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Canvas self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Canvas self);

    /// <summary>Called when the Canvas needs to be paintedYou can only call <c>:Draw...()</c> methods from inside this event</summary>
    public delegate void UpdateHandler(Canvas self, int width, int height);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Canvas self, string? key, object? value);
}
