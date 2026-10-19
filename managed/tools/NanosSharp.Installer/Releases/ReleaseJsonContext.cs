using System.Text.Json.Serialization;

namespace NanosSharp.Installer.Releases;

[JsonSerializable(typeof(GitHubRelease))]
internal sealed partial class ReleaseJsonContext : JsonSerializerContext;
