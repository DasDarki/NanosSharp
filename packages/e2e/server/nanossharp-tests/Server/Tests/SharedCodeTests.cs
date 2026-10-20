using NanosSharp.Tests.Framework;
using NanosSharp.Tests.Shared;

namespace NanosSharp.Tests.Tests;

public static class SharedCodeTests
{
    [Test]
    public static void SharedSourcesAreCompiledForTheServer()
    {
        Assert.Equal("server", SideInfo.Name);
    }
}
