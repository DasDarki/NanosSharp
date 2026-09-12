using Tomlyn;
using Tomlyn.Model;

namespace NanosSharp.Host.Packages;

internal sealed record PackageManifest(IReadOnlyList<string> Requirements)
{
    private const string FileName = "Package.toml";
    private const string RequirementsKey = "packages_requirements";

    public static PackageManifest Read(string packageDirectory)
    {
        var path = Path.Combine(packageDirectory, FileName);
        if (!File.Exists(path))
        {
            return new PackageManifest([]);
        }

        var model = Toml.ToModel(File.ReadAllText(path), path);
        var requirements = model.Values
            .OfType<TomlTable>()
            .Where(table => table.ContainsKey(RequirementsKey))
            .SelectMany(table => table[RequirementsKey] as TomlArray ?? [])
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return new PackageManifest(requirements);
    }
}
