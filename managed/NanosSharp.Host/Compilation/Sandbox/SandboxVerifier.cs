using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NanosSharp.Host.Compilation.Sandbox;

internal static class SandboxVerifier
{
    private static readonly HashSet<string> CompilerOnlyTypes = new(StringComparer.Ordinal)
    {
        "System.Runtime.CompilerServices.Unsafe",
        "System.Runtime.CompilerServices.RuntimeHelpers",
        "System.Runtime.InteropServices.MemoryMarshal",
        "System.Runtime.InteropServices.CollectionsMarshal",
    };

    public static IReadOnlyList<string> VerifySource(CSharpCompilation compilation)
    {
        var violations = new List<string>();
        foreach (var tree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var name in tree.GetRoot().DescendantNodes().OfType<SimpleNameSyntax>())
            {
                var symbol = model.GetSymbolInfo(name).Symbol;
                var type = symbol as INamedTypeSymbol ?? symbol?.ContainingType;
                if (type is not null && CompilerOnlyTypes.Contains(FullName(type.OriginalDefinition)))
                {
                    violations.Add($"{name.GetLocation().GetLineSpan()}: {FullName(type.OriginalDefinition)} cannot be used by client packages.");
                }
            }
        }

        return violations;
    }

    public static IReadOnlyList<string> VerifyAssembly(byte[] image, ApiAllowList allowList)
    {
        using var peReader = new PEReader(ImmutableArray.Create(image));
        var reader = peReader.GetMetadataReader();
        var violations = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var handle in reader.TypeReferences)
        {
            var name = TypeName(reader, handle);
            if (!allowList.IsTypeAllowed(name))
            {
                violations.Add($"the type {name} cannot be used by client packages.");
            }
        }

        foreach (var handle in reader.MemberReferences)
        {
            var member = reader.GetMemberReference(handle);
            var owner = ParentName(reader, member.Parent);
            var name = reader.GetString(member.Name);
            if (owner is not null && !allowList.IsMemberAllowed(owner, name))
            {
                violations.Add($"{owner}::{name} cannot be used by client packages.");
            }
        }

        if (reader.GetTableRowCount(TableIndex.ModuleRef) > 0 || reader.GetTableRowCount(TableIndex.ImplMap) > 0)
        {
            violations.Add("native code (DllImport) cannot be used by client packages.");
        }

        return violations.ToArray();
    }

    private static string? ParentName(MetadataReader reader, EntityHandle parent) => parent.Kind switch
    {
        HandleKind.TypeReference => TypeName(reader, (TypeReferenceHandle)parent),
        HandleKind.TypeSpecification => SpecificationName(reader, (TypeSpecificationHandle)parent),
        _ => null,
    };

    private static string? SpecificationName(MetadataReader reader, TypeSpecificationHandle handle)
    {
        var blob = reader.GetBlobReader(reader.GetTypeSpecification(handle).Signature);
        var code = blob.ReadSignatureTypeCode();
        if (code is SignatureTypeCode.Array or SignatureTypeCode.SZArray)
        {
            return "System.Array";
        }

        if (code != SignatureTypeCode.GenericTypeInstance)
        {
            return code.ToString();
        }

        blob.ReadSignatureTypeCode();
        var definition = blob.ReadTypeHandle();
        return definition.Kind switch
        {
            HandleKind.TypeReference => TypeName(reader, (TypeReferenceHandle)definition),
            HandleKind.TypeDefinition => null,
            _ => definition.Kind.ToString(),
        };
    }

    private static string TypeName(MetadataReader reader, TypeReferenceHandle handle)
    {
        var reference = reader.GetTypeReference(handle);
        if (reference.ResolutionScope.Kind == HandleKind.TypeReference)
        {
            return TypeName(reader, (TypeReferenceHandle)reference.ResolutionScope);
        }

        return Combine(reader.GetString(reference.Namespace), StripArity(reader.GetString(reference.Name)));
    }

    private static string FullName(INamedTypeSymbol type) =>
        Combine(type.ContainingNamespace.IsGlobalNamespace ? string.Empty : type.ContainingNamespace.ToDisplayString(), type.MetadataName.Split('`')[0]);

    private static string Combine(string name, string type) => name.Length == 0 ? type : $"{name}.{type}";

    private static string StripArity(string name)
    {
        var index = name.IndexOf('`');
        return index < 0 ? name : name[..index];
    }
}
