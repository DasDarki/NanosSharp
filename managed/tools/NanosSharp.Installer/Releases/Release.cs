using System.Text.RegularExpressions;

namespace NanosSharp.Installer.Releases;

internal sealed partial record Release(string Version, IReadOnlyDictionary<string, string> Assets)
{
    public const string ChecksumsAsset = "SHA256SUMS";

    public static Release FromDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new InstallerException($"The directory {directory} does not exist.");
        }

        var files = Directory.EnumerateFiles(directory).ToDictionary(file => Path.GetFileName(file), file => file, StringComparer.Ordinal);
        var version = files.Keys.Select(name => AssetPattern().Match(name)).FirstOrDefault(match => match.Success)?.Groups["version"].Value
                      ?? throw new InstallerException($"{directory} contains no NanosSharp bundle.");
        return new Release(version, files);
    }

    public static string ServerAsset(string version) => $"nanossharp-{version}-linux-x64.tar.gz";

    public static string ClientAsset(string version) => $"nanossharp-client-{version}-win-x64.zip";

    [GeneratedRegex(@"^nanossharp-(?:client-)?(?<version>.+?)-(?:linux|win)-x64\.")]
    private static partial Regex AssetPattern();
}
