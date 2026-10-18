using NanosSharp.Enums;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

public static partial class Events
{
    private static readonly object UnreliableValue = Reliability.Unreliable;

    /// <summary>Calls a custom event on the server.</summary>
    public static void CallRemote(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [eventName, null], arguments);

    /// <summary>Calls a custom event on the server without boxing the arguments.</summary>
    public static void CallRemote<T1>(string eventName, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            CallRemote(eventName, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, object?, T1>(CallRemoteSlot, 3, eventName, (object?)null, argument1);
    }

    /// <summary>Calls a custom event on the server without boxing the arguments.</summary>
    public static void CallRemote<T1, T2>(string eventName, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, object?, T1, T2>(CallRemoteSlot, 4, eventName, (object?)null, argument1, argument2);

    /// <summary>Calls a custom event on the server without boxing the arguments.</summary>
    public static void CallRemote<T1, T2, T3>(string eventName, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, object?, T1, T2, T3>(CallRemoteSlot, 5, eventName, (object?)null, argument1, argument2, argument3);

    /// <summary>Calls a custom event on the server without boxing the arguments.</summary>
    public static void CallRemote<T1, T2, T3, T4>(string eventName, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, object?, T1, T2, T3, T4>(CallRemoteSlot, 6, eventName, (object?)null, argument1, argument2, argument3, argument4);

    /// <summary>Calls a custom event on the server unreliably, the message may be lost or arrive out of order.</summary>
    public static void CallRemoteUnreliable(string eventName, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [eventName, UnreliableValue], arguments);

    /// <summary>Calls a custom event on the server unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1>(string eventName, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            CallRemoteUnreliable(eventName, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, object?, T1>(CallRemoteSlot, 3, eventName, UnreliableValue, argument1);
    }

    /// <summary>Calls a custom event on the server unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2>(string eventName, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, object?, T1, T2>(CallRemoteSlot, 4, eventName, UnreliableValue, argument1, argument2);

    /// <summary>Calls a custom event on the server unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2, T3>(string eventName, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, object?, T1, T2, T3>(CallRemoteSlot, 5, eventName, UnreliableValue, argument1, argument2, argument3);

    /// <summary>Calls a custom event on the server unreliably without boxing the arguments.</summary>
    public static void CallRemoteUnreliable<T1, T2, T3, T4>(string eventName, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, object?, T1, T2, T3, T4>(CallRemoteSlot, 6, eventName, UnreliableValue, argument1, argument2, argument3, argument4);
}
