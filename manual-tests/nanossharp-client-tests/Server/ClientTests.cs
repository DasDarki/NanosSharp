public sealed class ClientTests : Package
{
    private const string Prefix = "[TEST]";
    private const string CancelledMessage = "nanossharp cancel";

    private readonly HashSet<string> _passed = [];
    private readonly HashSet<string> _failed = [];
    private readonly string[] _expected =
    [
        "PlayerConnect", "PlayerSpawn", "PlayerReady", "PlayerFactory", "PlayerProperties", "Possess", "ClientRemoteEvent",
        "ServerRemoteEvent", "ChatEventClass", "ChatCancel", "PlayerDestroy",
    ];

    protected override void OnStart()
    {
        Server.OnPlayerConnect += (ip, accountId, name, steamId) => Check("PlayerConnect", name?.Length > 0, $"name '{name}'");
        Player.OnSpawn += player => Check("PlayerSpawn", player is TestPlayer, player.GetType().Name);
        Player.OnReady += OnReady;
        Player.OnDestroy += OnPlayerDestroy;
        Events.SubscribeRemote("nanossharp:client-ready", (Player player, long number, string text, Vector vector) =>
            Check("ClientRemoteEvent", number == 42 && text == "hello from lua" && vector == new Vector(1, 2, 3), $"{number} {text} {vector}"));
        Events.SubscribeRemote("nanossharp:client-pong", (Player player, long value) => Check("ServerRemoteEvent", value == 7, $"pong {value}"));
        Log.Info($"{Prefix} waiting for a player to join");
    }

    [ServerEvent]
    private void OnChat(ChatPlayerSubmitEvent chatEvent)
    {
        if (chatEvent.Message == CancelledMessage)
        {
            chatEvent.Cancel();
            Check("ChatCancel", true, "message cancelled, it must not appear in the chat");
            return;
        }

        Check("ChatEventClass", chatEvent.Player is TestPlayer && chatEvent.Message?.Length > 0, $"'{chatEvent.Message}' from {chatEvent.Player?.Name}");
    }

    private void OnReady(Player player)
    {
        Check("PlayerReady", true, player.Name);
        Check("PlayerFactory", player is TestPlayer { JoinedAt: > 0 }, player.GetType().Name);
        Check("PlayerProperties", Player.All.Contains(player) && player.Name.Length > 0 && player.GetAccountID().Length > 0,
            $"{player.Name} {player.GetAccountID()} ping {player.GetPing()}");

        var character = new Character(new Vector(0, 0, 200), Rotator.Zero, "nanos-world::SK_Mannequin");
        Character.PossessHandler? onPossess = null;
        onPossess = (self, possessor) =>
        {
            Check("Possess", ReferenceEquals(self, character) && ReferenceEquals(possessor, player) &&
                             ReferenceEquals(player.GetControlledCharacter(), character), $"{possessor?.Name} possessed {self}");
            Character.OnPossess -= onPossess;
        };
        Character.OnPossess += onPossess;
        player.Possess(character);

        Events.CallRemote("nanossharp:server-ping", player, 7);
        player.SendMessage("NanosSharp client tests: write any chat message, then write 'nanossharp cancel' (it must NOT appear), then disconnect.");
    }

    private void OnPlayerDestroy(Player player)
    {
        Check("PlayerDestroy", player is TestPlayer, player.GetType().Name);
        ScriptTimer.SetTimeout(() => Finish(player), 100);
    }

    private void Finish(Player player)
    {
        Check("PlayerInvalidAfterLeave", !player.IsValid, $"valid {player.IsValid}");
        foreach (var missing in _expected.Where(name => !_passed.Contains(name) && !_failed.Contains(name)))
        {
            Check(missing, false, "not observed");
        }

        Log.Info($"{Prefix} SUMMARY passed={_passed.Count} failed={_failed.Count}");
        MainThread.Post(Server.Stop);
    }

    private void Check(string name, bool condition, string detail)
    {
        if (_passed.Contains(name) || _failed.Contains(name))
        {
            return;
        }

        (condition ? _passed : _failed).Add(name);
        var message = $"{Prefix} {(condition ? "PASS" : "FAIL")} {name}: {detail}";
        if (condition)
        {
            Log.Info(message);
            return;
        }

        Log.Error(message);
    }
}

[EntityFactory]
public sealed class TestPlayer : Player
{
    public long JoinedAt { get; } = Environment.TickCount64;
}
