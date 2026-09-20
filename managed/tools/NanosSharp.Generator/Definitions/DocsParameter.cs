using System.Text.Json;

namespace NanosSharp.Generator.Definitions;

internal sealed class DocsParameter
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    public JsonElement? Default { get; set; }
}
