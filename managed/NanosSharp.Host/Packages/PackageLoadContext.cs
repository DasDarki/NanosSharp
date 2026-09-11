using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages;

internal sealed class PackageLoadContext(string name, string directory) : AssemblyLoadContext($"NanosSharp:{name}", true)
{
    private static readonly Assembly ApiAssembly = typeof(Package).Assembly;

    public Assembly LoadFromBytes(byte[] assembly, byte[]? symbols = null)
    {
        using var assemblyStream = new MemoryStream(assembly);
        using var symbolStream = symbols is null ? null : new MemoryStream(symbols);
        return LoadFromStream(assemblyStream, symbolStream);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == ApiAssembly.GetName().Name)
        {
            return ApiAssembly;
        }

        var path = Path.Combine(directory, $"{assemblyName.Name}.dll");
        if (!File.Exists(path))
        {
            return null;
        }

        var symbolsPath = Path.ChangeExtension(path, ".pdb");
        return LoadFromBytes(File.ReadAllBytes(path), File.Exists(symbolsPath) ? File.ReadAllBytes(symbolsPath) : null);
    }
}
