using NanosSharp.Diagnostics;
using NanosSharp.Host.Packages;

namespace NanosSharp.Host.Tests;

public sealed class DebugTests : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nanossharp-debug-").FullName;

    [Fact]
    public void BreakIsANoOpWhenDebuggingIsOff()
    {
        Assert.False(Debugger.IsEnabled);
        Debugger.Break();
    }

    [Fact]
    public void MarkerRoundTripsWithoutDebug()
    {
        File.WriteAllText(Path.Combine(CreateClientDirectory(), "Probe.cs"), "// probe");

        PackageMarker.Write(0, _directory);
        var marker = PackageMarker.Read(_directory);

        Assert.Null(marker.Incompatibility);
        Assert.False(marker.Debug);
    }

    [Fact]
    public void MissingMarkerReportsNoServerRuntime()
    {
        File.WriteAllText(Path.Combine(CreateClientDirectory(), "Probe.cs"), "// probe");

        var marker = PackageMarker.Read(_directory);

        Assert.NotNull(marker.Incompatibility);
        Assert.False(marker.Debug);
    }

    private string CreateClientDirectory()
    {
        var client = Path.Combine(_directory, "Client");
        Directory.CreateDirectory(client);
        return client;
    }

    public void Dispose() => Directory.Delete(_directory, true);
}
