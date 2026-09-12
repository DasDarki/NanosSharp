using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages.Loading;

internal sealed class PackageLoadContext(string packageName, PackageBuild build, Func<AssemblyName, Assembly?> resolveShared)
    : AssemblyLoadContext($"NanosSharp:{packageName}", true)
{
    private static readonly Assembly ApiAssembly = typeof(Package).Assembly;

    public IEnumerable<Assembly> LoadBuild() =>
        build.Assemblies.Keys.Select(name => LoadFromAssemblyName(new AssemblyName(name))).ToArray();

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == ApiAssembly.GetName().Name)
        {
            return ApiAssembly;
        }

        if (assemblyName.Name is { } name && build.Find(name) is { } image)
        {
            using var assemblyStream = new MemoryStream(image.Image);
            using var symbolStream = image.Symbols is null ? null : new MemoryStream(image.Symbols);
            return LoadFromStream(assemblyStream, symbolStream);
        }

        return resolveShared(assemblyName);
    }
}
