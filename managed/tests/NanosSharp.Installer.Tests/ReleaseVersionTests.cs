using NanosSharp.Updater;

namespace NanosSharp.Installer.Tests;

public sealed class ReleaseVersionTests
{
    [Theory]
    [InlineData("0.2.1", "0.2.0", true)]
    [InlineData("0.3.0", "0.2.9", true)]
    [InlineData("1.0.0", "0.9.9", true)]
    [InlineData("0.2.0", "0.2.0", false)]
    [InlineData("0.2.0", "0.2.1", false)]
    [InlineData("0.2.0", "0.2.0-beta.1", true)]
    [InlineData("0.2.0-beta.1", "0.2.0", false)]
    [InlineData("0.2.0-beta.2", "0.2.0-beta.1", true)]
    [InlineData("v0.2.1", "0.2.0", true)]
    public void IsNewerCompares(string candidate, string current, bool expected) =>
        Assert.Equal(expected, ReleaseVersion.IsNewer(candidate, current));
}
