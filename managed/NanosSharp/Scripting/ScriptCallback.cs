namespace NanosSharp.Scripting;

/// <summary>Marks a delegate that is passed to a native function expecting a callback.</summary>
public sealed class ScriptCallback
{
    internal Delegate Handler { get; }

    private ScriptCallback(Delegate handler)
    {
        Handler = handler;
    }

    /// <summary>Wraps a delegate, returns null for a null delegate.</summary>
    public static ScriptCallback? From(Delegate? handler) => handler is null ? null : new ScriptCallback(handler);
}
