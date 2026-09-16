using System.Xml.Linq;
using NuGet.Versioning;

namespace NanosSharp.Host.Packages.Dependencies;

internal static class ProjectFile
{
    public static string? Find(string serverDirectory) =>
        Directory.EnumerateFiles(serverDirectory, "*.csproj", SearchOption.TopDirectoryOnly).Order(StringComparer.Ordinal).FirstOrDefault();

    public static IReadOnlyList<PackageReference> ReadPackageReferences(string projectPath)
    {
        var document = XDocument.Load(projectPath);
        return document.Descendants()
            .Where(element => element.Name.LocalName == "PackageReference")
            .Select(ReadPackageReference)
            .OrderBy(reference => reference.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static PackageReference ReadPackageReference(XElement element)
    {
        var id = element.Attribute("Include")?.Value ?? element.Attribute("Update")?.Value;
        var version = element.Attribute("Version")?.Value ?? element.Elements().FirstOrDefault(child => child.Name.LocalName == "Version")?.Value;
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(version))
        {
            throw new InvalidDataException($"PackageReference '{element}' needs an Include and a Version.");
        }

        return new PackageReference(id, VersionRange.Parse(version));
    }
}
