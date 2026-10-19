using NanosSharp.Installer.Cli;

namespace NanosSharp.Installer.Tests;

public sealed class OptionsParserTests
{
    [Fact]
    public void ArgumentsAreParsed()
    {
        var options = OptionsParser.Parse(["server", "--path", "/srv/nanos", "--version", "v0.2.0", "--yes", "--force"]);

        Assert.Equal(InstallTarget.Server, options.Target);
        Assert.Equal("/srv/nanos", options.Path);
        Assert.Equal("0.2.0", options.Version);
        Assert.True(options.AssumeYes);
        Assert.True(options.Force);
        Assert.False(options.Uninstall);
    }

    [Fact]
    public void UnknownArgumentsAreRejected() => Assert.Throws<ArgumentException>(() => OptionsParser.Parse(["--unknown"]));

    [Fact]
    public void MissingValuesAreRejected() => Assert.Throws<ArgumentException>(() => OptionsParser.Parse(["client", "--path"]));
}
