using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NanosSharp.Client.API;

namespace NanosSharp.Client;

/// <summary>
/// The package loader is the heart of the NanosSharp client runtime. It loads all package files and compiles them
/// JIT while applying some security measures like only allowing specific namespaces and types.
/// </summary>
internal sealed class PackageLoader
{
    private static readonly IList<string> AllowedNamespaces = ["System", "NanosSharp.Client.API*"];
    private static readonly HashSet<string> AllowedClasses = ["System.Console", "System.Math", "System.Random"];
    private readonly Assembly _nanosSharpApiAssembly;
    
    internal PackageLoader()
    {
        _nanosSharpApiAssembly = Assembly.GetAssembly(typeof(Package)) ?? throw new InvalidOperationException("NanosSharp.Client.API assembly not found.");
    }

    /// <summary>
    /// Compiles the given package directory into an assembly.
    /// </summary>
    /// <param name="name">The name of the package.</param>
    /// <param name="dir">The directory containing the package files.</param>
    /// <returns>The compiled package assembly, or null if the package could not be compiled.</returns>
    internal Assembly? CompilePackage(string name, string dir)
    {
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var file in Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories))
        {
            var code = File.ReadAllText(file);
            if (!ValidateCode(code))
            {
                Runtime.Logger.Fatal("Invalid code in package {Name}; file {File}", name, file);
                return null;
            }

            syntaxTrees.Add(CSharpSyntaxTree.ParseText(code));
        }
        
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();
        
        references.Add(MetadataReference.CreateFromFile(_nanosSharpApiAssembly.Location));

        var compilation = CSharpCompilation.Create(
            "PackageAssembly_" + name,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = string.Join(Environment.NewLine,
                result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

            Runtime.Logger.Error("Failed to compile package {Name}:\n{Errors}", name, errors);
            return null;
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    private bool ValidateCode(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();

        // Validate namespace declarations
        var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
        if (namespaceDeclarations.Any(ns => !IsNamespaceAllowed(ns.Name.ToString())))
        {
            return false;
        }

        // Validate namespace aliases
        var namespaceAliases = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
            .Where(u => u.Alias != null);

        if (namespaceAliases.Any())
            return false;

        // Validate object creation (class usage)
        var objectCreations = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
        
        return objectCreations.Select(objCreation => objCreation.Type.ToString()).All(IsClassAllowed);
    }

    private bool IsNamespaceAllowed(string namespaceName)
    {
        foreach (var allowedNamespace in AllowedNamespaces)
        {
            if (allowedNamespace.EndsWith("*"))
            {
                var baseNamespace = allowedNamespace.TrimEnd('*');
                if (namespaceName.StartsWith(baseNamespace))
                    return true;
            }
            else if (namespaceName == allowedNamespace)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsClassAllowed(string typeName)
    {
        if (AllowedClasses.Contains(typeName))
            return true;

        return (from allowedNamespace in AllowedNamespaces
            where allowedNamespace.EndsWith("*")
            select allowedNamespace.TrimEnd('*')).Any(typeName.StartsWith);
    }
}