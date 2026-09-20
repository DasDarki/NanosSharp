using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Packages;

namespace NanosSharp.Host.Compilation;

internal static class ScriptCompiler
{
    public const string TargetFramework = "net10.0";

    public static readonly string[] GlobalUsings =
    [
        "System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Threading",
        "System.Threading.Tasks", "NanosSharp.Core", "NanosSharp.Entities", "NanosSharp.Enums", "NanosSharp.Logging",
        "NanosSharp.Mathematics", "NanosSharp.Packages", "NanosSharp.Scripting", "NanosSharp.Threading",
    ];

    private static readonly Lazy<MetadataReference[]> FrameworkReferences = new(LoadFrameworkReferences);

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest);

    private static readonly CSharpCompilationOptions CompilationOptions = new(
        OutputKind.DynamicallyLinkedLibrary,
        optimizationLevel: OptimizationLevel.Debug,
        allowUnsafe: true,
        nullableContextOptions: NullableContextOptions.Enable,
        specificDiagnosticOptions: new Dictionary<string, ReportDiagnostic>
        {
            ["CS1701"] = ReportDiagnostic.Suppress,
            ["CS1702"] = ReportDiagnostic.Suppress,
        });

    public static CompilationResult Compile(string assemblyName, IReadOnlyList<string> sourceFiles,
        IEnumerable<AssemblyImage> referenceImages)
    {
        var globalUsings = string.Concat(GlobalUsings.Select(name => $"global using {name};\n"));
        var syntaxTrees = sourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(file), Encoding.UTF8),
                ParseOptions, file))
            .Append(CSharpSyntaxTree.ParseText(SourceText.From(globalUsings, Encoding.UTF8), ParseOptions, "GlobalUsings.g.cs"));

        var references = FrameworkReferences.Value
            .Concat(referenceImages.Select(reference => MetadataReference.CreateFromImage(reference.Image)));
        var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, CompilationOptions);

        using var assemblyStream = new MemoryStream();
        using var symbolStream = new MemoryStream();
        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
        var result = compilation.Emit(assemblyStream, symbolStream, options: emitOptions);
        var diagnostics = result.Diagnostics
            .Where(diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning)
            .Select(diagnostic => diagnostic.ToString())
            .ToArray();

        return result.Success
            ? new CompilationResult(assemblyStream.ToArray(), symbolStream.ToArray(), diagnostics)
            : new CompilationResult(null, null, diagnostics);
    }

    private static MetadataReference[] LoadFrameworkReferences()
    {
        var trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(path => Path.GetFileName(path).StartsWith("System.", StringComparison.Ordinal) ||
                           Path.GetFileName(path) is "mscorlib.dll" or "netstandard.dll" or "Microsoft.CSharp.dll");
        return trustedAssemblies
            .Append(typeof(Package).Assembly.Location)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
    }
}
