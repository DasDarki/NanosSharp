using NanosSharp.Scripting;

namespace NanosSharp.Core;

public static partial class Events
{
    /// <summary>Calls a local custom event in every package without boxing the arguments.</summary>
    public static void Call<T1>(string eventName, T1 argument1)
    {
        if (typeof(T1) == typeof(object?[]))
        {
            Call(eventName, (ReadOnlySpan<object?>)(object?[]?)(object?)argument1);
            return;
        }

        ScriptApi.Call<string, T1>(CallSlot, 2, eventName, argument1);
    }

    /// <summary>Calls a local custom event in every package without boxing the arguments.</summary>
    public static void Call<T1, T2>(string eventName, T1 argument1, T2 argument2) =>
        ScriptApi.Call<string, T1, T2>(CallSlot, 3, eventName, argument1, argument2);

    /// <summary>Calls a local custom event in every package without boxing the arguments.</summary>
    public static void Call<T1, T2, T3>(string eventName, T1 argument1, T2 argument2, T3 argument3) =>
        ScriptApi.Call<string, T1, T2, T3>(CallSlot, 4, eventName, argument1, argument2, argument3);

    /// <summary>Calls a local custom event in every package without boxing the arguments.</summary>
    public static void Call<T1, T2, T3, T4>(string eventName, T1 argument1, T2 argument2, T3 argument3, T4 argument4) =>
        ScriptApi.Call<string, T1, T2, T3, T4>(CallSlot, 5, eventName, argument1, argument2, argument3, argument4);
}
