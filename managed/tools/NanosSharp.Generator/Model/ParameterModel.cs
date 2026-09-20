namespace NanosSharp.Generator.Model;

internal sealed record ParameterModel(string Name, CSharpType Type, bool IsOptional, string? Description);
