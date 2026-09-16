namespace NanosSharp.Host.Packages.Dependencies;

internal sealed record ResolvedDependencies(
    IReadOnlyList<string> CompileReferences,
    IReadOnlyList<string> RuntimeAssemblies,
    IReadOnlyDictionary<string, string> NativeLibraries)
{
    public static ResolvedDependencies Empty { get; } = new([], [], new Dictionary<string, string>());
}
