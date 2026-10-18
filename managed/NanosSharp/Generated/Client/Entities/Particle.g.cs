#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Particle
{
    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Particle", "NetworkAuthorityChange");
    private static readonly MethodSlot SetParameterMaterialFromCanvasSlot = new("SetParameterMaterialFromCanvas");
    private static readonly MethodSlot SetParameterMaterialFromWebUISlot = new("SetParameterMaterialFromWebUI");

    /// <summary>Sets a Material from a Canvas parameter in this Particle System</summary>
    public void SetParameterMaterialFromCanvas(string parameter, Entity value) =>
        Call(SetParameterMaterialFromCanvasSlot, 2, parameter, value);

    /// <summary>Sets a Material from a Canvas parameter in this Particle System</summary>
    public void SetParameterMaterialFromWebUI(string parameter, Entity value) =>
        Call(SetParameterMaterialFromWebUISlot, 2, parameter, value);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Particle self, bool isNetworkAuthority);
}
