using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using NanosSharp.Host.Compilation.Sandbox;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Packages;

namespace NanosSharp.Host.Compilation;

internal static class ScriptCompiler
{
    public const string TargetFramework = "net10.0";

    public static readonly string[] GlobalUsings =
    [
        "System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Threading",
        "System.Threading.Tasks", "NanosSharp.Core", "NanosSharp.Entities", "NanosSharp.Enums", "NanosSharp.Eventing", "NanosSharp.Injection",
        "NanosSharp.Logging", "NanosSharp.Mathematics", "NanosSharp.Packages", "NanosSharp.Scripting", "NanosSharp.Threading", "NanosSharp.Diagnostics",
    ];

    private const string DynamicRuntimeAssembly = "Microsoft.CSharp.dll";

    private static readonly Lazy<MetadataReference[]> FrameworkReferences = new(() => LoadFrameworkReferences(true));

    private static readonly Lazy<MetadataReference[]> SandboxReferences = new(() => LoadFrameworkReferences(false));

#if CLIENT
    public const string SideSymbol = "CLIENT";
#else
    public const string SideSymbol = "SERVER";
#endif

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest, preprocessorSymbols: [SideSymbol]);

    private static readonly CSharpCompilationOptions CompilationOptions = new(
        OutputKind.DynamicallyLinkedLibrary,
        allowUnsafe: true,
        nullableContextOptions: NullableContextOptions.Enable,
        specificDiagnosticOptions: new Dictionary<string, ReportDiagnostic>
        {
            ["CS1701"] = ReportDiagnostic.Suppress,
            ["CS1702"] = ReportDiagnostic.Suppress,
        });

    public static CompilationResult Compile(string assemblyName, IReadOnlyList<string> sourceFiles,
        IEnumerable<AssemblyImage> referenceImages, bool optimize, ApiAllowList? allowList = null)
    {
        var globalUsings = string.Concat(GlobalUsings.Select(name => $"global using {name};\n"));
        var syntaxTrees = sourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(file), Encoding.UTF8),
                ParseOptions, file))
            .Append(CSharpSyntaxTree.ParseText(SourceText.From(globalUsings, Encoding.UTF8), ParseOptions, "GlobalUsings.g.cs"));

        var frameworkReferences = allowList is null ? FrameworkReferences.Value : SandboxReferences.Value;
        var references = frameworkReferences
            .Concat(referenceImages.Select(reference => MetadataReference.CreateFromImage(reference.Image)));
        var options = CompilationOptions
            .WithOptimizationLevel(optimize ? OptimizationLevel.Release : OptimizationLevel.Debug)
            .WithAllowUnsafe(allowList is null);
        var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, options);
        if (allowList is not null && SandboxVerifier.VerifySource(compilation) is { Count: > 0 } sourceViolations)
        {
            return new CompilationResult(null, null, sourceViolations.ToArray());
        }

        using var assemblyStream = new MemoryStream();
        using var symbolStream = new MemoryStream();
        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
        var result = compilation.Emit(assemblyStream, symbolStream, options: emitOptions);
        var diagnostics = result.Diagnostics
            .Where(diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning)
            .Select(diagnostic => diagnostic.ToString())
            .ToArray();

        if (!result.Success)
        {
            return new CompilationResult(null, null, diagnostics);
        }

        var image = assemblyStream.ToArray();
        if (allowList is not null && SandboxVerifier.VerifyAssembly(image, allowList) is { Count: > 0 } violations)
        {
            return new CompilationResult(null, null, diagnostics.Concat(violations).ToArray());
        }

        return new CompilationResult(image, symbolStream.ToArray(), diagnostics);
    }

    private static MetadataReference[] LoadFrameworkReferences(bool includeDynamic)
    {
        var trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(path => Path.GetFileName(path).StartsWith("System.", StringComparison.Ordinal) ||
                           Path.GetFileName(path) is "mscorlib.dll" or "netstandard.dll" ||
                           (includeDynamic && Path.GetFileName(path) == DynamicRuntimeAssembly));
        return trustedAssemblies
            .Append(typeof(Package).Assembly.Location)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
    }
}
