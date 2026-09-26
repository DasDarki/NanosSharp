namespace NanosSharp.Core;

/// <summary>Server console commands with string arguments.</summary>
public static class Commands
{
    /// <summary>Registers a console command, the handler receives the command arguments.</summary>
    public static void Register(string name, Action<string[]> handler, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(handler);
        Action<object?[]> callback = arguments =>
            handler(arguments.Select(argument => argument?.ToString() ?? string.Empty).ToArray());
        ScriptConsole.RegisterCommand(name, callback, description);
    }

    /// <summary>Runs a console command.</summary>
    public static void Run(string command) => ScriptConsole.RunCommand(command);
}
