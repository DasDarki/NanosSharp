#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Class for spawning a dynamic Web Browser.</summary>
[ScriptClass("WebUI")]
public partial class WebUI : Entity
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

    /// <summary>Triggered when this page fails to load</summary>
    public static event FailHandler? OnFail
    {
        add => FailEvent.Add(value);
        remove => FailEvent.Remove(value);
    }

    /// <summary>Triggered when this page is fully loaded</summary>
    public static event ReadyHandler? OnReady
    {
        add => ReadyEvent.Add(value);
        remove => ReadyEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("WebUI", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("WebUI", "Destroy");
    private static readonly ClassEvent<FailHandler> FailEvent = new("WebUI", "Fail");
    private static readonly ClassEvent<ReadyHandler> ReadyEvent = new("WebUI", "Ready");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("WebUI", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("WebUI", "ValueChange");
    private static readonly MethodSlot BringToFrontSlot = new("BringToFront");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot CloseDevToolsSlot = new("CloseDevTools");
    private static readonly MethodSlot ExecuteJavaScriptSlot = new("ExecuteJavaScript");
    private static readonly MethodSlot GetNameSlot = new("GetName");
    private static readonly MethodSlot GetSizeSlot = new("GetSize");
    private static readonly MethodSlot GetVisibilitySlot = new("GetVisibility");
    private static readonly MethodSlot HasNodeFocusSlot = new("HasNodeFocus");
    private static readonly MethodSlot IsFrozenSlot = new("IsFrozen");
    private static readonly MethodSlot IsReadySlot = new("IsReady");
    private static readonly MethodSlot IsVisibleSlot = new("IsVisible");
    private static readonly MethodSlot LoadHTMLSlot = new("LoadHTML");
    private static readonly MethodSlot LoadURLSlot = new("LoadURL");
    private static readonly MethodSlot OpenDevToolsSlot = new("OpenDevTools");
    private static readonly MethodSlot RemoveFocusSlot = new("RemoveFocus");
    private static readonly MethodSlot ResizeSlot = new("Resize");
    private static readonly MethodSlot SendKeyEventSlot = new("SendKeyEvent");
    private static readonly MethodSlot SendMouseClickEventSlot = new("SendMouseClickEvent");
    private static readonly MethodSlot SendMouseMoveEventSlot = new("SendMouseMoveEvent");
    private static readonly MethodSlot SendMouseWheelEventSlot = new("SendMouseWheelEvent");
    private static readonly MethodSlot SetAutoResizeSlot = new("SetAutoResize");
    private static readonly MethodSlot SetFocusSlot = new("SetFocus");
    private static readonly MethodSlot SetFreezeSlot = new("SetFreeze");
    private static readonly MethodSlot SetLayoutSlot = new("SetLayout");
    private static readonly MethodSlot SetVisibilitySlot = new("SetVisibility");
    private static readonly MethodSlot SpawnSoundSlot = new("SpawnSound");

    /// <summary>Gets this WebUI name</summary>
    public string Name => GetName();

    /// <summary>Gets the current size of this WebUI</summary>
    public Vector2D Size => GetSize();

    /// <summary>Returns the current WebUI visibility</summary>
    public WidgetVisibility Visibility
    {
        get => GetVisibility();
        set => SetVisibility(value);
    }

    /// <summary>Returns if this WebUI is currently frozen</summary>
    public bool Frozen => IsFrozen();

    /// <summary>Returns if this WebUI is ready</summary>
    public bool Ready => IsReady();

    /// <summary>Returns true if this Actor is visible</summary>
    public bool Visible => IsVisible();

    /// <summary>Spawns a new WebUI.</summary>
    public WebUI(
        string name,
        string path,
        WidgetVisibility? visibility = null,
        bool? isTransparent = null,
        bool? autoResize = null,
        int? width = null,
        int? height = null)
    {
        SpawnSelf("WebUI", ScriptArguments.Trim([name, path, visibility, isTransparent, autoResize, width, height], 2));
    }

    protected WebUI()
    {
    }

    /// <summary>Puts this WebUI in the front of all WebUIs and Widgets</summary>
    public void BringToFront() => Call(BringToFrontSlot, 0);

    /// <summary>Calls an Event on the Browser's JavaScript</summary>
    public void CallEvent(string eventName, params object?[] args) => InvokeVariadic(CallEventSlot, [eventName], args);

    /// <summary>Closes the Developer Tools for this WebUI, it only works if the Client.SetDebugEnabled() not disabled</summary>
    public void CloseDevTools() => Call(CloseDevToolsSlot, 0);

    /// <summary>Executes a JavaScript code in the Browser</summary>
    public void ExecuteJavaScript(string javascriptCode) => Call(ExecuteJavaScriptSlot, 1, javascriptCode);

    /// <summary>Gets this WebUI name</summary>
    public string GetName() => CallResult<string>(GetNameSlot, 0)!;

    /// <summary>Gets the current size of this WebUI</summary>
    public Vector2D GetSize() => CallResult<Vector2D>(GetSizeSlot, 0)!;

    /// <summary>Returns the current WebUI visibility</summary>
    public WidgetVisibility GetVisibility() => CallResult<WidgetVisibility>(GetVisibilitySlot, 0)!;

    /// <summary>Returns if this WebUI has any DOM Node focused</summary>
    public bool HasNodeFocus() => CallResult<bool>(HasNodeFocusSlot, 0)!;

    /// <summary>Returns if this WebUI is currently frozen</summary>
    public bool IsFrozen() => CallResult<bool>(IsFrozenSlot, 0)!;

    /// <summary>Returns if this WebUI is ready</summary>
    public bool IsReady() => CallResult<bool>(IsReadySlot, 0)!;

    /// <summary>Returns true if this Actor is visible</summary>
    public bool IsVisible() => CallResult<bool>(IsVisibleSlot, 0)!;

    /// <summary>Loads a pure HTML in this Browser</summary>
    public void LoadHTML(string html) => Call(LoadHTMLSlot, 1, html);

    /// <summary>Loads a new File/URL in this Browser</summary>
    public void LoadURL(string url) => Call(LoadURLSlot, 1, url);

    /// <summary>Opens the Developer Tools for this WebUI, it only works if the Client.SetDebugEnabled() was not disabled</summary>
    public void OpenDevTools() => Call(OpenDevToolsSlot, 0);

    /// <summary>Removes the focus from this WebUI (and sets it back to game viewport)</summary>
    public void RemoveFocus() => Call(RemoveFocusSlot, 0);

    /// <summary>Resizes the Canvas if not using auto_resize</summary>
    public void Resize(uint width, uint height) => Call(ResizeSlot, 2, width, height);

    /// <summary>Sends a Key Event into the WebUI programmatically</summary>
    public void SendKeyEvent(WebUIKeyType keyType, int keyCode, WebUIModifier? modifiers = null) =>
        Call(SendKeyEventSlot, 2, keyType, keyCode, modifiers);

    /// <summary>Sends a Mouse Click into the WebUI programmatically</summary>
    public void SendMouseClickEvent(
        uint mouseX,
        uint mouseY,
        WebUIMouseType mouseType,
        bool isMouseUp,
        WebUIModifier? modifiers = null,
        int? clickCount = null) =>
        Call(SendMouseClickEventSlot, 4, mouseX, mouseY, mouseType, isMouseUp, modifiers, clickCount);

    /// <summary>Sends a Mouse Move Event into the WebUI programmatically</summary>
    public void SendMouseMoveEvent(
        uint mouseX,
        uint mouseY,
        WebUIModifier? modifiers = null,
        bool? mouseLeave = null) =>
        Call(SendMouseMoveEventSlot, 2, mouseX, mouseY, modifiers, mouseLeave);

    /// <summary>Sends a Mouse Event into the WebUI programmatically</summary>
    public void SendMouseWheelEvent(uint mouseX, uint mouseY, float deltaX, float deltaY) =>
        Call(SendMouseWheelEventSlot, 4, mouseX, mouseY, deltaX, deltaY);

    /// <summary>Sets if the canvas should auto resize to screen size</summary>
    public void SetAutoResize(bool autoResize) => Call(SetAutoResizeSlot, 1, autoResize);

    /// <summary>Enables the focus on this browser (i.e. can receive Keyboard input and will trigger input events)</summary>
    public void SetFocus() => Call(SetFocusSlot, 0);

    /// <summary>Freezes the WebUI rendering to the surface (JavaScript keeps running)</summary>
    public void SetFreeze(bool freeze) => Call(SetFreezeSlot, 1, freeze);

    /// <summary>Sets the Layout as Canvas on Screen</summary>
    public void SetLayout(
        Vector2D? location = null,
        Vector2D? size = null,
        Vector2D? anchorsMin = null,
        Vector2D? anchorsMax = null,
        Vector2D? alignment = null) =>
        Call(SetLayoutSlot, 0, location, size, anchorsMin, anchorsMax, alignment);

    /// <summary>Sets the visibility in screen</summary>
    public void SetVisibility(WidgetVisibility visibility) => Call(SetVisibilitySlot, 1, visibility);

    /// <summary>Spawns a Sound entity that plays this WebUI audio</summary>
    public Sound SpawnSound(
        Vector? location = null,
        bool? is2d = null,
        float? volume = null,
        float? innerRadius = null,
        float? falloffDistance = null,
        AttenuationFunction? attenuationFunction = null) =>
        CallResult<Sound, Vector?, bool?, float?, float?, float?, AttenuationFunction?>(SpawnSoundSlot, 0, location, is2d, volume, innerRadius, falloffDistance, attenuationFunction)!;

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the Fail event of this entity only.</summary>
    public ScriptSubscription SubscribeFail(FailHandler handler) => Subscribe("Fail", handler);

    /// <summary>Subscribes to the Ready event of this entity only.</summary>
    public ScriptSubscription SubscribeReady(ReadyHandler handler) => Subscribe("Ready", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the Fail event of this entity when a subclass overrides it.</summary>
    [EventHook("Fail")]
    protected virtual void HandleFail(int errorCode, string? message)
    {
    }

    /// <summary>Called for the Ready event of this entity when a subclass overrides it.</summary>
    [EventHook("Ready")]
    protected virtual void HandleReady()
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
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
    public delegate void DestroyHandler(WebUI self);

    /// <summary>Triggered when this page fails to load</summary>
    public delegate void FailHandler(WebUI self, int errorCode, string? message);

    /// <summary>Triggered when this page is fully loaded</summary>
    public delegate void ReadyHandler(WebUI self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(WebUI self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(WebUI self, string? key, object? value);
}
