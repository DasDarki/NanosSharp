using System.Net;
using System.Net.Sockets;
using System.Text;
using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class AsyncTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Test]
    public static async Task HttpRequestCompletesOnMainThread()
    {
        var port = FindFreePort();
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();
        var serving = Task.Run(() => ServeOnce(listener));

        var response = await WithTimeout(HTTP.RequestAsync($"http://127.0.0.1:{port}", "/hello", HTTPMethod.POST, "ping"));
        await serving;

        Assert.True(MainThread.IsCurrent, "Continuation is not on the main thread");
        Assert.Equal(200, response.Status);
        Assert.True(response.IsSuccess, "Response is not successful");
        Assert.Equal("POST /hello ping", response.Data);
        Assert.True(response.Headers.Any(header => header.Key.Equals("X-NanosSharp", StringComparison.OrdinalIgnoreCase)),
            $"Headers are [{string.Join(", ", response.Headers.Keys)}]");
    }

    [Test]
    public static async Task DatabaseQueriesRunAsynchronously()
    {
        var database = new Database(DatabaseEngine.SQLite, "db=nanossharp-async-test.db", 1);
        try
        {
            await WithTimeout(database.ExecuteAsync("DROP TABLE IF EXISTS items"));
            await WithTimeout(database.ExecuteAsync("CREATE TABLE items (id INTEGER PRIMARY KEY, name TEXT)"));
            var inserted = await WithTimeout(database.ExecuteAsync("INSERT INTO items (name) VALUES (?), (?)", "first", "second"));
            var rows = await WithTimeout(database.SelectAsync("SELECT name FROM items WHERE name = ?", "second"));

            Assert.True(MainThread.IsCurrent, "Continuation is not on the main thread");
            Assert.Equal(2L, inserted);
            Assert.Equal(1, rows.Count);
            Assert.Equal("second", rows[0]["name"] as string);
        }
        finally
        {
            database.Close();
        }
    }

    [Test]
    public static async Task DatabaseErrorsAreThrown()
    {
        var database = new Database(DatabaseEngine.SQLite, "db=nanossharp-async-test.db", 1);
        try
        {
            var failed = false;
            try
            {
                await WithTimeout(database.SelectAsync("SELECT * FROM table_that_does_not_exist"));
            }
            catch (DatabaseException)
            {
                failed = true;
            }

            Assert.True(failed, "Invalid query did not throw a DatabaseException");
        }
        finally
        {
            database.Close();
        }
    }

    [Test]
    public static async Task FileIsReadAsynchronously()
    {
        const string path = "nanossharp-async-test.txt";
        var writer = new ScriptFile(path, true);
        writer.Write("hello from a file");
        writer.Close();

        var reader = new ScriptFile(path);
        var content = await WithTimeout(reader.ReadAsync());
        reader.Close();

        Assert.True(MainThread.IsCurrent, "Continuation is not on the main thread");
        Assert.Equal("hello from a file", content);
    }

    private static async Task<T> WithTimeout<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Scheduler.Delay(Timeout));
        return completed == task ? await task : throw new TimeoutException("The asynchronous operation did not complete.");
    }

    private static async Task ServeOnce(HttpListener listener)
    {
        var context = await listener.GetContextAsync();
        using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var payload = Encoding.UTF8.GetBytes($"{context.Request.HttpMethod} {context.Request.Url!.AbsolutePath} {body}");
        context.Response.StatusCode = 200;
        context.Response.AddHeader("X-NanosSharp", "yes");
        await context.Response.OutputStream.WriteAsync(payload);
        context.Response.Close();
    }

    private static int FindFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
