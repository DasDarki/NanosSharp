using System.Reflection;
using System.Xml;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The package library resolver is responsible for resolving the package libraries through different means.
/// </summary>
internal static class PackageLibraryResolver
{
    private static readonly Assembly _nanosSharpServerAssembly = typeof(PackageLibraryResolver).Assembly;
    
    /// <summary>
    /// Tries to resolve the assembly from the given assembly name.
    /// </summary>
    /// <param name="packagePath">Used for a fallback if the assembly cannot be resolved.</param>
    /// <param name="assemblyName">The name of the assembly to resolve.</param>
    /// <param name="preferredAssemblyVersion">An optional preferred version of the assembly to resolve. If not provided, the latest version will be used.</param>
    /// <returns>The resolved assemblies path, or null if the assembly could not be resolved.</returns>
    /// <remarks>
    /// See <see cref="PackageLoader"/> for the order in which the assembly is resolved.
    /// </remarks>
    public static string? Resolve(string packagePath, string assemblyName, string? preferredAssemblyVersion = null)
    {
        // resolve possible version from csproj
        preferredAssemblyVersion ??= FindWantedAssemblyVersion(packagePath, assemblyName);
        
        // 1. Check if already loaded
        var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a =>
            {
                if (a.GetName().Name == assemblyName && !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                {
                    if (preferredAssemblyVersion != null)
                    {
                        return a.GetName().Version?.ToString() == preferredAssemblyVersion;
                    }
                    
                    return true;
                }
                
                return false;
            });

        if (alreadyLoaded != null)
            return alreadyLoaded.Location;
        
        // 2. Check local plugin directory
        var localPath = Path.Combine(packagePath, assemblyName + ".dll");
        if (IOFile.Exists(localPath))
            return localPath;
        
        // 3. Check global package cache
        var cachedPackage = GetCachedPackage(assemblyName, preferredAssemblyVersion);
        if (cachedPackage != null)
            return cachedPackage;
        
        // 4. Load from NuGet
        var nugetPackage = NuGetLibraryResolver.Resolve(GetCachePath(), assemblyName, preferredAssemblyVersion);
        return nugetPackage;
    }
    
    private static string? GetCachedPackage(string packageName, string? version = null)
    {
        var cachePath = GetCachePath();
        if (version == null)
        {
            var latestFile = Path.Combine(cachePath, packageName + ".latest");
            version = IOFile.Exists(latestFile) ? IOFile.ReadAllText(latestFile).Trim() : "latest";
        }
        
        var packageFile = Path.Combine(cachePath, packageName + "." + version + ".dll");
        if (IOFile.Exists(packageFile))
        {
            return packageFile;
        }

        return null;
    }

    private static string GetCachePath()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "dotnet", ".cache", "packages");
        Directory.CreateDirectory(path);
        return path;
    }

    private static string? FindWantedAssemblyVersion(string packagePath, string assemblyName)
    {
        var csprojPath = FindCsProj(packagePath, assemblyName);
        if (csprojPath == null)
        {
            return null;
        }
        
        var doc = new XmlDocument();
        try
        {
            doc.Load(csprojPath);
        }
        catch (XmlException)
        {
            return null; // Invalid XML, cannot parse
        }
        
        // find the <PackageReference> element for the assembly
        var packageReference = doc.SelectSingleNode($"/Project/ItemGroup/PackageReference[@Include='{assemblyName}']");

        // Get the Version attribute
        var versionAttribute = packageReference?.Attributes?["Version"];
        if (versionAttribute == null)
        {
            return null; // No version specified
        }
        
        var version = versionAttribute.Value;
        if (string.IsNullOrEmpty(version))
        {
            return null; // Empty version
        }
        
        // Check if the version is a valid semantic version
        if (Version.TryParse(version, out var parsedVersion))
        {
            return parsedVersion.ToString();
        }
        
        // If not a valid version, return null
        return null;
    }
    
    private static string? FindCsProj(string packagePath, string packageName)
    {
        var csprojPath = Path.Combine(packagePath, $"{packageName}.csproj");
        if (IOFile.Exists(csprojPath))
        {
            return csprojPath;
        }
        
        // If not found, search for any .csproj file in the directory
        var files = Directory.GetFiles(packagePath, "*.csproj", SearchOption.TopDirectoryOnly);
        return files.Length > 0 ? files[0] : null;
    }
}