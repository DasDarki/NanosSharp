namespace NanosSharp.Scripting;

/// <summary>A value only meaningful to scripts, it can be stored and passed back but not inspected.</summary>
public sealed class ScriptOpaque : ScriptHandle
{
    internal ScriptOpaque(nint handle) : base(handle)
    {
    }
}
