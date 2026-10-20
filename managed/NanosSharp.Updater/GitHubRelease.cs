using System.Text.Json.Serialization;

namespace NanosSharp.Updater;

internal sealed class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("assets")]
    public List<GitHubAsset> Assets { get; set; } = [];
}

internal sealed class GitHubAsset
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; } = string.Empty;
}

[JsonSerializable(typeof(GitHubRelease))]
internal sealed partial class ReleaseJsonContext : JsonSerializerContext;
