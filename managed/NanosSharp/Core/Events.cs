using NanosSharp.Entities;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Custom events shared between packages and clients.</summary>
public static class Events
{
    private const string ClassName = "Events";

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
        ScriptApi.Invoke(ClassName, "Call", [eventName, .. arguments]);

    /// <summary>Calls a custom event on one client.</summary>
    public static void CallRemote(string eventName, Player player, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.Invoke(ClassName, "CallRemote", [eventName, player, null, .. arguments]);

    /// <summary>Calls a custom event on every client.</summary>
    public static void BroadcastRemote(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.Invoke(ClassName, "BroadcastRemote", [eventName, null, .. arguments]);
}
