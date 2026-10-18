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
