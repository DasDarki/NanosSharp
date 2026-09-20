namespace NanosSharp.Generator.Model;

internal sealed record MethodModel(
    string Name,
    CSharpType ReturnType,
    IReadOnlyList<ParameterModel> Parameters,
    bool IsStatic,
    string? Description)
{
    public int RequiredParameterCount => Parameters.TakeWhile(parameter => !parameter.IsOptional && parameter.Type.Kind != CSharpTypeKind.Variadic).Count();
}
