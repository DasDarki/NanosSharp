namespace NanosSharp.Scripting;

/// <summary>Raised when nanos world rejects a scripting call.</summary>
public sealed class ScriptException(string message) : Exception(message);
