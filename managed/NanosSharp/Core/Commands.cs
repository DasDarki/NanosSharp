using NanosSharp.Interop.Callbacks;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Server console commands.</summary>
public static class Commands
{
    private const string ClassName = "Console_I";

    /// <summary>Registers a console command, the handler receives the command arguments.</summary>
    public static void Register(string name, Action<string[]> handler, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(handler);
        Action<object?[]> callback = arguments =>
            handler(arguments.Select(argument => argument?.ToString() ?? string.Empty).ToArray());
        using var function = new ScriptFunction(FunctionRegistry.CreateNative(callback));
        ScriptApi.Invoke(ClassName, "RegisterCommand", name, function, description);
    }

    /// <summary>Runs a console command.</summary>
    public static void Run(string command) => ScriptApi.Invoke(ClassName, "RunCommand", command);
}
