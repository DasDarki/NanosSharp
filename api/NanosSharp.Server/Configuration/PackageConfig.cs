using Tomlet;
using Tomlet.Attributes;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Configuration;

/// <summary>
/// The package configuration for NanosSharp packages.
/// </summary>
public sealed class PackageConfig
{
    /// <summary>
    /// The name of the package used to identify it in the system.
    /// </summary>
    [TomlProperty("name")]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// The version of the package. This is used to identify the version of the package in the system.
    /// </summary>
    [TomlProperty("version")]
    public string Version { get; set; } = "0.0.0";
    
    /// <summary>
    /// The author of the package.
    /// </summary>
    [TomlProperty("author")]
    public string Author { get; set; } = "Unknown";
    
    /// <summary>
    /// Other packages that this package depends on. This is used to ensure that all dependencies are loaded before this package.
    /// These are only soft dependencies, meaning that the package can still function without them, but they may provide additional functionality or features.
    /// </summary>
    [TomlProperty("soft_deps")]
    public List<string> SoftDependencies { get; set; } = [];
    
    /// <summary>
    /// Other packages that this package hard depends on. This is used to ensure that all dependencies are loaded before this package.
    /// These are hard dependencies, meaning that the package cannot function without them and will not load if they are not present.
    /// </summary>
    [TomlProperty("hard_deps")]
    public List<string> HardDependencies { get; set; } = [];
    
    /// <summary>
    /// Other C# libraries that this package depends on. This is used to ensure that all libraries are loaded before this package.
    /// The key are the exact assembly names of the libraries, and the value is the wanted version of the library. If "*"
    /// is used as the version, the latest version will be used.
    /// </summary>
    [TomlProperty("libs")]
    public Dictionary<string, string> Libraries { get; set; } = new();
    
    /// <summary>
    /// Whether or not the package should run in debug mode. This is used to enable additional logging and debugging features.
    /// </summary>
    [TomlProperty("debug")]
    public bool IsDebug { get; set; } = false;

    internal static PackageConfig? LoadFromFile(string dir)
    {
        var path = Path.Combine(dir, "Package.cs.toml");
        if (!IOFile.Exists(path))
        {
            return null;
        }
        
        try
        {
            return TomletMain.To<PackageConfig>(IOFile.ReadAllText(path));
        }
        catch
        {
            return null;
        }
    }
}