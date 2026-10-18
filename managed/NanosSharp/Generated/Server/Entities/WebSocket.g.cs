#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class WebSocket
{
    /// <summary>Triggered when the connection was closed</summary>
    public static event CloseHandler? OnClose
    {
        add => CloseEvent.Add(value);
        remove => CloseEvent.Remove(value);
    }

    /// <summary>Triggered when a text message was received</summary>
    public static event ReceiveMessageHandler? OnReceiveMessage
    {
        add => ReceiveMessageEvent.Add(value);
        remove => ReceiveMessageEvent.Remove(value);
    }

    private static readonly ClassEvent<CloseHandler> CloseEvent = new("WebSocket", "Close");
    private static readonly ClassEvent<ReceiveMessageHandler> ReceiveMessageEvent = new("WebSocket", "ReceiveMessage");

    /// <summary>Subscribes to the Close event of this entity only.</summary>
    public ScriptSubscription SubscribeClose(CloseHandler handler) => Subscribe("Close", handler);

    /// <summary>Subscribes to the ReceiveMessage event of this entity only.</summary>
    public ScriptSubscription SubscribeReceiveMessage(ReceiveMessageHandler handler) => Subscribe("ReceiveMessage", handler);

    /// <summary>Called for the Close event of this entity when a subclass overrides it.</summary>
    [EventHook("Close")]
    protected virtual void HandleClose()
    {
    }

    /// <summary>Called for the ReceiveMessage event of this entity when a subclass overrides it.</summary>
    [EventHook("ReceiveMessage")]
    protected virtual void HandleReceiveMessage(string? message)
    {
    }

    /// <summary>Triggered when the connection was closed</summary>
    public delegate void CloseHandler(WebSocket self);

    /// <summary>Triggered when a text message was received</summary>
    public delegate void ReceiveMessageHandler(WebSocket self, string? message);
}
