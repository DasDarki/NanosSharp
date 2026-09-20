namespace NanosSharp.Generator.Model;

internal sealed record PropertyModel(string Name, CSharpType Type, MethodModel Getter, MethodModel? Setter);
