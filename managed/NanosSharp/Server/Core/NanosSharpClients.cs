using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Core;

/// <summary>Tracks which players run a compatible NanosSharp client runtime.</summary>
public static class NanosSharpClients
{
    private const string VersionPlaceholder = "{version}";
    private static readonly Dictionary<Player, ClientState> States = [];
    private static readonly List<ScriptSubscription> Subscriptions = [];
    private static ClientPolicy _policy = new(false, TimeSpan.FromSeconds(60), string.Empty);

    private delegate void HandshakeHandler(Player player, long protocolVersion = Handshake.MissingProtocolVersion, string? version = null);

    /// <summary>Raised when a player completed the handshake with a compatible NanosSharp client.</summary>
    public static event Action<Player, NanosSharpClient>? Connected;

    /// <summary>Raised when a player has no compatible NanosSharp client, either reported by a Lua fallback or when the handshake request stays unanswered.</summary>
    public static event Action<Player>? Missing;

    /// <summary>Whether players without a compatible NanosSharp client are kicked.</summary>
    public static bool IsRequired => _policy.IsRequired;

    /// <summary>The NanosSharp client of the player, null when it is missing or was not reported yet.</summary>
    public static NanosSharpClient? Get(Player player) => States.TryGetValue(player, out var state) ? state.Client : null;

    internal static void Start(ClientPolicy policy)
    {
        _policy = policy;
        Subscriptions.Add(Events.SubscribeRemote(Handshake.EventName, (HandshakeHandler)OnHandshake));
        Subscriptions.Add(ScriptApi.Subscribe("Player", "Ready", (Action<Player>)OnReady));
        Subscriptions.Add(ScriptApi.Subscribe("Player", "Destroy", (Action<Player>)OnDestroy));
    }

    internal static void Stop()
    {
        foreach (var subscription in Subscriptions)
        {
            subscription.Dispose();
        }

        Subscriptions.Clear();
        foreach (var state in States.Values)
        {
            ScriptTimer.ClearTimeout(state.TimeoutTimer);
        }

        States.Clear();
    }

    private static void OnHandshake(Player player, long protocolVersion, string? version)
    {
        var state = GetState(player);
        if (state.IsResolved)
        {
            return;
        }

        if (protocolVersion == Handshake.MissingProtocolVersion)
        {
            ResolveMissing(player, state, "has no NanosSharp client");
            return;
        }

        if (protocolVersion != Handshake.ProtocolVersion)
        {
            ResolveMissing(player, state,
                $"has NanosSharp {version} with protocol {protocolVersion}, the server expects protocol {Handshake.ProtocolVersion}");
            return;
        }

        state.Client = new NanosSharpClient(version ?? "unknown", (int)protocolVersion);
        state.IsResolved = true;
        ScriptTimer.ClearTimeout(state.TimeoutTimer);
        Log.Info($"{player.Name} joined with NanosSharp {state.Client.Version}.");
        Raise(() => Connected?.Invoke(player, state.Client));
    }

    private static void OnReady(Player player)
    {
        var state = GetState(player);
        if (state.IsResolved)
        {
            return;
        }

        state.TimeoutTimer = ScriptTimer.SetTimeout(() => OnTimeout(player), (int)_policy.HandshakeTimeout.TotalMilliseconds);
        Events.CallRemote(Handshake.RequestEventName, player);
    }

    private static void OnTimeout(Player player)
    {
        if (States.TryGetValue(player, out var state) && !state.IsResolved)
        {
            ResolveMissing(player, state, "did not report a NanosSharp client in time");
        }
    }

    private static void OnDestroy(Player player)
    {
        if (States.Remove(player, out var state))
        {
            ScriptTimer.ClearTimeout(state.TimeoutTimer);
        }
    }

    private static void ResolveMissing(Player player, ClientState state, string reason)
    {
        state.IsResolved = true;
        ScriptTimer.ClearTimeout(state.TimeoutTimer);
        Log.Info($"{player.Name} {reason}.");
        Raise(() => Missing?.Invoke(player));
        if (_policy.IsRequired && player.IsValid)
        {
            player.Kick(_policy.KickMessage.Replace(VersionPlaceholder, Handshake.RuntimeVersion, StringComparison.Ordinal));
        }
    }

    private static ClientState GetState(Player player)
    {
        if (!States.TryGetValue(player, out var state))
        {
            state = new ClientState();
            States[player] = state;
        }

        return state;
    }

    private static void Raise(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Log.Exception(exception, "A NanosSharp client handler failed");
        }
    }

    private sealed class ClientState
    {
        public NanosSharpClient? Client { get; set; }

        public bool IsResolved { get; set; }

        public int TimeoutTimer { get; set; }
    }
}
