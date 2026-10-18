using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Definitions;

internal sealed class DocsFunction
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Authority { get; set; } = "both";

    public List<DocsParameter> Parameters { get; set; } = [];

    public List<DocsReturn> Return { get; set; } = [];

    public bool IsAvailableOn(ApiSide side) =>
        Authority is "both" or "authority" or "network-authority" || Authority == (side == ApiSide.Server ? "server" : "client");
}
