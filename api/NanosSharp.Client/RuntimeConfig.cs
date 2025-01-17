using Tomlet;
using Tomlet.Attributes;

namespace NanosSharp.Client;

/// <summary>
/// The runtime configuration of the NanosSharp client.
/// </summary>
internal sealed class RuntimeConfig
{
    [TomlProperty("debug")] 
    [TomlPrecedingComment("If the client should run in debug mode.")]
    public bool IsDebug { get; set; }

    /// <summary>
    /// Loads the current runtime config file. If the file does not exist, a new one will be created.
    /// </summary>
    /// <returns>The runtime config.</returns>
    internal static RuntimeConfig Load()
    {
        var path = Path.Combine(Environment.CurrentDirectory, ".config.toml");
        if (!File.Exists(path))
        {
            var config = new RuntimeConfig();
            File.WriteAllText(path, TomletMain.TomlStringFrom(config));
            return config;
        }
        
        return TomletMain.To<RuntimeConfig>(File.ReadAllText(path));
    }
}