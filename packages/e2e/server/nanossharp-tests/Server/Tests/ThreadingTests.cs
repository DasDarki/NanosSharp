using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ThreadingTests
{
    [Test]
    public static async Task AwaitContinuesOnMainThread()
    {
        await Task.Delay(50);
        Assert.True(MainThread.IsCurrent, "Continuation is not on the main thread");
    }

    [Test]
    public static async Task BackgroundThreadIsRejected()
    {
        var exception = await Task.Run(() => Record(() => _ = Server.Name));
        Assert.True(exception is InvalidOperationException, $"Expected InvalidOperationException, got {exception?.GetType().Name ?? "nothing"}");
    }

    [Test]
    public static async Task InvokeAsyncRunsOnMainThread()
    {
        var name = await Task.Run(() => MainThread.InvokeAsync(() => Server.Name));
        Assert.Equal(Server.Name, name);
    }

    private static Exception? Record(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}
