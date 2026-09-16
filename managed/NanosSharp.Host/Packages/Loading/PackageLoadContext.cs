using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Interop;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages.Loading;

internal sealed class PackageLoadContext(string packageName, PackageBuild build, Func<AssemblyName, Assembly?> resolveShared)
    : AssemblyLoadContext($"NanosSharp:{packageName}", true)
{
    private static readonly Assembly ApiAssembly = typeof(Package).Assembly;

    private readonly AssemblyDependencyResolver[] _resolvers =
        build.DependencyManifests.Select(path => new AssemblyDependencyResolver(path)).ToArray();

    public IEnumerable<Assembly> LoadBuild() =>
        build.Assemblies.Keys.Order(StringComparer.Ordinal).Select(name => LoadFromAssemblyName(new AssemblyName(name))).ToArray();

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == ApiAssembly.GetName().Name)
        {
            return ApiAssembly;
        }

        if (assemblyName.Name is { } name && build.Find(name) is { } image)
        {
            return LoadImage(image.Image, image.Symbols);
        }

        var resolvedPath = _resolvers.Select(resolver => resolver.ResolveAssemblyToPath(assemblyName)).FirstOrDefault(path => path is not null);
        return resolvedPath is null ? resolveShared(assemblyName) : LoadImage(File.ReadAllBytes(resolvedPath), null);
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var candidates = new[] { unmanagedDllName, $"lib{unmanagedDllName}.so", $"{unmanagedDllName}.so" };
        var bundledPath = candidates.Select(candidate => build.NativeLibraries.GetValueOrDefault(candidate)).FirstOrDefault(path => path is not null);
        var resolvedPath = bundledPath ?? _resolvers.Select(resolver => resolver.ResolveUnmanagedDllToPath(unmanagedDllName))
            .FirstOrDefault(path => path is not null);
        return resolvedPath is null ? 0 : NativeRuntime.LoadIsolatedLibrary(resolvedPath);
    }

    private Assembly LoadImage(byte[] image, byte[]? symbols)
    {
        using var assemblyStream = new MemoryStream(image);
        using var symbolStream = symbols is null ? null : new MemoryStream(symbols);
        return LoadFromStream(assemblyStream, symbolStream);
    }
}
