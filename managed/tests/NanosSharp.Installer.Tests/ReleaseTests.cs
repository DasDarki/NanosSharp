using NanosSharp.Installer.Releases;

namespace NanosSharp.Installer.Tests;

public sealed class ReleaseTests : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nanossharp-installer-").FullName;

    [Fact]
    public void ChecksumsAreParsed()
    {
        var checksums = Checksums.Parse("""
            48DD5FC8  nanossharp-0.2.0-linux-x64.tar.gz
            e69d839b *nanossharp-client-0.2.0-win-x64.zip

            """);

        Assert.Equal("48dd5fc8", checksums["nanossharp-0.2.0-linux-x64.tar.gz"]);
        Assert.Equal("e69d839b", checksums["nanossharp-client-0.2.0-win-x64.zip"]);
    }

    [Fact]
    public void ChecksumOfAFileIsVerified()
    {
        var file = Path.Combine(_directory, "data.txt");
        File.WriteAllText(file, "nanos");

        Assert.True(Checksums.Matches(file, "f37f0907ed8dcaaaaf1bf159f852f2457505f053f38fc3ca3a435bee2f14bd48"));
        Assert.False(Checksums.Matches(file, "00"));
    }

    [Theory]
    [InlineData("nanossharp-0.2.0-linux-x64.tar.gz", "0.2.0")]
    [InlineData("nanossharp-client-0.2.0-beta.1-win-x64.zip", "0.2.0-beta.1")]
    public void VersionIsReadFromLocalFiles(string fileName, string version)
    {
        File.WriteAllText(Path.Combine(_directory, fileName), string.Empty);
        File.WriteAllText(Path.Combine(_directory, Release.ChecksumsAsset), string.Empty);

        var release = Release.FromDirectory(_directory);

        Assert.Equal(version, release.Version);
        Assert.Equal(Path.Combine(_directory, fileName), release.Assets[fileName]);
    }

    public void Dispose() => Directory.Delete(_directory, true);
}
