namespace NanosSharp.Host.Packages.Building;

internal sealed record PackageBuild(IReadOnlyDictionary<string, AssemblyImage> Assemblies)
{
    public AssemblyImage? Find(string name) => Assemblies.GetValueOrDefault(name);
}
