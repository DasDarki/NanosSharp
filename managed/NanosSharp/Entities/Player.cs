using NanosSharp.Core;
using NanosSharp.Entities.Binding;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A connected player.</summary>
[ScriptClass(ScriptClassName)]
public sealed class Player : Entity
{
    /// <summary>All players currently connected.</summary>
    public static IEnumerable<Player> All => EntityRegistry.All.OfType<Player>().Where(player => player.IsValid);

    /// <summary>Raised when a player spawned.</summary>
    public static event Action<Player>? Spawned
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Raised when a player finished loading and is ready.</summary>
    public static event Action<Player>? Ready
    {
        add => ReadyEvent.Add(value);
        remove => ReadyEvent.Remove(value);
    }

    /// <summary>Raised when a player left the server.</summary>
    public static event Action<Player>? Left
    {
        add => DestroyEvent.Add(value);
        remove => DestroyEvent.Remove(value);
    }

    /// <summary>Raised when a player took control of a character.</summary>
    public static event Action<Player, Character>? Possessed
    {
        add => PossessEvent.Add(value);
        remove => PossessEvent.Remove(value);
    }

    /// <summary>Raised when a player released a character.</summary>
    public static event Action<Player, Character>? UnPossessed
    {
        add => UnPossessEvent.Add(value);
        remove => UnPossessEvent.Remove(value);
    }

    private const string ScriptClassName = "Player";
    private static readonly ClassEvent<Action<Player>> SpawnEvent = new(ScriptClassName, "Spawn");
    private static readonly ClassEvent<Action<Player>> ReadyEvent = new(ScriptClassName, "Ready");
    private static readonly ClassEvent<Action<Player>> DestroyEvent = new(ScriptClassName, "Destroy");
    private static readonly ClassEvent<Action<Player, Character>> PossessEvent = new(ScriptClassName, "Possess");
    private static readonly ClassEvent<Action<Player, Character>> UnPossessEvent = new(ScriptClassName, "UnPossess");

    /// <summary>Display name.</summary>
    public string Name
    {
        get => Invoke<string>("GetName") ?? string.Empty;
        set => Invoke("SetName", value);
    }

    /// <summary>nanos world account id.</summary>
    public string AccountId => Invoke<string>("GetAccountID") ?? string.Empty;

    /// <summary>nanos world account name.</summary>
    public string AccountName => Invoke<string>("GetAccountName") ?? string.Empty;

    /// <summary>Steam id.</summary>
    public string SteamId => Invoke<string>("GetSteamID") ?? string.Empty;

    /// <summary>IP address.</summary>
    public string Ip => Invoke<string>("GetIP") ?? string.Empty;

    /// <summary>Ping in milliseconds.</summary>
    public int Ping => Invoke<int>("GetPing");

    /// <summary>Whether the player hosts the server.</summary>
    public bool IsHost => Invoke<bool>("IsHost");

    /// <summary>Character currently controlled by the player.</summary>
    public Character? ControlledCharacter => Invoke<Character>("GetControlledCharacter");

    private Player()
    {
    }

    /// <summary>Takes control of a character.</summary>
    public void Possess(Character character, float blendTime = 0f, float exponent = 0f) =>
        Invoke("Possess", character, blendTime, exponent);

    /// <summary>Releases the controlled character.</summary>
    public void UnPossess() => Invoke("UnPossess");

    /// <summary>Kicks the player.</summary>
    public void Kick(string reason) => Invoke("Kick", reason);

    /// <summary>Bans the player.</summary>
    public void Ban(string reason) => Invoke("Ban", reason);

    /// <summary>Sends a chat message only this player sees.</summary>
    public void SendMessage(string message) => Chat.SendMessage(this, message);
}
