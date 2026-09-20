namespace NanosSharp.Generator.Model;

internal sealed record EnumModel(string Name, string? Description, IReadOnlyList<EnumValueModel> Values, bool IsFlags);
