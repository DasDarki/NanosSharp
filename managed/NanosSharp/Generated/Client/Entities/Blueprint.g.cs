#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Blueprint
{
    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Blueprint", "NetworkAuthorityChange");
    private static readonly MethodSlot BindBlueprintEventDispatcherSlot = new("BindBlueprintEventDispatcher");
    private static readonly MethodSlot GetBlueprintPropertyValueSlot = new("GetBlueprintPropertyValue");
    private static readonly MethodSlot SetBlueprintPropertyValueSlot = new("SetBlueprintPropertyValue");
    private static readonly MethodSlot SetMaterialFromCanvasSlot = new("SetMaterialFromCanvas");
    private static readonly MethodSlot SetMaterialFromSceneCaptureSlot = new("SetMaterialFromSceneCapture");
    private static readonly MethodSlot SetMaterialFromWebUISlot = new("SetMaterialFromWebUI");
    private static readonly MethodSlot UnbindBlueprintEventDispatcherSlot = new("UnbindBlueprintEventDispatcher");

    /// <summary>Assigns and Binds a Blueprint Event Dispatcher</summary>
    public ScriptFunction BindBlueprintEventDispatcher(string dispatcherName, Delegate callback) =>
        CallResult<ScriptFunction, string, ScriptCallback?>(BindBlueprintEventDispatcherSlot, 2, dispatcherName, ScriptCallback.From(callback))!;

    /// <summary>Gets a Blueprint Property/Variable value</summary>
    public object? GetBlueprintPropertyValue(string propertyName) =>
        CallResult<object?, string>(GetBlueprintPropertyValueSlot, 1, propertyName);

    /// <summary>Sets a Blueprint Property/Variable value directly</summary>
    public void SetBlueprintPropertyValue(string propertyName, object? value) =>
        Call(SetBlueprintPropertyValueSlot, 2, propertyName, value);

    /// <summary>Sets the material at the specified index of this Actor to a Canvas object</summary>
    public void SetMaterialFromCanvas(Entity canvas, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromCanvasSlot, 1, canvas, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a SceneCapture object</summary>
    public void SetMaterialFromSceneCapture(Entity sceneCapture, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromSceneCaptureSlot, 1, sceneCapture, index, argument3);

    /// <summary>Sets the material at the specified index of this Actor to a WebUI object</summary>
    public void SetMaterialFromWebUI(Entity webui, int? index = null, string? argument3 = null) =>
        Call(SetMaterialFromWebUISlot, 1, webui, index, argument3);

    /// <summary>Unbinds a Blueprint Event Dispatcher</summary>
    public void UnbindBlueprintEventDispatcher(string dispatcherName, Delegate? callback = null) =>
        Call(UnbindBlueprintEventDispatcherSlot, 1, dispatcherName, ScriptCallback.From(callback));

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Blueprint self, bool isNetworkAuthority);
}
