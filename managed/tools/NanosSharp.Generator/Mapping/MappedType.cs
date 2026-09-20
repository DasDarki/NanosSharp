using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Mapping;

internal sealed record MappedType(CSharpType Type, bool IsOptional);
