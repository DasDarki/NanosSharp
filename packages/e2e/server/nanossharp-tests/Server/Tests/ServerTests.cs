using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ServerTests
{
    [Test]
    public static void ServerInformationIsAvailable()
    {
        Assert.True(Server.Name.Length > 0, "Server name is empty");
        Assert.True(Server.Version.Length > 0, "Server version is empty");
        Assert.True(Server.TickRate > 0, "Tick rate is not positive");
    }

    [Test]
    public static void TestPackageIsListedAsOwnPackage()
    {
        var names = Server.GetPackages().Select(package => package["name"]).ToArray();
        Assert.True(names.Contains("nanossharp-tests"), $"Package list is [{string.Join(", ", names)}]");
    }

    [Test]
    public static void UnknownClassThrows()
    {
        Assert.Throws<ScriptException>(() => ScriptApi.Invoke("DoesNotExist", "Anything"));
    }
}
