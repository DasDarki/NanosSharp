using System.Xml.Linq;
using NanosSharp.Host.Compilation;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages.Dependencies;

internal static class ProjectGenerator
{
    private const string SharedDirectoryName = "Shared";

    public static string Generate(string packageName, string sideDirectory, IReadOnlyList<string> dependencies)
    {
        var packageDirectory = Directory.GetParent(sideDirectory)!.FullName;
        var sharedProject = EnsureSharedProject(packageName, packageDirectory);

        var items = new XElement("ItemGroup", ApiReference(sideDirectory));
        if (sharedProject is not null)
        {
            items.Add(new XElement("ProjectReference", new XAttribute("Include", Path.GetRelativePath(sideDirectory, sharedProject))));
        }

        foreach (var reference in DependencyReferences(sideDirectory, dependencies))
        {
            items.Add(reference);
        }

        items.Add(new XElement("None", new XAttribute("Include", "../Package.toml"), new XAttribute("Link", "Package.toml")));

        var path = Path.Combine(sideDirectory, $"{packageName}.csproj");
        var project = new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            Properties($"{packageName}.Scripts", ScriptCompiler.SideSymbol),
            items,
            GlobalUsings());

        new XDocument(project).Save(path);
        return path;
    }

    private static string? EnsureSharedProject(string packageName, string packageDirectory)
    {
        var sharedDirectory = Path.Combine(packageDirectory, SharedDirectoryName);
        if (!Directory.Exists(sharedDirectory) || !HasSources(sharedDirectory))
        {
            return null;
        }

        var existing = ProjectFile.Find(sharedDirectory);
        if (existing is not null)
        {
            return existing;
        }

        var path = Path.Combine(sharedDirectory, $"{packageName}.Shared.csproj");
        var project = new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            Properties($"{packageName}.Shared", null),
            new XElement("ItemGroup", ApiReference(sharedDirectory)),
            GlobalUsings());

        new XDocument(project).Save(path);
        return path;
    }

    private static XElement Properties(string assemblyName, string? sideSymbol)
    {
        var group = new XElement("PropertyGroup",
            new XElement("TargetFramework", ScriptCompiler.TargetFramework),
            new XElement("LangVersion", "latest"),
            new XElement("Nullable", "enable"),
            new XElement("AllowUnsafeBlocks", "true"),
            new XElement("ImplicitUsings", "disable"),
            new XElement("AssemblyName", assemblyName));
        if (sideSymbol is not null)
        {
            group.Add(new XElement("DefineConstants", $"$(DefineConstants);{sideSymbol}"));
        }

        return group;
    }

    private static XElement ApiReference(string directory) =>
        new("Reference",
            new XAttribute("Include", "NanosSharp"),
            new XElement("HintPath", Path.GetRelativePath(directory, typeof(Package).Assembly.Location)),
            new XElement("Private", "false"));

    private static IEnumerable<XElement> DependencyReferences(string sideDirectory, IReadOnlyList<string> dependencies)
    {
        var sideName = Path.GetFileName(sideDirectory);
        var packagesRoot = Directory.GetParent(sideDirectory)?.Parent?.FullName;
        if (packagesRoot is null)
        {
            return [];
        }

        return dependencies.SelectMany(dependency =>
            ReferencesFor(sideDirectory, Path.Combine(packagesRoot, dependency), dependency, sideName));
    }

    private static IEnumerable<XElement> ReferencesFor(string sideDirectory, string dependencyDirectory, string dependency,
        string sideName)
    {
        var sideProject = Path.Combine(dependencyDirectory, sideName, $"{dependency}.csproj");
        if (File.Exists(sideProject))
        {
            return [ProjectReference(sideDirectory, sideProject)];
        }

        var sharedProject = Path.Combine(dependencyDirectory, SharedDirectoryName, $"{dependency}.Shared.csproj");
        if (File.Exists(sharedProject))
        {
            return [ProjectReference(sideDirectory, sharedProject)];
        }

        var assemblyDirectory = Path.Combine(dependencyDirectory, sideName);
        if (!Directory.Exists(assemblyDirectory))
        {
            return [];
        }

        return Directory.EnumerateFiles(assemblyDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetFileNameWithoutExtension(path) is not "NanosSharp")
            .Order(StringComparer.Ordinal)
            .Select(path => AssemblyReference(sideDirectory, path));
    }

    private static XElement ProjectReference(string sideDirectory, string projectPath) =>
        new("ProjectReference",
            new XAttribute("Include", Path.GetRelativePath(sideDirectory, projectPath)),
            new XElement("Private", "false"),
            new XElement("ReferenceOutputAssembly", "true"));

    private static XElement AssemblyReference(string sideDirectory, string assemblyPath) =>
        new("Reference",
            new XAttribute("Include", Path.GetFileNameWithoutExtension(assemblyPath)),
            new XElement("HintPath", Path.GetRelativePath(sideDirectory, assemblyPath)),
            new XElement("Private", "false"));

    private static XElement GlobalUsings() =>
        new("ItemGroup", ScriptCompiler.GlobalUsings.Select(name => new XElement("Using", new XAttribute("Include", name))));

    private static bool HasSources(string directory) =>
        Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Any(path => !path.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"));
}
