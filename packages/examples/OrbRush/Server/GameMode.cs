namespace OrbRush;

/// <summary>Server side of Orb Rush: spawns players, scatters orbs, scores pickups and runs the round loop.</summary>
public sealed class GameMode : Package
{
    private static readonly Vector ArenaCenter = new(0, 0, 150);
    private const double ArenaRadius = 2500;
    private const double SpawnHeight = 120;

    [Inject]
    private ScoreBoard _scoreBoard = null!;

    private readonly List<Orb> _orbs = new();
    private readonly Random _random = new();
    private int _roundTimer;

    /// <inheritdoc />
    protected override void OnStart()
    {
        Log.Info($"Orb Rush server starting, protocol {Protocol.Version}.");
        Player.OnReady += OnPlayerReady;
        Events.SubscribeRemote(Protocol.RequestPing, (Player player) => OnPingRequested(player));

        for (var index = 0; index < Protocol.OrbCount; index++)
        {
            _orbs.Add(SpawnOrb());
        }

        StartRound();
    }

    /// <inheritdoc />
    protected override void OnStop()
    {
        Player.OnReady -= OnPlayerReady;
        ScriptTimer.ClearTimeout(_roundTimer);

        foreach (var orb in _orbs)
        {
            orb.Destroy();
        }

        _orbs.Clear();
        Log.Info("Orb Rush server stopped.");
    }

    private void OnPlayerReady(Player player)
    {
        var character = new Character(ArenaCenter + new Vector(0, 0, SpawnHeight), Rotator.Zero, "nanos-world::SK_Mannequin");
        player.Possess(character);
        _scoreBoard.Register(player);
        player.SendMessage($"Welcome {player.Name}! Grab the glowing orbs - {RemainingSeconds()}s left in the round.");
        BroadcastScores();
    }

    private void StartRound()
    {
        _scoreBoard.Reset();

        foreach (var orb in _orbs)
        {
            orb.MoveTo(RandomPoint());
        }

        Events.BroadcastRemote(Protocol.RoundStarted, Protocol.RoundSeconds);
        BroadcastScores();
        _roundTimer = ScriptTimer.SetTimeout(EndRound, (int)(Protocol.RoundSeconds * 1000));
        Log.Info("Orb Rush round started.");
    }

    private void EndRound()
    {
        var leader = _scoreBoard.Leader();
        if (leader is null)
        {
            Events.BroadcastRemote(Protocol.RoundEnded, "nobody", 0);
        }
        else
        {
            Events.BroadcastRemote(Protocol.RoundEnded, leader.Value.Name, leader.Value.Score);
        }

        _roundTimer = ScriptTimer.SetTimeout(StartRound, 5000);
    }

    private Orb SpawnOrb()
    {
        var location = RandomPoint();
        var light = new Light(
            location,
            color: new Color(1f, 0.82f, 0.2f),
            lightType: LightType.Point,
            intensity: 5000f,
            attenuationRadius: 600f,
            castShadows: false);
        var trigger = new Trigger(
            location,
            Rotator.Zero,
            new Vector(150, 150, 300),
            TriggerType.Sphere,
            isVisible: false,
            overlapOnlyClasses: ["Character"]);

        var orb = new Orb(light, trigger);
        trigger.SubscribeBeginOverlap((_, actor) => OnOrbOverlap(orb, actor));
        return orb;
    }

    private void OnOrbOverlap(Orb orb, Actor? actor)
    {
        if (actor is not Character character || !character.IsValid)
        {
            return;
        }

        var player = character.Player;
        if (player is null || !player.IsValid)
        {
            return;
        }

        var score = _scoreBoard.Award(player);
        orb.MoveTo(RandomPoint());
        Events.CallRemote(Protocol.OrbCollected, player, score);
        BroadcastScores();
    }

    private void OnPingRequested(Player player)
    {
        if (player.GetControlledCharacter() is not Character character || !character.IsValid)
        {
            return;
        }

        var from = character.GetLocation();
        var nearest = FindNearestOrb(from);
        if (nearest is null)
        {
            return;
        }

        var offset = nearest.Light.GetLocation() - from;
        Events.CallRemote(Protocol.PingResult, player, offset.X, offset.Y, offset.Length);
    }

    private Orb? FindNearestOrb(Vector from)
    {
        Orb? nearest = null;
        var best = double.MaxValue;

        foreach (var orb in _orbs)
        {
            if (!orb.Light.IsValid)
            {
                continue;
            }

            var distance = (orb.Light.GetLocation() - from).Length;
            if (distance >= best)
            {
                continue;
            }

            best = distance;
            nearest = orb;
        }

        return nearest;
    }

    private void BroadcastScores()
    {
        Events.BroadcastRemote(Protocol.ScoreUpdate, _scoreBoard.Leaderboard());
    }

    private long RemainingSeconds()
    {
        if (!ScriptTimer.IsValid(_roundTimer))
        {
            return 0;
        }

        return Math.Max(0, ScriptTimer.GetRemainingTime(_roundTimer) / 1000);
    }

    private Vector RandomPoint()
    {
        var angle = _random.NextDouble() * Math.PI * 2;
        var distance = Math.Sqrt(_random.NextDouble()) * ArenaRadius;
        var x = ArenaCenter.X + Math.Cos(angle) * distance;
        var y = ArenaCenter.Y + Math.Sin(angle) * distance;
        return new Vector(x, y, ArenaCenter.Z);
    }
}
