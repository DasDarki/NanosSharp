using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ConsoleTests
{
    [Test]
    public static void RegisteredCommandReceivesArguments()
    {
        string[]? received = null;
        Commands.Register("nanossharp-test-command", arguments => received = arguments, "Test command");
        Commands.Run("nanossharp-test-command first second");

        Assert.True(received is not null, "Command handler was not invoked");
        Assert.Equal("first,second", string.Join(",", received!), "Command arguments");
    }
}
