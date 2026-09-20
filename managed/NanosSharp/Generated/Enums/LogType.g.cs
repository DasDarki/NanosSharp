#nullable enable

namespace NanosSharp.Enums;

/// <summary>Types of log messages, which define their color and filter</summary>
public enum LogType
{
    Display = 0,
    Warning = 1,
    Error = 2,
    Debug = 3,
    Verbose = 4,
    /// <summary>Messages printed by scripts</summary>
    Scripting = 5,
    /// <summary>Warnings printed by scripts</summary>
    ScriptingWarn = 6,
    /// <summary>Errors raised by scripts</summary>
    ScriptingError = 7,
    /// <summary>Chat messages</summary>
    Chat = 8,
    /// <summary>Messages logged from WebUI JavaScript</summary>
    WebUI = 9,
    Success = 10,
    Fatal = 11,
}
