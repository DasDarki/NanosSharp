using Tomlet;
using Tomlet.Attributes;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Configuration;

/// <summary>
/// The internal runtime configuration for the server module of NanosSharp.
/// </summary>
internal sealed class RuntimeConfig
{
    [TomlPrecedingComment("Whether or not the NanosSharp server should run in debug mode.")]
    public bool Debug { get; set; }
    
    [TomlPrecedingComment("Overrides the default debug flag for each package. If true, all packages will run in debug mode.")]
    public bool DebugPackages { get; set; }
    
    [TomlPrecedingComment("The list of packages that are loaded by the NanosSharp server. These packages are loaded in the order they are defined here.")]
    public List<string> Packages { get; set; } = [];

    /// <summary>
    /// Loads or creates a new runtime configuration.
    /// </summary>
    internal static RuntimeConfig Load()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "NanosSharp.toml");
        if (IOFile.Exists(path))
        {
            return TomletMain.To<RuntimeConfig>(IOFile.ReadAllText(path));
        }

        var config = new RuntimeConfig();
        IOFile.WriteAllText(path, TomletMain.TomlStringFrom(config));
        return config;
    }
}