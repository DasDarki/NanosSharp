namespace NanosSharp.Core;

/// <summary>
/// Exports a static class as a Lua table of its public static methods, or a single static method as a global
/// function, when the package starts.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public sealed class ExportAttribute(string? name = null) : Attribute
{
    /// <summary>Global name of the export, the class or method name when omitted.</summary>
    public string? Name { get; } = name;
}
