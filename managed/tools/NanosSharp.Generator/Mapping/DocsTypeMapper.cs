using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Mapping;

internal sealed class DocsTypeMapper(
    IReadOnlySet<string> generatedTypes,
    IReadOnlySet<string> enumNames,
    IReadOnlyDictionary<string, string> renamedTypes)
{
    private static readonly Dictionary<string, string> PrimitiveTypes = new(StringComparer.Ordinal)
    {
        ["boolean"] = "bool",
        ["integer"] = "int",
        ["float"] = "float",
        ["number"] = "double",
        ["string"] = "string",
        ["text"] = "string",
        ["Vector"] = "Vector",
        ["Rotator"] = "Rotator",
        ["Color"] = "Color",
        ["Vector2D"] = "Vector2D",
        ["any"] = "object?",
        ["table"] = "object?",
    };

    public CSharpType Map(string docsType)
    {
        var type = docsType.Split('|')[0].Trim().TrimEnd('?');
        if (type.EndsWith("[]", StringComparison.Ordinal))
        {
            return new CSharpType($"IEnumerable<{Map(type[..^2])}>");
        }

        if (type == "function")
        {
            return CSharpType.Callback;
        }

        if (PrimitiveTypes.TryGetValue(type, out var primitive))
        {
            return new CSharpType(primitive);
        }

        if (type.EndsWith("Path", StringComparison.Ordinal))
        {
            return new CSharpType("string");
        }

        if (enumNames.Contains(type) || generatedTypes.Contains(type))
        {
            return new CSharpType(renamedTypes.GetValueOrDefault(type, type));
        }

        return new CSharpType("Entity");
    }

    public bool IsEntityType(string name) => generatedTypes.Contains(name) || renamedTypes.Values.Contains(name);

    public string? FindEnum(string docsType)
    {
        var candidate = docsType.Split('|')[0].Trim().TrimEnd('?');
        return enumNames.Contains(candidate) ? candidate : null;
    }
}
