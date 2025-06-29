using System.Collections;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NanosSharp.API;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The package compiler utilizes Roslyn to compile packages at runtime.
/// </summary>
internal static class PackageCompiler
{
    //TODO: implement a way to resolve package dependencies through csproj files or all DLLs in the package directory.
    public static Assembly? Compile(AssemblyLoadContext context, string packagePath, string packageName, string[] sourceFiles, bool emitDebugSymbols = false)
    {
        var syntaxTrees = sourceFiles.Select(file =>
        {
            var code = IOFile.ReadAllText(file, Encoding.UTF8);
            return CSharpSyntaxTree.ParseText(code, path: file, encoding: Encoding.UTF8);
        }).ToList();
        
        var compilation = CSharpCompilation.Create(
            assemblyName: packageName,
            syntaxTrees: syntaxTrees,
            references: GetRequiredReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var dllStream = new MemoryStream();
        using var pdbStream = emitDebugSymbols ? new MemoryStream() : null;

        var result = compilation.Emit(
            peStream: dllStream,
            pdbStream: pdbStream,
            options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb)
        );

        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                Console.Error.WriteLine(diagnostic.ToString());
            }

            return null;
        }

        dllStream.Seek(0, SeekOrigin.Begin);
        pdbStream?.Seek(0, SeekOrigin.Begin);

        return emitDebugSymbols && pdbStream != null
            ? context.LoadFromStream(dllStream, pdbStream)
            : context.LoadFromStream(dllStream);
    }
    
    private static List<MetadataReference> GetRequiredReferences()
    {
        var references = new List<MetadataReference>
        {
            GetRefMetaData<ConsoleKey>(),
            GetRefMetaData<HttpClient>(),
            GetRefMetaData<IQueryable>(),
            GetRefMetaData<IDictionary>(),
            GetRefMetaData<Serilog.Core.Logger>(),
            GetRefMetaData<ILuaVM>(),
            GetRefMetaData<ServerModule>(),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        };

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            {
                try
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to add reference for assembly '{assembly.GetName().Name}': {ex.Message}");
                }
            }
        }

        return references;
    }
    
    private static MetadataReference GetRefMetaData<T>()
    {
        var type = typeof(T);
        var loc = type.Assembly.Location;
        return MetadataReference.CreateFromFile(loc);
    }
}