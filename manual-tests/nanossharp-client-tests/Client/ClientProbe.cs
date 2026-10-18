public sealed class ClientProbe : Package
{
    private const string Prefix = "[CLIENT-TEST]";

    private int _interval;
    private int _ticks;
    private bool _keyLogged;
    private TextRender? _text;

    protected override async void OnStart()
    {
        Log.Info($"client probe started with {SharedProbe.Message}");
        Check("PackageInfo", () => $"{Title} {Version}");
        Check("ClientState", () => $"map '{Client.Map}', language {Client.Language}, viewport {Viewport.ViewportSize}, frame time {Client.FrameTime}");
        Input.OnKeyPress += OnKeyPress;
        Events.SubscribeRemote("nanossharp:csharp-ping", (long value) =>
        {
            Check("ServerRemoteEvent", () => $"received {value}");
            Events.CallRemoteUnreliable("nanossharp:csharp-pong", value + 1);
        });
        _interval = ScriptTimer.SetInterval(OnInterval, 1000);

        await Scheduler.Delay(500);
        Log.Info("client probe resumed after await");
        Check("PackageInfoLater", () => $"{Title} {Version}");
        Check("LocalPlayer", () => Client.LocalPlayer?.Name ?? "no local player yet");
        CheckLuaInterop();
        CheckCommands();
        Check("ClientEntity", () =>
        {
            _text = new TextRender(new Vector(0, 0, 300), Rotator.Zero, "NanosSharp");
            return $"spawned {_text}";
        });
    }

    protected override void OnStop()
    {
        Input.OnKeyPress -= OnKeyPress;
        ScriptTimer.ClearInterval(_interval);
        if (_text is { IsValid: true })
        {
            _text.Destroy();
        }

        Log.Info("client probe stopped");
    }

    [Export]
    public static string ClientShout(string text) => text.ToUpperInvariant();

    private static void CheckLuaInterop()
    {
        Check("LuaExportCall", () => Require(Exports.Call<long>("ClientLuaLibrary.Add", 2, 3), 5L));
        Check("LuaInvokesCSharpDelegate", () => Require(Exports.Call<long>("ClientLuaLibrary.Invoke", ScriptCallback.From((Func<long, long>)(value => value * 2)), 21), 42L));
        Exports.Set("CSharpClientLibrary", new Dictionary<string, object?> { ["Multiply"] = (Func<long, long, long>)((first, second) => first * second) });
        Check("LuaUsesCSharpExport", () => Require(Exports.Call<long>("ClientLuaUseCSharp", 6, 7), 42L));
        Exports.Set("ClientProbeValue", new Dictionary<string, object?> { ["count"] = 12 });
        Check("ExportRoundTrip", () => Require(Exports.Get<long>("ClientProbeValue.count"), 12L));
        Check("ExportAttribute", () => Require(Exports.Call<string>("ClientShout", "nanos"), "NANOS"));
    }

    private static void CheckCommands()
    {
        Commands.Register("nanossharp_probe", arguments => Check("ConsoleCommand", () => $"arguments '{string.Join(' ', arguments)}'"), "NanosSharp client probe");
        Commands.Run("nanossharp_probe hello world");
    }

    private static string Require<T>(T? actual, T expected)
    {
        if (!Equals(actual, expected))
        {
            throw new InvalidOperationException($"expected {expected} but got {actual}");
        }

        return $"{actual}";
    }

    private bool OnKeyPress(string? keyName, float delta)
    {
        if (!_keyLogged)
        {
            _keyLogged = true;
            Check("InputEvent", () => $"key {keyName}");
        }

        return true;
    }

    private void OnInterval()
    {
        _ticks++;
        if (_ticks <= 3)
        {
            Log.Info($"client probe interval {_ticks}");
        }
    }

    private static void Check(string name, Func<string> probe)
    {
        try
        {
            Log.Info($"{Prefix} PASS {name}: {probe()}");
        }
        catch (Exception exception)
        {
            Log.Error($"{Prefix} FAIL {name}: {exception.Message}");
        }
    }
}
