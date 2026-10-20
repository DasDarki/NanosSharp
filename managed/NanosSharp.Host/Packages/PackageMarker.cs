using System.Text.Json;
using System.Text.Json.Nodes;
using NanosSharp.Core;
using NanosSharp.Diagnostics;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages;

internal static class PackageMarker
{
    private const string ProtocolProperty = "protocol";
    private const string VersionProperty = "version";
    private const string DebugProperty = "debug";
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
            [DebugProperty] = DebugState.IsEnabled,
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

    public static MarkerState Read(string packageDirectory)
    {
        var path = Path.Combine(packageDirectory, PackageBuilder.ClientDirectoryName, Handshake.MarkerFileName);
        if (!File.Exists(path))
        {
            return new MarkerState("the server does not run NanosSharp", false);
        }

        try
        {
            var content = JsonNode.Parse(File.ReadAllText(path));
            var protocol = content?[ProtocolProperty]?.GetValue<int>();
            var version = content?[VersionProperty]?.GetValue<string>();
            var debug = content?[DebugProperty]?.GetValue<bool>() ?? false;
            return protocol == Handshake.ProtocolVersion
                ? new MarkerState(null, debug)
                : new MarkerState($"the server runs NanosSharp {version} with protocol {protocol}, this client supports protocol {Handshake.ProtocolVersion}", false);
        }
        catch (Exception exception) when (exception is JsonException or InvalidOperationException or FormatException)
        {
            return new MarkerState($"the NanosSharp marker of the server is invalid ({exception.Message})", false);
        }
    }

    private sealed record WrittenMarker(string Path, string? CreatedDirectory);
}
