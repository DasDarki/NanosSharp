using System.Net;
using System.Net.Http;
using System.Text;
using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ExtendedApiTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Test]
    public static async Task HttpServerRouteAnswersRequests()
    {
        using var route = HTTPServer.Route(HTTPMethod.POST, "/nanossharp-test", request =>
            new HttpServerResponse(201, $"{request.Body}:{request.Query["name"]}", "text/plain",
                new Dictionary<string, string> { ["X-NanosSharp"] = request.Path }));

        var url = $"http://127.0.0.1:{Server.Port}/scripting/nanossharp-test?name=route";
        var response = await WithTimeout(Task.Run(() => SendAsync(url)));

        Assert.Equal(HttpStatusCode.Created, response.Status);
        Assert.Equal("ping:route", response.Body);
        Assert.Equal("/nanossharp-test", response.Header);
    }

    [Test]
    public static async Task HttpServerRouteFailureReturnsServerError()
    {
        using var route = HTTPServer.Route(HTTPMethod.POST, "/nanossharp-failure",
            (Func<HttpServerRequest, HttpServerResponse>)(_ => throw new InvalidOperationException("expected test failure")));

        var url = $"http://127.0.0.1:{Server.Port}/scripting/nanossharp-failure";
        var response = await WithTimeout(Task.Run(() => SendAsync(url)));

        Assert.Equal(HttpStatusCode.InternalServerError, response.Status);
    }

    [Test]
    public static async Task WebSocketExchangesMessages()
    {
        var port = FreePort();
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();
        var serving = Task.Run(() => EchoOnce(listener));

        var received = new TaskCompletionSource<string>();
        var closed = new TaskCompletionSource();
        var socket = new WebSocket($"ws://127.0.0.1:{port}/");
        socket.SubscribeReceiveMessage((_, message) => received.TrySetResult(message ?? string.Empty));
        socket.SubscribeClose(_ => closed.TrySetResult());

        await WithTimeout(WaitUntil(() => socket.Send("hello")));
        var message = await WithTimeout(received.Task);
        socket.Close();
        await WithTimeout(closed.Task);
        await WithTimeout(serving);

        Assert.Equal("echo:hello", message);
    }

    [Test]
    public static void JsonAndTomlRoundTrip()
    {
        var json = JSON.Stringify(new Dictionary<string, object?> { ["name"] = "nanos", ["count"] = 3 });
        var parsed = JSON.Parse(json) as Dictionary<object, object?>;
        Assert.True(parsed is not null, $"JSON {json} was not parsed into a table");
        Assert.Equal("nanos", parsed!["name"] as string);

        var toml = TOML.Parse("title = \"server\"\nslots = 32") as Dictionary<object, object?>;
        Assert.True(toml is not null, "TOML was not parsed into a table");
        Assert.Equal(32L, Convert.ToInt64(toml!["slots"]));
        Assert.True(TOML.Dump(toml).Contains("title"), "TOML dump does not contain the title");
    }

    [Test]
    public static void CharacterGaspSpawns()
    {
        var character = new CharacterGASP(new Vector(0, 0, 100), Rotator.Zero, "nanos-world::SK_Mannequin");
        try
        {
            Assert.True(character.IsValid, "CharacterGASP is not valid");
            Assert.Equal("CharacterGASP", character.ClassName);
            Assert.True(character.GetHealth() > 0, "CharacterGASP has no health");
        }
        finally
        {
            character.Destroy();
        }
    }

    private static async Task<(HttpStatusCode Status, string Body, string? Header)> SendAsync(string url)
    {
        using var client = new HttpClient();
        using var response = await client.PostAsync(url, new StringContent("ping"));
        var header = response.Headers.TryGetValues("X-NanosSharp", out var values) ? values.FirstOrDefault() : null;
        return (response.StatusCode, await response.Content.ReadAsStringAsync(), header);
    }

    private static async Task EchoOnce(HttpListener listener)
    {
        var context = await listener.GetContextAsync();
        var webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
        var buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        var reply = Encoding.UTF8.GetBytes("echo:" + Encoding.UTF8.GetString(buffer, 0, result.Count));
        await webSocket.SendAsync(reply, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
        await webSocket.ReceiveAsync(buffer, CancellationToken.None);
    }

    private static async Task<bool> WaitUntil(Func<bool> condition)
    {
        while (!condition())
        {
            await Scheduler.Delay(50);
        }

        return true;
    }

    private static async Task<T> WithTimeout<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Scheduler.Delay(Timeout));
        return completed == task ? await task : throw new TimeoutException("The operation did not complete.");
    }

    private static async Task WithTimeout(Task task)
    {
        var completed = await Task.WhenAny(task, Scheduler.Delay(Timeout));
        await (completed == task ? task : throw new TimeoutException("The operation did not complete."));
    }

    private static int FreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
