namespace NanosSharp.Generator.Model;

internal sealed class TypeModel
{
    public required string Name { get; init; }

    public required TypeKind Kind { get; init; }

    public required string Namespace { get; init; }

    public string? ScriptClassName { get; init; }

    public bool IsPrimary { get; init; } = true;

    public string? BaseType { get; init; }

    public IReadOnlyList<string> Interfaces { get; set; } = [];

    public string? Description { get; init; }

    public ConstructorModel? Constructor { get; set; }

    public List<MethodModel> Methods { get; } = [];

    public List<PropertyModel> Properties { get; } = [];

    public List<EventModel> Events { get; } = [];
}
