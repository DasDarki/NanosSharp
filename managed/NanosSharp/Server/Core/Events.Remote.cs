using NanosSharp.Entities;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

public static partial class Events
{
    private static readonly StaticMethodSlot BroadcastRemoteSlot = new(ClassName, "BroadcastRemote");
    private static readonly object UnreliableValue = Reliability.Unreliable;

    /// <summary>Calls a custom event on one client.</summary>
    public static void CallRemote(string eventName, Player player, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [eventName, player, null], arguments);

    /// <summary>Calls a custom event on one client without boxing the arguments.</summary>
    public static void CallRemote<T1>(string eventName, Player player, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            CallRemote(eventName, player, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, Player, object?, T1>(CallRemoteSlot, 4, eventName, player, (object?)null, argument1);
    }

    /// <summary>Calls a custom event on one client without boxing the arguments.</summary>
    public static void CallRemote<T1, T2>(string eventName, Player player, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, Player, object?, T1, T2>(CallRemoteSlot, 5, eventName, player, (object?)null, argument1, argument2);

    /// <summary>Calls a custom event on one client without boxing the arguments.</summary>
    public static void CallRemote<T1, T2, T3>(string eventName, Player player, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, Player, object?, T1, T2, T3>(CallRemoteSlot, 6, eventName, player, (object?)null, argument1, argument2, argument3);

    /// <summary>Calls a custom event on one client without boxing the arguments.</summary>
    public static void CallRemote<T1, T2, T3, T4>(string eventName, Player player, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, Player, object?, T1, T2, T3, T4>(CallRemoteSlot, 7, eventName, player, (object?)null, argument1, argument2, argument3, argument4);

    /// <summary>Calls a custom event on one client unreliably, the message may be lost or arrive out of order.</summary>
    public static void CallRemoteUnreliable(string eventName, Player player, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [eventName, player, UnreliableValue], arguments);

    /// <summary>Calls a custom event on one client unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1>(string eventName, Player player, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            CallRemoteUnreliable(eventName, player, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, Player, object?, T1>(CallRemoteSlot, 4, eventName, player, UnreliableValue, argument1);
    }

    /// <summary>Calls a custom event on one client unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2>(string eventName, Player player, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, Player, object?, T1, T2>(CallRemoteSlot, 5, eventName, player, UnreliableValue, argument1, argument2);

    /// <summary>Calls a custom event on one client unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2, T3>(string eventName, Player player, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, Player, object?, T1, T2, T3>(CallRemoteSlot, 6, eventName, player, UnreliableValue, argument1, argument2, argument3);

    /// <summary>Calls a custom event on one client unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2, T3, T4>(string eventName, Player player, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, Player, object?, T1, T2, T3, T4>(CallRemoteSlot, 7, eventName, player, UnreliableValue, argument1, argument2, argument3, argument4);

    /// <summary>Calls a custom event on every client.</summary>
    public static void BroadcastRemote(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(BroadcastRemoteSlot, [eventName, null], arguments);

    /// <summary>Calls a custom event on every client without boxing the arguments.</summary>
    public static void BroadcastRemote<T1>(string eventName, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            BroadcastRemote(eventName, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, object?, T1>(BroadcastRemoteSlot, 3, eventName, (object?)null, argument1);
    }

    /// <summary>Calls a custom event on every client without boxing the arguments.</summary>
    public static void BroadcastRemote<T1, T2>(string eventName, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, object?, T1, T2>(BroadcastRemoteSlot, 4, eventName, (object?)null, argument1, argument2);

    /// <summary>Calls a custom event on every client without boxing the arguments.</summary>
    public static void BroadcastRemote<T1, T2, T3>(string eventName, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, object?, T1, T2, T3>(BroadcastRemoteSlot, 5, eventName, (object?)null, argument1, argument2, argument3);

    /// <summary>Calls a custom event on every client without boxing the arguments.</summary>
    public static void BroadcastRemote<T1, T2, T3, T4>(string eventName, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, object?, T1, T2, T3, T4>(BroadcastRemoteSlot, 6, eventName, (object?)null, argument1, argument2, argument3, argument4);

    /// <summary>Calls a custom event on every client unreliably, the message may be lost or arrive out of order.</summary>
    public static void BroadcastRemoteUnreliable(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(BroadcastRemoteSlot, [eventName, UnreliableValue], arguments);

    /// <summary>Calls a custom event on every client unreliably without boxing the arguments.</summary>
    public static void BroadcastRemoteUnreliable<T1>(string eventName, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            BroadcastRemoteUnreliable(eventName, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, object?, T1>(BroadcastRemoteSlot, 3, eventName, UnreliableValue, argument1);
    }

    /// <summary>Calls a custom event on every client unreliably without boxing the arguments.</summary>
    public static void BroadcastRemoteUnreliable<T1, T2>(string eventName, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, object?, T1, T2>(BroadcastRemoteSlot, 4, eventName, UnreliableValue, argument1, argument2);

    /// <summary>Calls a custom event on every client unreliably without boxing the arguments.</summary>
    public static void BroadcastRemoteUnreliable<T1, T2, T3>(string eventName, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, object?, T1, T2, T3>(BroadcastRemoteSlot, 5, eventName, UnreliableValue, argument1, argument2, argument3);

    /// <summary>Calls a custom event on every client unreliably without boxing the arguments.</summary>
    public static void BroadcastRemoteUnreliable<T1, T2, T3, T4>(string eventName, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, object?, T1, T2, T3, T4>(BroadcastRemoteSlot, 6, eventName, UnreliableValue, argument1, argument2, argument3, argument4);
}
