using SystemDebugger = System.Diagnostics.Debugger;

namespace NanosSharp.Diagnostics;

/// <summary>Debugging helpers for packages, active only when debugging is enabled on this side.</summary>
public static class Debugger
{
    /// <summary>Whether debugging is enabled, which needs the server and this side to opt in.</summary>
    public static bool IsEnabled => DebugState.IsEnabled;

    /// <summary>Whether a debugger is currently attached to the process.</summary>
    public static bool IsAttached => SystemDebugger.IsAttached;

    /// <summary>Stops in a debugger at this line when debugging is enabled, launching one if needed, otherwise does nothing.</summary>
    public static void Break()
    {
        if (!DebugState.IsEnabled)
        {
            return;
        }

        if (!SystemDebugger.IsAttached)
        {
            SystemDebugger.Launch();
        }

        if (SystemDebugger.IsAttached)
        {
            SystemDebugger.Break();
        }
    }
}
