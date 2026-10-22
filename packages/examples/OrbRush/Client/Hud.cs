namespace OrbRush;

/// <summary>Client side of Orb Rush: a chat based HUD, a local round clock and a key bound orb finder.</summary>
public sealed class Hud : Package
{
    private const string PingKey = "F";

    private readonly List<ScriptSubscription> _subscriptions = new();
    private float _secondsLeft;
    private int _lastAnnounced;

    /// <inheritdoc />
    protected override void OnStart()
    {
        Client.OnSpawnLocalPlayer += OnSpawnLocalPlayer;
        Client.OnTick += OnTick;
        Input.OnKeyPress += OnKeyPress;

        _subscriptions.Add(Events.SubscribeRemote(Protocol.OrbCollected, (int score) => OnOrbCollected(score)));
        _subscriptions.Add(Events.SubscribeRemote(Protocol.ScoreUpdate, (string board) => OnScoreUpdate(board)));
        _subscriptions.Add(Events.SubscribeRemote(Protocol.RoundStarted, (float seconds) => OnRoundStarted(seconds)));
        _subscriptions.Add(Events.SubscribeRemote(Protocol.RoundEnded, (string winner, int score) => OnRoundEnded(winner, score)));
        _subscriptions.Add(Events.SubscribeRemote(Protocol.PingResult, (double x, double y, double distance) => OnPingResult(x, y, distance)));

        Log.Info($"Orb Rush HUD ready, protocol {Protocol.Version}. Press {PingKey} to ping the nearest orb.");
    }

    /// <inheritdoc />
    protected override void OnStop()
    {
        Client.OnSpawnLocalPlayer -= OnSpawnLocalPlayer;
        Client.OnTick -= OnTick;
        Input.OnKeyPress -= OnKeyPress;

        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
        Log.Info("Orb Rush HUD stopped.");
    }

    private void OnSpawnLocalPlayer(Player? localPlayer)
    {
        var name = localPlayer?.Name ?? "player";
        Chat.AddMessage($"<cyan>Orb Rush</> loaded for {name}. Press <green>{PingKey}</> to find the nearest orb.");
    }

    private void OnTick(float deltaTime)
    {
        if (_secondsLeft <= 0f)
        {
            return;
        }

        _secondsLeft -= deltaTime;
        var whole = (int)Math.Ceiling(_secondsLeft);
        if (whole > 5 || whole < 1 || whole == _lastAnnounced)
        {
            return;
        }

        _lastAnnounced = whole;
        Chat.AddMessage($"Round ending in {whole}...");
    }

    private bool OnKeyPress(string? keyName, float delta)
    {
        if (string.Equals(keyName, PingKey, StringComparison.OrdinalIgnoreCase))
        {
            Events.CallRemote(Protocol.RequestPing);
        }

        return true;
    }

    private void OnOrbCollected(int score)
    {
        Chat.AddMessage($"<green>Orb collected!</> Your score is now {score}.");
    }

    private void OnScoreUpdate(string board)
    {
        Chat.AddMessage($"<grey>Scores:</> {board}");
    }

    private void OnRoundStarted(float seconds)
    {
        _secondsLeft = seconds;
        _lastAnnounced = 0;
        Chat.AddMessage($"<yellow>New round!</> Collect as many orbs as you can in {(int)seconds} seconds.");
    }

    private void OnRoundEnded(string winner, int score)
    {
        _secondsLeft = 0f;
        _lastAnnounced = 0;
        Chat.AddMessage($"<yellow>Round over!</> Winner: {winner} with {score} orbs.");
    }

    private void OnPingResult(double x, double y, double distance)
    {
        var heading = Compass(x, y);
        Chat.AddMessage($"Nearest orb is <green>{(int)distance}</> units to the {heading}.");
    }

    private static string Compass(double x, double y)
    {
        var northSouth = x >= 0 ? "north" : "south";
        var eastWest = y >= 0 ? "east" : "west";
        if (Math.Abs(x) >= Math.Abs(y) * 2)
        {
            return northSouth;
        }

        if (Math.Abs(y) >= Math.Abs(x) * 2)
        {
            return eastWest;
        }

        return $"{northSouth}-{eastWest}";
    }
}
