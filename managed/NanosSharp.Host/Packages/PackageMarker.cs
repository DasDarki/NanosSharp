using System.Text.Json;
using System.Text.Json.Nodes;
using NanosSharp.Core;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages;

internal static class PackageMarker
{
    private const string ProtocolProperty = "protocol";
    private const string VersionProperty = "version";
    private static readonly Dictionary<nint, WrittenMarker> WrittenMarkers = [];

    public static void Write(nint package, string packageDirectory)
    {
        if (!PackageBuilder.HasClientCode(packageDirectory))
        {
            return;
        }

        var clientDirectory = Path.Combine(packageDirectory, PackageBuilder.ClientDirectoryName);
        var path = Path.Combine(clientDirectory, Handshake.MarkerFileName);
        if (File.Exists(path))
        {
            Log.Warning($"{path} is reserved by NanosSharp and is replaced for the client transfer.");
        }

        var createdDirectory = !Directory.Exists(clientDirectory);
        Directory.CreateDirectory(clientDirectory);
        var content = new JsonObject
        {
            [ProtocolProperty] = Handshake.ProtocolVersion,
            [VersionProperty] = Handshake.RuntimeVersion,
        };
        File.WriteAllText(path, content.ToJsonString());
        WrittenMarkers[package] = new WrittenMarker(path, createdDirectory ? clientDirectory : null);
    }

    public static void Remove(nint package)
    {
        if (!WrittenMarkers.Remove(package, out var marker))
        {
            return;
        }

        File.Delete(marker.Path);
        if (marker.CreatedDirectory is { } directory && !Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }
    }

    public static string? FindIncompatibility(string packageDirectory)
    {
        var path = Path.Combine(packageDirectory, PackageBuilder.ClientDirectoryName, Handshake.MarkerFileName);
        if (!File.Exists(path))
        {
            return "the server does not run NanosSharp";
        }

        try
        {
            var content = JsonNode.Parse(File.ReadAllText(path));
            var protocol = content?[ProtocolProperty]?.GetValue<int>();
            var version = content?[VersionProperty]?.GetValue<string>();
            return protocol == Handshake.ProtocolVersion
                ? null
                : $"the server runs NanosSharp {version} with protocol {protocol}, this client supports protocol {Handshake.ProtocolVersion}";
        }
        catch (Exception exception) when (exception is JsonException or InvalidOperationException or FormatException)
        {
            return $"the NanosSharp marker of the server is invalid ({exception.Message})";
        }
    }

    private sealed record WrittenMarker(string Path, string? CreatedDirectory);
}
