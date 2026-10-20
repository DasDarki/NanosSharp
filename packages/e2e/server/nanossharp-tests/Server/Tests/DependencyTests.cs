using System.Runtime.Loader;
using Microsoft.Data.Sqlite;
using NanosSharp.Tests.Framework;
using Tomlyn;

namespace NanosSharp.Tests.Tests;

public static class DependencyTests
{
    [Test]
    public static void NuGetPackageIsIsolatedFromHost()
    {
        var assembly = typeof(Toml).Assembly;
        Assert.Equal(0, assembly.GetName().Version!.Major, "Tomlyn major version");
        Assert.Equal("NanosSharp:nanossharp-tests", AssemblyLoadContext.GetLoadContext(assembly)?.Name);

        var model = Toml.ToModel("answer = 42");
        Assert.Equal(42L, model["answer"]);
    }

    [Test]
    public static void NativeLibraryFromNuGetLoads()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 40 + 2";
        Assert.Equal(42L, command.ExecuteScalar());
    }
}
