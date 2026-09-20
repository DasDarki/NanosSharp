namespace NanosSharp.Generator.Definitions;

internal sealed class DocsEnum
{
    public string? Description { get; set; }

    public List<DocsEnumValue> Enums { get; set; } = [];
}
