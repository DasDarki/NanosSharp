namespace NanosSharp.Generator.Model;

internal sealed record ConstructorModel(IReadOnlyList<ParameterModel> Parameters, string? Description)
{
    public int RequiredParameterCount => Parameters.TakeWhile(parameter => !parameter.IsOptional).Count();
}
