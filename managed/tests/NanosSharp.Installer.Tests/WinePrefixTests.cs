using NanosSharp.Installer.Client;

namespace NanosSharp.Installer.Tests;

public sealed class WinePrefixTests
{
    private const string Header = @"[Software\\Wine\\DllOverrides] 1791146050";

    [Fact]
    public void OverrideIsAddedAfterTheTimestamp()
    {
        List<string> lines = [Header, "#time=1dd543fb5f94ffc", "\"d3d11\"=\"native\"", "", "[Other] 1"];

        WinePrefix.ApplyOverride(lines, "version", WinePrefix.NativeThenBuiltin);

        Assert.Equal([Header, "#time=1dd543fb5f94ffc", "\"version\"=\"native,builtin\"", "\"d3d11\"=\"native\"", "", "[Other] 1"], lines);
    }

    [Fact]
    public void ExistingOverrideIsReplaced()
    {
        List<string> lines = [Header, "\"version\"=\"builtin\"", ""];

        WinePrefix.ApplyOverride(lines, "version", WinePrefix.NativeThenBuiltin);

        Assert.Equal([Header, "\"version\"=\"native,builtin\"", ""], lines);
    }

    [Fact]
    public void OverrideIsRemoved()
    {
        List<string> lines = [Header, "\"version\"=\"native,builtin\"", "\"d3d11\"=\"native\"", ""];

        WinePrefix.ApplyOverride(lines, "version", null);

        Assert.Equal([Header, "\"d3d11\"=\"native\"", ""], lines);
    }

    [Fact]
    public void MissingSectionIsCreated()
    {
        List<string> lines = ["WINE REGISTRY Version 2", "", "[Other] 1"];

        WinePrefix.ApplyOverride(lines, "version", WinePrefix.NativeThenBuiltin);

        Assert.StartsWith(@"[Software\\Wine\\DllOverrides] ", lines[^2]);
        Assert.Equal("\"version\"=\"native,builtin\"", lines[^1]);
    }

    [Fact]
    public void OverridesOfOtherSectionsAreKept()
    {
        List<string> lines = [Header, "", "[Other] 1", "\"version\"=\"keep\""];

        WinePrefix.ApplyOverride(lines, "version", null);

        Assert.Equal([Header, "", "[Other] 1", "\"version\"=\"keep\""], lines);
    }
}
