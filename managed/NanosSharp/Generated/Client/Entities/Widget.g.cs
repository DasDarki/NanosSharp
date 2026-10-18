#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>The Widget class supports spawning Unreal Widgets classes through scripting and manipulate them such as Blueprints!</summary>
[ScriptClass("Widget")]
public partial class Widget : Entity
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

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Widget", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Widget", "Destroy");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Widget", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Widget", "ValueChange");
    private static readonly MethodSlot AddChildSlot = new("AddChild");
    private static readonly MethodSlot AddToViewportSlot = new("AddToViewport");
    private static readonly MethodSlot BindBlueprintEventDispatcherSlot = new("BindBlueprintEventDispatcher");
    private static readonly MethodSlot BringToFrontSlot = new("BringToFront");
    private static readonly MethodSlot CallBlueprintEventSlot = new("CallBlueprintEvent");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot GetBlueprintPropertyValueSlot = new("GetBlueprintPropertyValue");
    private static readonly MethodSlot GetVisibilitySlot = new("GetVisibility");
    private static readonly MethodSlot SetBlueprintPropertyValueSlot = new("SetBlueprintPropertyValue");
    private static readonly MethodSlot SetContentForSlotSlot = new("SetContentForSlot");
    private static readonly MethodSlot SetFocusSlot = new("SetFocus");
    private static readonly MethodSlot SetSlotAlignmentSlot = new("SetSlotAlignment");
    private static readonly MethodSlot SetSlotCanvasLayoutSlot = new("SetSlotCanvasLayout");
    private static readonly MethodSlot SetSlotPaddingSlot = new("SetSlotPadding");
    private static readonly MethodSlot SetSlotSizeSlot = new("SetSlotSize");
    private static readonly MethodSlot SetVisibilitySlot = new("SetVisibility");
    private static readonly MethodSlot SpawnWidget3DSlot = new("SpawnWidget3D");
    private static readonly MethodSlot UnbindBlueprintEventDispatcherSlot = new("UnbindBlueprintEventDispatcher");

    /// <summary>Returns the current WebUI visibility</summary>
    public WidgetVisibility Visibility
    {
        get => GetVisibility();
        set => SetVisibility(value);
    }

    /// <summary>Spawns a new Widget.</summary>
    public Widget(string blueprintPath)
    {
        SpawnSelf("Widget", [blueprintPath]);
    }

    protected Widget()
    {
    }

    /// <summary>Adds a new child widget to this Widget container, if this is a Panel</summary>
    public void AddChild(Entity other) => Call(AddChildSlot, 1, other);

    /// <summary>Adds it to the game's viewport and fills the entire screen</summary>
    public void AddToViewport() => Call(AddToViewportSlot, 0);

    /// <summary>Assigns and Binds a Blueprint Event Dispatcher</summary>
    public ScriptFunction BindBlueprintEventDispatcher(string dispatcherName, Delegate callback) =>
        CallResult<ScriptFunction, string, ScriptCallback?>(BindBlueprintEventDispatcherSlot, 2, dispatcherName, ScriptCallback.From(callback))!;

    /// <summary>Puts this Widget in the front of all WebUIs and Widgets</summary>
    public void BringToFront() => Call(BringToFrontSlot, 0);

    /// <summary>Calls a Blueprint Event or Function</summary>
    public object?[] CallBlueprintEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic<object?[]>(CallBlueprintEventSlot, [eventName], arguments)!;

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Gets a Blueprint Property/Variable value</summary>
    public object? GetBlueprintPropertyValue(string propertyName) =>
        CallResult<object?, string>(GetBlueprintPropertyValueSlot, 1, propertyName);

    /// <summary>Returns the current WebUI visibility</summary>
    public WidgetVisibility GetVisibility() => CallResult<WidgetVisibility>(GetVisibilitySlot, 0)!;

    /// <summary>Sets a Blueprint Property/Variable value directly</summary>
    public void SetBlueprintPropertyValue(string propertyName, object? value) =>
        Call(SetBlueprintPropertyValueSlot, 2, propertyName, value);

    /// <summary>Sets the widget for a given slot by name, if this is a UserWidget</summary>
    public void SetContentForSlot(string slotName, Entity? widget = null) =>
        Call(SetContentForSlotSlot, 1, slotName, widget);

    /// <summary>Enables the focus on this Widget (i.e. can receive Keyboard input and will trigger input events)</summary>
    public void SetFocus() => Call(SetFocusSlot, 0);

    /// <summary>Sets the slot alignment for horizontal and vertical positioning</summary>
    public void SetSlotAlignment(
        WidgetHorizontalAlignment? horizontalAlignment = null,
        WidgetVerticalAlignment? verticalAlignment = null) =>
        Call(SetSlotAlignmentSlot, 0, horizontalAlignment, verticalAlignment);

    /// <summary>Sets the Layout as Canvas on Screen</summary>
    public void SetSlotCanvasLayout(
        Vector2D? location = null,
        Vector2D? size = null,
        Vector2D? anchorsMin = null,
        Vector2D? anchorsMax = null,
        Vector2D? alignment = null) =>
        Call(SetSlotCanvasLayoutSlot, 0, location, size, anchorsMin, anchorsMax, alignment);

    /// <summary>Sets the slot padding for horizontal and vertical spacing</summary>
    public void SetSlotPadding(Vector2D? paddingHorizontal = null, Vector2D? paddingVertical = null) =>
        Call(SetSlotPaddingSlot, 0, paddingHorizontal, paddingVertical);

    /// <summary>Sets the slot size and size rule</summary>
    public void SetSlotSize(float? size = null, WidgetSizeRule? slateSizeRule = null) =>
        Call(SetSlotSizeSlot, 0, size, slateSizeRule);

    /// <summary>Sets the visibility in screen</summary>
    public void SetVisibility(WidgetVisibility visibility) => Call(SetVisibilitySlot, 1, visibility);

    /// <summary>Spawns a 3D Widget actor rendering this Widget in the world</summary>
    public Widget3D SpawnWidget3D(
        Vector? location = null,
        Rotator? rotation = null,
        WidgetSpace? widgetSpace = null,
        bool? autoSize = null,
        Vector2D? size = null,
        float? autoRepaintRate = null,
        Vector2D? pivot = null) =>
        CallResult<Widget3D, Vector?, Rotator?, WidgetSpace?, bool?, Vector2D?, float?, Vector2D?>(SpawnWidget3DSlot, 0, location, rotation, widgetSpace, autoSize, size, autoRepaintRate, pivot)!;

    /// <summary>Unbinds a Blueprint Event Dispatcher</summary>
    public void UnbindBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null) =>
        Call(UnbindBlueprintEventDispatcherSlot, 1, dispatcherName, ScriptCallback.From(callback));

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

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

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Widget self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Widget self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Widget self, string? key, object? value);
}
