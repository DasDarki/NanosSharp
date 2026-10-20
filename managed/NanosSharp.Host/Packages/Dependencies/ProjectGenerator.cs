using System.Xml.Linq;
using NanosSharp.Host.Compilation;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages.Dependencies;

internal static class ProjectGenerator
{
    private const string SharedDirectoryName = "Shared";

    public static string Generate(string packageName, string sideDirectory)
    {
        var packageDirectory = Directory.GetParent(sideDirectory)!.FullName;
        var sharedProject = EnsureSharedProject(packageName, packageDirectory);

        var items = new XElement("ItemGroup", ApiReference(sideDirectory));
        if (sharedProject is not null)
        {
            items.Add(new XElement("ProjectReference", new XAttribute("Include", Path.GetRelativePath(sideDirectory, sharedProject))));
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

    private static XElement GlobalUsings() =>
        new("ItemGroup", ScriptCompiler.GlobalUsings.Select(name => new XElement("Using", new XAttribute("Include", name))));

    private static bool HasSources(string directory) =>
        Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Any(path => !path.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"));
}
