namespace NanosSharp.Generator.Definitions;

internal sealed class DocsFunction
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Authority { get; set; } = "both";

    public List<DocsParameter> Parameters { get; set; } = [];

    public List<DocsReturn> Return { get; set; } = [];

    public bool IsServerSide => Authority is "server" or "both" or "authority" or "network-authority";
}
