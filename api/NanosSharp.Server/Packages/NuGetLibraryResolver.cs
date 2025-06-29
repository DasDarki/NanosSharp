using System.IO.Compression;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The NuGet library resolver is responsible for resolving libraries from NuGet packages.
/// </summary>
internal static class NuGetLibraryResolver
{
    public static string? Resolve(string cacheDir, string assemblyName, string? preferredAssemblyVersion = null)
    {
        var logger = NullLogger.Instance;
        var providers = new List<Lazy<INuGetResourceProvider>>(Repository.Provider.GetCoreV3());
        var repo = new SourceRepository(new PackageSource("https://api.nuget.org/v3/index.json"), providers);

        var resource = repo.GetResource<FindPackageByIdResource>();
            
        using var cache = new SourceCacheContext();
        NuGetVersion? version;
        if (!string.IsNullOrEmpty(preferredAssemblyVersion))
        {
            version = NuGetVersion.Parse(preferredAssemblyVersion);
        }
        else
        {
            var versions = resource.GetAllVersionsAsync(assemblyName, cache, logger, CancellationToken.None).Result;
            version = versions?.Max();
            if (version == null)
                return null;
        }
        
        using var memStream = new MemoryStream();
        var success = resource.CopyNupkgToStreamAsync(
            assemblyName,
            version,
            memStream,
            cache,
            logger,
            CancellationToken.None).GetAwaiter().GetResult();

        if (!success)
            return null;
        
        using var zip = new ZipArchive(memStream, ZipArchiveMode.Read);
        var entry = zip.Entries.FirstOrDefault(e =>
            e.FullName.StartsWith("lib/", StringComparison.OrdinalIgnoreCase) &&
            e.FullName.EndsWith(assemblyName + ".dll", StringComparison.OrdinalIgnoreCase));

        if (entry == null)
            return null;

        var targetDll = Path.Combine(cacheDir, $"{assemblyName}.{version}.dll");
        using var entryStream = entry.Open();
        using var fs = new FileStream(targetDll, FileMode.Create, FileAccess.Write);
        entryStream.CopyTo(fs);

        if (string.IsNullOrEmpty(preferredAssemblyVersion))
        {
            var latestFile = Path.Combine(cacheDir, assemblyName + ".latest");
            IOFile.WriteAllText(latestFile, version.ToNormalizedString());
        }
        
        return null;
    }
}