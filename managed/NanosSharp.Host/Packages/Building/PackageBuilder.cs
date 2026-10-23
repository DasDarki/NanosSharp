using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using NanosSharp.Host.Compilation;
using NanosSharp.Host.Packages.Dependencies;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages.Building;

internal static class PackageBuilder
{
#if CLIENT
    public const string SideDirectoryName = "Client";
    public const bool IsSandboxed = true;
#else
    public const string SideDirectoryName = "Server";
    public const bool IsSandboxed = false;
#endif

    public const string SharedDirectoryName = "Shared";
    public const string ClientDirectoryName = "Client";

    private const string ApiAssemblyName = "NanosSharp";
    private const string DependencyManifestExtension = ".deps.json";
    private static readonly string[] IgnoredDirectories = ["bin", "obj"];
    private static readonly string[] RelevantExtensions = [".cs", ".dll", ".pdb", ".so", ".csproj", ".json"];
    private static readonly string[] NativeRuntimeIdentifiers = ["linux-x64", "linux", "unix"];

    public static bool HasManagedCode(string packageDirectory)
    {
        var sideDirectory = Path.Combine(packageDirectory, SideDirectoryName);
        var sharedDirectory = Path.Combine(packageDirectory, SharedDirectoryName);
        return FindSources(sideDirectory).Any() || FindAssemblies(sideDirectory).Any() || FindSources(sharedDirectory).Any();
    }

    public static bool HasClientCode(string packageDirectory)
    {
        var clientDirectory = Path.Combine(packageDirectory, ClientDirectoryName);
        return FindSources(clientDirectory).Any() ||
            FindAssemblies(clientDirectory).Any() ||
            FindSources(Path.Combine(packageDirectory, SharedDirectoryName)).Any();
    }

    public static bool IsRelevantFile(string path) =>
        RelevantExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase) && !IsIgnored(path);

    public static PackageBuild Build(string packageName, string packageDirectory, IEnumerable<AssemblyImage> requiredImages,
        PackageBuildOptions options)
    {
        var sideDirectory = Path.Combine(packageDirectory, SideDirectoryName);
        var assemblyPaths = FindAssemblies(sideDirectory).ToArray();
        var assemblies = assemblyPaths.Select(ReadAssembly).ToDictionary(image => image.Name, StringComparer.Ordinal);
        var sources = FindSources(sideDirectory).Concat(FindSources(Path.Combine(packageDirectory, SharedDirectoryName))).ToArray();
        var dependencies = IsSandboxed ? ResolvedDependencies.Empty : RestoreDependencies(packageName, sideDirectory, sources.Length > 0, options);

        foreach (var runtimeAssembly in dependencies.RuntimeAssemblies.Select(ReadAssembly))
        {
            assemblies.TryAdd(runtimeAssembly.Name, runtimeAssembly);
        }

        if (sources.Length > 0)
        {
            var references = assemblyPaths.Concat(dependencies.CompileReferences).Select(ReadAssembly).Concat(requiredImages);
            var scripts = CompileScripts($"{packageName}.Scripts", sources, references, options.Optimize);
            assemblies[scripts.Name] = scripts;
        }

        var nativeLibraries = FindNativeLibraries(sideDirectory)
            .Concat(dependencies.NativeLibraries)
            .DistinctBy(entry => entry.Key)
            .ToDictionary(StringComparer.Ordinal);
        var manifests = assemblyPaths.Where(path => File.Exists(Path.ChangeExtension(path, DependencyManifestExtension))).ToArray();
        return new PackageBuild(assemblies, nativeLibraries, manifests);
    }

    private static ResolvedDependencies RestoreDependencies(string packageName, string sideDirectory, bool hasSources,
        PackageBuildOptions options)
    {
        var projectPath = ProjectFile.Find(sideDirectory);
        if (projectPath is null && hasSources && options.GenerateProjectFiles)
        {
            projectPath = ProjectGenerator.Generate(packageName, sideDirectory);
            LaunchConfiguration.Write(sideDirectory);
            Log.Info($"Generated {Path.GetFileName(projectPath)} for IDE support and NuGet references.");
        }

        if (projectPath is null)
        {
            return ResolvedDependencies.Empty;
        }

        try
        {
            return options.Restorer.Restore(ProjectFile.ReadPackageReferences(projectPath));
        }
        catch (Exception exception) when (exception is not PackageBuildException)
        {
            throw new PackageBuildException($"Restoring NuGet packages failed: {exception.Message}");
        }
    }

    private static AssemblyImage CompileScripts(string assemblyName, IReadOnlyList<string> sources,
        IEnumerable<AssemblyImage> references, bool optimize)
    {
        var result = ScriptCompiler.Compile(assemblyName, sources, references, optimize);
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

    private static IEnumerable<KeyValuePair<string, string>> FindNativeLibraries(string sideDirectory) =>
        IsSandboxed ? [] : NativeRuntimeIdentifiers
            .Select(runtimeIdentifier => Path.Combine(sideDirectory, "runtimes", runtimeIdentifier, "native"))
            .Prepend(sideDirectory)
            .Where(Directory.Exists)
            .SelectMany(directory => Directory.GetFiles(directory, "*.so*").Order(StringComparer.Ordinal))
            .Select(path => KeyValuePair.Create(Path.GetFileName(path), path));

    private static IEnumerable<string> FindSources(string directory) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !IsIgnored(path))
                .Order(StringComparer.Ordinal)
            : [];

    private static IEnumerable<string> FindAssemblies(string directory) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly)
                .Where(path => Path.GetFileNameWithoutExtension(path) != ApiAssemblyName)
                .Order(StringComparer.Ordinal)
            : [];

    private static bool IsIgnored(string path) =>
        path.Split(Path.DirectorySeparatorChar).Any(segment => IgnoredDirectories.Contains(segment));
}
