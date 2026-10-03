using NanosSharp.Entities;
using NanosSharp.Interop.Reflection;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Custom events shared between packages and clients.</summary>
public static class Events
{
    private const string ClassName = "Events";
    private static readonly StaticMethodSlot CallSlot = new(ClassName, "Call");
    private static readonly StaticMethodSlot CallRemoteSlot = new(ClassName, "CallRemote");
    private static readonly StaticMethodSlot BroadcastRemoteSlot = new(ClassName, "BroadcastRemote");

    /// <summary>Subscribes to a local custom event, the handler parameters are converted automatically.</summary>
    public static ScriptSubscription Subscribe(string eventName, Delegate handler) =>
        ScriptApi.Subscribe(ClassName, eventName, handler);

    /// <summary>Subscribes to a custom event sent by clients, the first handler parameter is the sending player.</summary>
    public static ScriptSubscription SubscribeRemote(string eventName, Delegate handler) =>
        ScriptSubscription.Create(handler,
            function => ScriptApi.Invoke(ClassName, "SubscribeRemote", eventName, function),
            function => ScriptApi.Invoke(ClassName, "UnsubscribeRemote", eventName, function));

    /// <summary>Calls a local custom event in every package.</summary>
    public static void Call(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallSlot, [eventName], arguments);

    /// <summary>Calls a custom event on one client.</summary>
    public static void CallRemote(string eventName, Player player, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [eventName, player, null], arguments);

    /// <summary>Calls a custom event on every client.</summary>
    public static void BroadcastRemote(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(BroadcastRemoteSlot, [eventName, null], arguments);
}
