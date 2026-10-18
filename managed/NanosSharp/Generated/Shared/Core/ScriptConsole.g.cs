#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Exposes access to registering Console Commands and Logging messages.</summary>
public static partial class ScriptConsole
{
    /// <summary>Called when a log is received</summary>
    public static event LogEntryHandler? OnLogEntry
    {
        add => LogEntryEvent.Add(value);
        remove => LogEntryEvent.Remove(value);
    }

    /// <summary>Called when a console command is submitted</summary>
    public static event PlayerSubmitHandler? OnPlayerSubmit
    {
        add => PlayerSubmitEvent.Add(value);
        remove => PlayerSubmitEvent.Remove(value);
    }

    private static readonly ClassEvent<LogEntryHandler> LogEntryEvent = new("Console_I", "LogEntry");
    private static readonly ClassEvent<PlayerSubmitHandler> PlayerSubmitEvent = new("Console_I", "PlayerSubmit");
    private static readonly StaticMethodSlot DebugSlot = new("Console_I", "Debug");
    private static readonly StaticMethodSlot ErrorSlot = new("Console_I", "Error");
    private static readonly StaticMethodSlot LogSlot = new("Console_I", "Log");
    private static readonly StaticMethodSlot RegisterCommandSlot = new("Console_I", "RegisterCommand");
    private static readonly StaticMethodSlot RunCommandSlot = new("Console_I", "RunCommand");
    private static readonly StaticMethodSlot WarnSlot = new("Console_I", "Warn");

    /// <summary>Logs a debug message in the console, with formatted arguments. Outputted only when the current log level is Debug or Verbose</summary>
    public static void Debug(string message) => ScriptApi.Call(DebugSlot, 1, message);

    /// <summary>Logs a red error in the console with stack information</summary>
    public static void Error(string message) => ScriptApi.Call(ErrorSlot, 1, message);

    /// <summary>Logs and formats a message in the console</summary>
    public static void Log(string message) => ScriptApi.Call(LogSlot, 1, message);

    /// <summary>Registers a new Console Command</summary>
    public static void RegisterCommand(
        string command,
        Delegate callback,
        string? description = null,
        IEnumerable<string>? parameters = null) =>
        ScriptApi.Call(RegisterCommandSlot, 2, command, ScriptCallback.From(callback), description, parameters);

    /// <summary>Runs a Console Command programmatically (only scripting-registered commands can be triggered)</summary>
    public static void RunCommand(string command) => ScriptApi.Call(RunCommandSlot, 1, command);

    /// <summary>Logs an orange warning in the console with stack information</summary>
    public static void Warn(string message) => ScriptApi.Call(WarnSlot, 1, message);

    /// <summary>Called when a log is received</summary>
    public delegate void LogEntryHandler(string? text, LogType type);

    /// <summary>Called when a console command is submitted</summary>
    public delegate void PlayerSubmitHandler(string? text);
}
