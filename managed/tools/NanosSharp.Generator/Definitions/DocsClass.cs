namespace NanosSharp.Generator.Definitions;

internal sealed class DocsClass
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsBase { get; set; }

    public List<string> Inheritance { get; set; } = [];

    public List<DocsFunction> Constructors { get; set; } = [];

    public List<DocsFunction> Functions { get; set; } = [];

    public List<DocsFunction> StaticFunctions { get; set; } = [];

    public List<DocsEvent> Events { get; set; } = [];
}
