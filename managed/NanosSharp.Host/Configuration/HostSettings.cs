using System.Text.Json.Serialization;
using Tomlyn;

namespace NanosSharp.Host.Configuration;

internal sealed class HostSettings
{
    public HotReloadSettings HotReload { get; set; } = new();

    [JsonPropertyName("nuget")]
    public NuGetSettings NuGet { get; set; } = new();

    public ProjectSettings Projects { get; set; } = new();

    public static HostSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            var defaults = new HostSettings();
            File.WriteAllText(path, TomlSerializer.Serialize(defaults, HostSettingsContext.Default.HostSettings));
            return defaults;
        }

        return TomlSerializer.Deserialize(File.ReadAllText(path), HostSettingsContext.Default.HostSettings) ?? new HostSettings();
    }
}
