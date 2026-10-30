using Tomlyn;
using Tomlyn.Model;

namespace NanosSharp.Host.Packages;

internal sealed record PackageManifest(IReadOnlyList<string> Requirements, IReadOnlyList<string> Optional)
{
    private const string FileName = "Package.toml";
    private const string RequirementsKey = "packages_requirements";
    private const string OptionalKey = "packages_optional";

    /// <summary>Every package this one depends on, required and optional, for assembly sharing and reloads.</summary>
    public IReadOnlyList<string> Dependencies { get; } =
        Requirements.Concat(Optional).Distinct(StringComparer.Ordinal).ToArray();

    public static PackageManifest Read(string packageDirectory)
    {
        var path = Path.Combine(packageDirectory, FileName);
        if (!File.Exists(path))
        {
            return new PackageManifest([], []);
        }

        var model = TomlSerializer.Deserialize<TomlTable>(File.ReadAllText(path)) ?? [];
        var requirements = ReadList(model, RequirementsKey);
        var optional = ReadList(model, OptionalKey).Where(name => !requirements.Contains(name)).ToArray();
        return new PackageManifest(requirements, optional);
    }

    private static string[] ReadList(TomlTable model, string key) =>
        model.Values
            .OfType<TomlTable>()
            .Where(table => table.ContainsKey(key))
            .SelectMany(table => table[key] as TomlArray ?? [])
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();
}
