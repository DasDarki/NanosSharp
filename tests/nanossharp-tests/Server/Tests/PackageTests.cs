using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class PackageTests
{
    [Test]
    public static void PackageInformationComesFromNanosWorld()
    {
        var package = TestRunner.Instance;
        Assert.Equal("nanossharp-tests", package.Name);
        Assert.Equal("NanosSharp Tests", package.Title);
        Assert.Equal("1.0.0", package.Version);
        Assert.Equal(PackageType.Script, package.PackageType);
        Assert.True(!package.IsUnloading, "Package reports unloading");
    }

    [Test]
    public static void PackageFilesAreListed()
    {
        var files = TestRunner.Instance.GetFiles("Server", ".cs");
        Assert.True(files.Any(file => file.EndsWith("PackageTests.cs", StringComparison.Ordinal)), $"Files: {string.Join(", ", files.Take(5))}");
    }

    [Test]
    public static void PersistentDataRoundTrips()
    {
        var package = TestRunner.Instance;
        var value = Guid.NewGuid().ToString();
        package.SetPersistentData("nanossharp-test", value);
        Assert.Equal(value, package.GetPersistentData<string>("nanossharp-test"));
    }
}
