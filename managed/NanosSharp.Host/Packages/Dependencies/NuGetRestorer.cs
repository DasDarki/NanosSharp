using System.IO.Compression;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NanosSharp.Host.Packages.Dependencies;

internal sealed class NuGetRestorer(IReadOnlyList<string> sources, string cacheDirectory)
{
    private const string CompleteMarkerFileName = ".nanossharp-complete";
    private static readonly NuGetFramework TargetFramework = NuGetFramework.Parse("net10.0");
    private static readonly string[] RuntimeIdentifiers = ["linux-x64", "linux", "unix"];
    private static readonly string[] IgnoredPackagePrefixes = ["NETStandard.Library", "Microsoft.NETCore.", "NanosSharp"];
    private static readonly HashSet<string> FrameworkAssemblies = LoadFrameworkAssemblies();

    private readonly Dictionary<string, IReadOnlyList<NuGetVersion>> _availableVersions = new(StringComparer.OrdinalIgnoreCase);

    public ResolvedDependencies Restore(IReadOnlyList<PackageReference> references) =>
        references.Count == 0 ? ResolvedDependencies.Empty : Task.Run(() => RestoreAsync(references)).GetAwaiter().GetResult();

    private async Task<ResolvedDependencies> RestoreAsync(IReadOnlyList<PackageReference> references)
    {
        using var cache = new SourceCacheContext();
        var repositories = sources.Select(source => Repository.Factory.GetCoreV3(source)).ToArray();
        var selected = new Dictionary<string, (NuGetVersion Version, string Directory)>(StringComparer.OrdinalIgnoreCase);
        var pending = new Queue<PackageReference>(references);

        while (pending.TryDequeue(out var reference))
        {
            var version = await FindVersionAsync(repositories, cache, reference);
            if (selected.TryGetValue(reference.Id, out var existing) && existing.Version >= version)
            {
                continue;
            }

            var directory = await EnsureExtractedAsync(repositories, cache, reference.Id, version);
            selected[reference.Id] = (version, directory);
            foreach (var dependency in ReadDependencies(directory))
            {
                pending.Enqueue(dependency);
            }
        }

        return CollectAssets(selected.Values.Select(entry => entry.Directory).Order(StringComparer.Ordinal));
    }

    private async Task<NuGetVersion> FindVersionAsync(SourceRepository[] repositories, SourceCacheContext cache,
        PackageReference reference)
    {
        var cachedMinimum = reference.Version.MinVersion;
        if (cachedMinimum is not null && reference.Version.IsMinInclusive && IsExtracted(reference.Id, cachedMinimum))
        {
            return cachedMinimum;
        }

        var versions = await GetAvailableVersionsAsync(repositories, cache, reference.Id);
        return reference.Version.FindBestMatch(versions) ??
               throw new InvalidOperationException($"No version of NuGet package {reference.Id} matches {reference.Version}.");
    }

    private async Task<IReadOnlyList<NuGetVersion>> GetAvailableVersionsAsync(SourceRepository[] repositories,
        SourceCacheContext cache, string id)
    {
        if (_availableVersions.TryGetValue(id, out var known))
        {
            return known;
        }

        var versions = new List<NuGetVersion>();
        foreach (var resource in await GetResourcesAsync(repositories))
        {
            versions.AddRange(await resource.GetAllVersionsAsync(id, cache, NullLogger.Instance, CancellationToken.None));
        }

        var distinct = versions.Distinct().Order().ToArray();
        _availableVersions[id] = distinct;
        return distinct;
    }

    private async Task<string> EnsureExtractedAsync(SourceRepository[] repositories, SourceCacheContext cache, string id,
        NuGetVersion version)
    {
        var directory = GetPackageDirectory(id, version);
        if (IsExtracted(id, version))
        {
            return directory;
        }

        using var package = new MemoryStream();
        if (!await TryDownloadAsync(repositories, cache, id, version, package))
        {
            throw new InvalidOperationException($"NuGet package {id} {version} could not be downloaded.");
        }

        package.Position = 0;
        Directory.CreateDirectory(directory);
        ZipFile.ExtractToDirectory(package, directory, true);
        await File.WriteAllTextAsync(Path.Combine(directory, CompleteMarkerFileName), version.ToNormalizedString());
        return directory;
    }

    private bool IsExtracted(string id, NuGetVersion version) =>
        File.Exists(Path.Combine(GetPackageDirectory(id, version), CompleteMarkerFileName));

    private string GetPackageDirectory(string id, NuGetVersion version) =>
        Path.Combine(cacheDirectory, id.ToLowerInvariant(), version.ToNormalizedString().ToLowerInvariant());

    private static async Task<bool> TryDownloadAsync(SourceRepository[] repositories, SourceCacheContext cache, string id,
        NuGetVersion version, Stream destination)
    {
        foreach (var resource in await GetResourcesAsync(repositories))
        {
            if (await resource.CopyNupkgToStreamAsync(id, version, destination, cache, NullLogger.Instance, CancellationToken.None))
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<List<FindPackageByIdResource>> GetResourcesAsync(SourceRepository[] repositories)
    {
        var resources = new List<FindPackageByIdResource>();
        foreach (var repository in repositories)
        {
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            if (resource is not null)
            {
                resources.Add(resource);
            }
        }

        return resources;
    }

    private static IEnumerable<PackageReference> ReadDependencies(string directory)
    {
        using var reader = new PackageFolderReader(directory);
        var groups = reader.GetPackageDependencies().ToArray();
        var nearest = new FrameworkReducer().GetNearest(TargetFramework, groups.Select(group => group.TargetFramework));
        var dependencies = groups.FirstOrDefault(group => group.TargetFramework.Equals(nearest))?.Packages ?? [];
        return dependencies
            .Where(dependency => !IsIgnoredPackage(dependency.Id))
            .Select(dependency => new PackageReference(dependency.Id, dependency.VersionRange))
            .ToArray();
    }

    private static ResolvedDependencies CollectAssets(IEnumerable<string> packageDirectories)
    {
        var compileReferences = new List<string>();
        var runtimeAssemblies = new List<string>();
        var nativeLibraries = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var directory in packageDirectories)
        {
            using var reader = new PackageFolderReader(directory);
            var libraries = FindRuntimeLibraries(reader, directory);
            if (libraries.Count > 0 && libraries.All(IsFrameworkAssembly))
            {
                continue;
            }

            var references = NearestItems(reader.GetItems("ref"), directory);
            compileReferences.AddRange(references.Count > 0 ? references : libraries);
            runtimeAssemblies.AddRange(libraries);
            foreach (var native in FindNativeLibraries(directory))
            {
                nativeLibraries.TryAdd(Path.GetFileName(native), native);
            }
        }

        return new ResolvedDependencies(compileReferences, runtimeAssemblies, nativeLibraries);
    }

    private static List<string> FindRuntimeLibraries(PackageFolderReader reader, string directory)
    {
        foreach (var runtimeIdentifier in RuntimeIdentifiers)
        {
            var runtimeDirectory = Path.Combine(directory, "runtimes", runtimeIdentifier, "lib");
            var libraries = NearestFrameworkFolder(runtimeDirectory);
            if (libraries.Count > 0)
            {
                return libraries;
            }
        }

        return NearestItems(reader.GetLibItems(), directory);
    }

    private static List<string> NearestFrameworkFolder(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var folders = Directory.GetDirectories(directory).ToDictionary(folder => NuGetFramework.ParseFolder(Path.GetFileName(folder)));
        var nearest = new FrameworkReducer().GetNearest(TargetFramework, folders.Keys);
        return nearest is null ? [] : Directory.GetFiles(folders[nearest], "*.dll").Order(StringComparer.Ordinal).ToList();
    }

    private static List<string> NearestItems(IEnumerable<FrameworkSpecificGroup> groups, string directory)
    {
        var groupList = groups.ToArray();
        var nearest = new FrameworkReducer().GetNearest(TargetFramework, groupList.Select(group => group.TargetFramework));
        var items = groupList.FirstOrDefault(group => group.TargetFramework.Equals(nearest))?.Items ?? [];
        return items
            .Where(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(item => Path.Combine(directory, item))
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    private static IEnumerable<string> FindNativeLibraries(string directory) =>
        RuntimeIdentifiers
            .Select(runtimeIdentifier => Path.Combine(directory, "runtimes", runtimeIdentifier, "native"))
            .Where(Directory.Exists)
            .SelectMany(nativeDirectory => Directory.GetFiles(nativeDirectory).Order(StringComparer.Ordinal));

    private static bool IsIgnoredPackage(string id) =>
        IgnoredPackagePrefixes.Any(prefix => id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    private static bool IsFrameworkAssembly(string path) => FrameworkAssemblies.Contains(Path.GetFileNameWithoutExtension(path));

    private static HashSet<string> LoadFrameworkAssemblies() =>
        (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(Path.GetFileNameWithoutExtension)
            .OfType<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
}
