using System.Xml.Linq;
using NanosSharp.Host.Compilation;
using NanosSharp.Packages;

namespace NanosSharp.Host.Packages.Dependencies;

internal static class ProjectGenerator
{
    public static string Generate(string packageName, string sideDirectory)
    {
        var path = Path.Combine(sideDirectory, $"{packageName}.csproj");
        var apiPath = Path.GetRelativePath(sideDirectory, typeof(Package).Assembly.Location);
        var project = new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",
                new XElement("TargetFramework", ScriptCompiler.TargetFramework),
                new XElement("LangVersion", "latest"),
                new XElement("Nullable", "enable"),
                new XElement("AllowUnsafeBlocks", "true"),
                new XElement("ImplicitUsings", "disable"),
                new XElement("AssemblyName", $"{packageName}.Scripts"),
                new XElement("DefineConstants", $"$(DefineConstants);{ScriptCompiler.SideSymbol}")),
            new XElement("ItemGroup",
                new XElement("Reference",
                    new XAttribute("Include", "NanosSharp"),
                    new XElement("HintPath", apiPath),
                    new XElement("Private", "false"))),
            new XElement("ItemGroup",
                new XElement("Compile",
                    new XAttribute("Include", "../Shared/**/*.cs"),
                    new XAttribute("Link", "Shared/%(RecursiveDir)%(Filename)%(Extension)")),
                new XElement("None",
                    new XAttribute("Include", "../Package.toml"),
                    new XAttribute("Link", "Package.toml"))),
            new XElement("ItemGroup", ScriptCompiler.GlobalUsings.Select(name => new XElement("Using", new XAttribute("Include", name)))));

        new XDocument(project).Save(path);
        return path;
    }
}
