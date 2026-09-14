using NanosSharp.Interop;
using NanosSharp.Packages;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Logging;

/// <summary>Writes to the nanos world server console, prefixed with the calling package.</summary>
public static class Log
{
    private const string ConsoleClass = "Console_I";

    /// <summary>Writes an informational entry.</summary>
    public static void Info(string message) => Write("Log", LogLevel.Script, message);

    /// <summary>Writes a warning entry.</summary>
    public static void Warning(string message) => Write("Warn", LogLevel.ScriptWarning, message);

    /// <summary>Writes an error entry.</summary>
    public static void Error(string message) => Write("Error", LogLevel.ScriptError, message);

    /// <summary>Writes a debug entry, only visible with debug log level.</summary>
    public static void Debug(string message) => Write("Debug", LogLevel.ScriptDebug, message);

    /// <summary>Writes a success entry.</summary>
    public static void Success(string message) => WriteNative(LogLevel.Success, message);

    /// <summary>Writes an exception including its stack trace.</summary>
    public static void Exception(Exception exception, string? context = null) =>
        Error(context is null ? exception.ToString() : $"{context}: {exception}");

    private static void Write(string function, LogLevel fallbackLevel, string message)
    {
        if (TryWriteConsole(function, message))
        {
            return;
        }

        WriteNative(fallbackLevel, message);
    }

    private static void WriteNative(LogLevel level, string message)
    {
        var package = PackageContext.Current;
        NativeRuntime.Log(level, package is null ? message : $"[{package.Name}] {message}");
    }

    private static bool TryWriteConsole(string function, string message)
    {
        if (!MainThread.IsCurrent || !NativeRuntime.IsEngineAttached || !PackageContext.CanAccessNative)
        {
            return false;
        }

        try
        {
            ScriptApi.Invoke(ConsoleClass, function, message);
            return true;
        }
        catch (ScriptException)
        {
            return false;
        }
    }
}
