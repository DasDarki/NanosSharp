using System.Collections.Concurrent;
using NanosSharp.Server.Configuration;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The internal component which manages all plugins/packages in the space of the C# server.
/// </summary>
public sealed class PackageManager
{
    private readonly ConcurrentDictionary<string, LoadedPackage> _packages = new();

    /// <summary>
    /// Loads all packages with the given names. This method will search for the packages in package directory and load them if found.
    /// This method will also build a dependency tree in order to load packages in the correct order based on their dependencies.
    /// </summary>
    /// <param name="packageNames">A list of package names to load.</param>
    internal void LoadAll(List<string> packageNames)
    {
        GetPackagesDirectory();
        
        var packageOrder = new List<(string Path, string Name, PackageConfig Config, int SoftCount, int HardCount)>();
        foreach (var packageName in packageNames)
        {
            var packagePath = GetPackagePath(packageName);
            if (!Directory.Exists(packagePath))
            {
                continue;
            }

            var config = PackageConfig.LoadFromFile(packagePath);
            if (config == null)
            {
                ServerModule.Logger.Fatal("Package '{X}' has no valid configuration file ({Y}).", packageName, "Package.cs.toml");
                continue;
            }

            packageOrder.Add((packagePath, packageName, config, config.SoftDependencies.Count, config.HardDependencies.Count));
        }
        
        packageOrder.Sort((a, b) =>
        {
            if (a.HardCount != b.HardCount)
            {
                return b.HardCount.CompareTo(a.HardCount); // Hard dependencies first
            }
            return b.SoftCount.CompareTo(a.SoftCount); // Then soft dependencies
        });

        foreach (var pkg in packageOrder)
        {
            LoadPackageInternal(pkg.Path, pkg.Name, pkg.Config);
        }
    }

    /// <summary>
    /// Loads the package with the given name. This method will search for the package in package directory and load it if found.
    /// If the package is already loaded, it will not be loaded again.
    /// </summary>
    /// <param name="name">The name of the package to load.</param>
    public void LoadPackage(string name)
    {
        var packagePath = GetPackagePath(name);
        if (!Directory.Exists(packagePath))
        {
            ServerModule.Logger.Warn("Package '{X}' not found in the package directory.", name);
            return;
        }
        
        if (_packages.ContainsKey(name))
        {
            ServerModule.Logger.Warn("Package '{X}' is already loaded.", name);
            return;
        }
        
        var config = PackageConfig.LoadFromFile(packagePath);
        if (config == null)
        {
            ServerModule.Logger.Fatal("Package '{X}' has no valid configuration file ({Y}).", name, "Package.cs.toml");
            return;
        }

        LoadPackageInternal(packagePath, name, config);
    }

    private void LoadPackageInternal(string packagePath, string name, PackageConfig config)
    {
        var debug = ServerModule.Config.DebugPackages || config.IsDebug;
        var pkg = PackageLoader.Load(packagePath, debug);
        if (pkg == null)
        {
            ServerModule.Logger.Fatal("Failed to load package '{X}'.", name);
            return;
        }
        
        var (assembly, context) = pkg.Value;
        
        var packageType = assembly.GetTypes().FirstOrDefault(t => t.BaseType?.FullName == typeof(Package).FullName && !t.IsAbstract);
        if (packageType == null)
        {
            ServerModule.Logger.Fatal("Package '{X}' does not contain a valid main class.", name);
            return;
        }
        
        var packageInstance = CreatePackageInstance(packageType);
        if (packageInstance == null)
        {
            ServerModule.Logger.Fatal("Failed to create an instance of the package '{X}' from type '{Y}'.", name, packageType.FullName ?? "<null>");
            return;
        }

        packageInstance.Logger = new Logger(name, debug);
        packageInstance.Config = config;
        packageInstance.Directory = packagePath;

        var loadedPackage = new LoadedPackage(context, assembly, packageInstance);
        ServerModule.Logger.Info("Package '{X}' loaded successfully.", name);
        
        try
        {
            packageInstance.OnStart();
            _packages[name] = loadedPackage;
            ServerModule.Logger.Info("Package '{X}' started successfully.", name);
        }
        catch (Exception ex)
        {
            ServerModule.Logger.Error("Failed to start package '{X}': {Y}", name, ex.Message);
        }
    }
    
    private Package? CreatePackageInstance(Type packageType)
    {
        try
        {
            return (Package)Activator.CreateInstance(packageType)!;
        }
        catch
        {
            return null;
        }
    }

    private string GetPackagePath(string name)
    {
        return Path.Combine(GetPackagesDirectory(), name);
    }
    
    private string GetPackagesDirectory()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Packages_NanosSharp");
        Directory.CreateDirectory(path);
        return path;
    }
}