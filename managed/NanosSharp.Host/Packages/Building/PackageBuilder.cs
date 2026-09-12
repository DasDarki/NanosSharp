using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using NanosSharp.Host.Compilation;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages.Building;

internal static class PackageBuilder
{
    public const string ServerDirectoryName = "Server";

    private const string ApiAssemblyName = "NanosSharp";
    private static readonly string[] IgnoredDirectories = ["bin", "obj"];

    public static bool HasManagedCode(string packageDirectory)
    {
        var serverDirectory = Path.Combine(packageDirectory, ServerDirectoryName);
        return Directory.Exists(serverDirectory) &&
               (FindSources(serverDirectory).Any() || FindAssemblies(serverDirectory).Any());
    }

    public static bool IsRelevantFile(string path) =>
        Path.GetExtension(path) is ".cs" or ".dll" or ".pdb" && !IsIgnored(path);

    public static PackageBuild Build(string packageName, string packageDirectory, IEnumerable<AssemblyImage> references)
    {
        var serverDirectory = Path.Combine(packageDirectory, ServerDirectoryName);
        var assemblies = FindAssemblies(serverDirectory)
            .Select(ReadAssembly)
            .ToDictionary(image => image.Name, StringComparer.Ordinal);

        var sources = FindSources(serverDirectory).ToArray();
        if (sources.Length > 0)
        {
            var scripts = CompileScripts($"{packageName}.Scripts", sources, assemblies.Values.Concat(references));
            assemblies[scripts.Name] = scripts;
        }

        return new PackageBuild(assemblies);
    }

    private static AssemblyImage CompileScripts(string assemblyName, IReadOnlyList<string> sources,
        IEnumerable<AssemblyImage> references)
    {
        var result = ScriptCompiler.Compile(assemblyName, sources, references);
        foreach (var diagnostic in result.Diagnostics)
        {
            Log.Warning(diagnostic);
        }

        return result.Succeeded
            ? new AssemblyImage(assemblyName, result.Assembly!, result.Symbols)
            : throw new PackageBuildException($"Compilation of {sources.Count} C# file(s) failed.");
    }

    private static AssemblyImage ReadAssembly(string path)
    {
        try
        {
            var image = File.ReadAllBytes(path);
            var symbolsPath = Path.ChangeExtension(path, ".pdb");
            var symbols = File.Exists(symbolsPath) ? File.ReadAllBytes(symbolsPath) : null;
            return new AssemblyImage(ReadAssemblyName(image, path), image, symbols);
        }
        catch (IOException exception)
        {
            throw new PackageBuildException($"{Path.GetFileName(path)} is not readable yet: {exception.Message}", true);
        }
    }

    private static string ReadAssemblyName(byte[] image, string path)
    {
        try
        {
            using var reader = new PEReader(new MemoryStream(image));
            var metadata = reader.GetMetadataReader();
            return metadata.GetString(metadata.GetAssemblyDefinition().Name);
        }
        catch (Exception exception) when (exception is BadImageFormatException or InvalidOperationException)
        {
            throw new PackageBuildException($"{Path.GetFileName(path)} is not a valid .NET assembly.", true);
        }
    }

    private static IEnumerable<string> FindSources(string serverDirectory) =>
        Directory.EnumerateFiles(serverDirectory, "*.cs", SearchOption.AllDirectories).Where(path => !IsIgnored(path));

    private static IEnumerable<string> FindAssemblies(string serverDirectory) =>
        Directory.EnumerateFiles(serverDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetFileNameWithoutExtension(path) != ApiAssemblyName);

    private static bool IsIgnored(string path) =>
        path.Split(Path.DirectorySeparatorChar).Any(segment => IgnoredDirectories.Contains(segment));
}
