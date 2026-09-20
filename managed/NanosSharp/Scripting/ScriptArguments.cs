namespace NanosSharp.Scripting;

/// <summary>Helpers for passing arguments to scripting functions.</summary>
public static class ScriptArguments
{
    /// <summary>Removes trailing unset optional arguments so nanos world applies its own defaults.</summary>
    public static ReadOnlySpan<object?> Trim(ReadOnlySpan<object?> arguments, int requiredCount)
    {
        var length = arguments.Length;
        while (length > requiredCount && arguments[length - 1] is null)
        {
            length--;
        }

        return arguments[..length];
    }
}
