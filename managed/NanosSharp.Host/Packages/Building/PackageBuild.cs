namespace NanosSharp.Host.Packages.Building;

internal sealed record PackageBuild(
    IReadOnlyDictionary<string, AssemblyImage> Assemblies,
    IReadOnlyDictionary<string, string> NativeLibraries,
    IReadOnlyList<string> DependencyManifests)
{
    public AssemblyImage? Find(string name) => Assemblies.GetValueOrDefault(name);
}
