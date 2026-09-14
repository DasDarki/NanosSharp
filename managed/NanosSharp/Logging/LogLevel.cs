namespace NanosSharp.Logging;

/// <summary>Severity of a server log entry, matching the nanos world log types.</summary>
public enum LogLevel
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Debug = 3,
    Verbose = 4,
    Script = 5,
    ScriptWarning = 6,
    ScriptError = 7,
    ScriptDebug = 8,
    Chat = 9,
    WebUI = 10,
    Success = 11,
    Fatal = 12,
}
