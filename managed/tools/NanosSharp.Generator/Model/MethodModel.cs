namespace NanosSharp.Generator.Model;

internal sealed record MethodModel(
    string Name,
    CSharpType ReturnType,
    IReadOnlyList<ParameterModel> Parameters,
    bool IsStatic,
    string? Description)
{
    public string CSharpName => char.IsLower(Name[0]) ? $"{char.ToUpperInvariant(Name[0])}{Name[1..]}" : Name;

    public int RequiredParameterCount => Parameters.TakeWhile(parameter => !parameter.IsOptional && parameter.Type.Kind != CSharpTypeKind.Variadic).Count();
}
