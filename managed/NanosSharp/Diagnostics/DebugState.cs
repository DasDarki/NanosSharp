namespace NanosSharp.Diagnostics;

internal static class DebugState
{
    public static bool IsEnabled { get; private set; }

    public static void Enable() => IsEnabled = true;
}
