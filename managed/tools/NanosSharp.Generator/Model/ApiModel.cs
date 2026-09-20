namespace NanosSharp.Generator.Model;

internal sealed record ApiModel(IReadOnlyList<TypeModel> Types, IReadOnlyList<EnumModel> Enums);
