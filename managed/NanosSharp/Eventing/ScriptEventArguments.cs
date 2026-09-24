using NanosSharp.Interop.Values;

namespace NanosSharp.Eventing;

internal static class ScriptEventArguments
{
    public static T? Get<T>(object?[] arguments, int index) =>
        index < arguments.Length ? ValueConverter.Convert<T>(arguments[index]) : default;
}
