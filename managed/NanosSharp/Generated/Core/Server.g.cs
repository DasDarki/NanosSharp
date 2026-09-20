#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Server represents all Server controls in the Server side.</summary>
public static partial class Server
{
    /// <summary>Called when the map changes</summary>
    public static event ChangeMapHandler? OnChangeMap
    {
        add => ChangeMapEvent.Add(value);
        remove => ChangeMapEvent.Remove(value);
    }

    /// <summary>Triggered when any package is loaded</summary>
    public static event PackageLoadHandler? OnPackageLoad
    {
        add => PackageLoadEvent.Add(value);
        remove => PackageLoadEvent.Remove(value);
    }

    /// <summary>Triggered when any package is unloaded</summary>
    public static event PackageUnloadHandler? OnPackageUnload
    {
        add => PackageUnloadEvent.Add(value);
        remove => PackageUnloadEvent.Remove(value);
    }

    /// <summary>Called when a player is attempting to connect to the server</summary>
    public static event PlayerConnectHandler? OnPlayerConnect
    {
        add => PlayerConnectEvent.Add(value);
        remove => PlayerConnectEvent.Remove(value);
    }

    /// <summary>Called when a player is disconnecting from the server</summary>
    public static event PlayerDisconnectHandler? OnPlayerDisconnect
    {
        add => PlayerDisconnectEvent.Add(value);
        remove => PlayerDisconnectEvent.Remove(value);
    }

    /// <summary>Triggered when the server is restarting</summary>
    public static event RestartHandler? OnRestart
    {
        add => RestartEvent.Add(value);
        remove => RestartEvent.Remove(value);
    }

    /// <summary>Triggered once when the server has started and all packages are loaded</summary>
    public static event StartHandler? OnStart
    {
        add => StartEvent.Add(value);
        remove => StartEvent.Remove(value);
    }

    /// <summary>Triggered when the server is stopping</summary>
    public static event StopHandler? OnStop
    {
        add => StopEvent.Add(value);
        remove => StopEvent.Remove(value);
    }

    /// <summary>Triggered every 33ms by default</summary>
    public static event TickHandler? OnTick
    {
        add => TickEvent.Add(value);
        remove => TickEvent.Remove(value);
    }

    /// <summary>Triggered when a Value is changed with <c>Server.SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ChangeMapHandler> ChangeMapEvent = new("Server", "ChangeMap");
    private static readonly ClassEvent<PackageLoadHandler> PackageLoadEvent = new("Server", "PackageLoad");
    private static readonly ClassEvent<PackageUnloadHandler> PackageUnloadEvent = new("Server", "PackageUnload");
    private static readonly ClassEvent<PlayerConnectHandler> PlayerConnectEvent = new("Server", "PlayerConnect");
    private static readonly ClassEvent<PlayerDisconnectHandler> PlayerDisconnectEvent = new("Server", "PlayerDisconnect");
    private static readonly ClassEvent<RestartHandler> RestartEvent = new("Server", "Restart");
    private static readonly ClassEvent<StartHandler> StartEvent = new("Server", "Start");
    private static readonly ClassEvent<StopHandler> StopEvent = new("Server", "Stop");
    private static readonly ClassEvent<TickHandler> TickEvent = new("Server", "Tick");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Server", "ValueChange");

    /// <summary>Gets a list of all values keys</summary>
    public static IReadOnlyList<string> AllValuesKeys => GetAllValuesKeys();

    /// <summary>Returns the number of connected players (including players in the process of connecting)</summary>
    public static int ConnectionCount => GetConnectionCount();

    /// <summary>Returns the Server Custom Settings passed as parameter or New Game screen</summary>
    public static IReadOnlyDictionary<string, object?> CustomSettings => GetCustomSettings();

    /// <summary>Returns the default entity dimension</summary>
    public static uint DefaultEntityDimension
    {
        get => GetDefaultEntityDimension();
        set => SetDefaultEntityDimension(value);
    }

    /// <summary>Returns the default player dimension</summary>
    public static uint DefaultPlayerDimension
    {
        get => GetDefaultPlayerDimension();
        set => SetDefaultPlayerDimension(value);
    }

    /// <summary>Returns the Server Description</summary>
    public static string Description
    {
        get => GetDescription();
        set => SetDescription(value);
    }

    /// <summary>Returns the server Public IP</summary>
    public static string IP => GetIP();

    /// <summary>Returns the current log level</summary>
    public static int LogLevel => GetLogLevel();

    /// <summary>Returns the current Map Package name</summary>
    public static string Map => GetMap();

    /// <summary>Returns the current Map Asset reference</summary>
    public static string MapAsset => GetMapAsset();

    /// <summary>Returns a list of the Custom Config of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyDictionary<string, object?> MapConfig => GetMapConfig();

    /// <summary>Returns a list of the Spawn Points of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> MapSpawnPoints => GetMapSpawnPoints();

    /// <summary>Returns the Max Players configured</summary>
    public static int MaxPlayers => GetMaxPlayers();

    /// <summary>Returns the Server Name</summary>
    public static string Name
    {
        get => GetName();
        set => SetName(value);
    }

    /// <summary>Returns the server Port</summary>
    public static int Port => GetPort();

    /// <summary>Returns the server QueryPort</summary>
    public static int QueryPort => GetQueryPort();

    /// <summary>Returns the Configured Tick Rate</summary>
    public static int TickRate => GetTickRate();

    /// <summary>Gets the Unix Epoch Time in milliseconds</summary>
    public static ulong Time => GetTime();

    /// <summary>Returns the server version</summary>
    public static string Version => GetVersion();

    /// <summary>Returns if the Server is announced in the Master List</summary>
    public static bool Announced => IsAnnounced();

    /// <summary>Returns if the server was started with --enable_unsafe_libs</summary>
    public static int UnsafeLibsEnabled => IsUnsafeLibsEnabled();

    /// <summary>Adds a spawn point to the current map's spawn points list</summary>
    public static void AddMapSpawnPoint(Vector location, Rotator? rotation = null) =>
        ScriptApi.Invoke("Server", "AddMapSpawnPoint", ScriptArguments.Trim([location, rotation], 1));

    /// <summary>Ban the player from the server by its Account ID, ideal for banning from inside PlayerConnect event</summary>
    public static void BanByAccountID(string playerAccountId, string reason) =>
        ScriptApi.Invoke("Server", "BanByAccountID", playerAccountId, reason);

    /// <summary>Restarts the server in a new Map, restarts all packages and reconnects all players</summary>
    public static void ChangeMap(string mapPath) => ScriptApi.Invoke("Server", "ChangeMap", mapPath);

    /// <summary>Returns the actors within the specified radius</summary>
    public static IReadOnlyList<object?> GetActorsInRadius(
        Vector location,
        float radius,
        IEnumerable<string>? onlyClasses = null,
        uint? dimension = null) =>
        ScriptApi.Invoke<IReadOnlyList<object?>>("Server", "GetActorsInRadius", ScriptArguments.Trim([location, radius, onlyClasses, dimension], 2))!;

    /// <summary>Gets a list of all values keys</summary>
    public static IReadOnlyList<string> GetAllValuesKeys() =>
        ScriptApi.Invoke<IReadOnlyList<string>>("Server", "GetAllValuesKeys")!;

    /// <summary>Returns the number of connected players (including players in the process of connecting)</summary>
    public static int GetConnectionCount() => ScriptApi.Invoke<int>("Server", "GetConnectionCount")!;

    /// <summary>Returns the Server Custom Settings passed as parameter or New Game screen</summary>
    public static IReadOnlyDictionary<string, object?> GetCustomSettings() =>
        ScriptApi.Invoke<IReadOnlyDictionary<string, object?>>("Server", "GetCustomSettings")!;

    /// <summary>Returns the default entity dimension</summary>
    public static uint GetDefaultEntityDimension() => ScriptApi.Invoke<uint>("Server", "GetDefaultEntityDimension")!;

    /// <summary>Returns the default player dimension</summary>
    public static uint GetDefaultPlayerDimension() => ScriptApi.Invoke<uint>("Server", "GetDefaultPlayerDimension")!;

    /// <summary>Returns the Server Description</summary>
    public static string GetDescription() => ScriptApi.Invoke<string>("Server", "GetDescription")!;

    /// <summary>Returns the entity with the specified ID</summary>
    public static object? GetEntityByID(uint entityId) =>
        ScriptApi.Invoke<object?>("Server", "GetEntityByID", entityId);

    /// <summary>Returns the server Public IP</summary>
    public static string GetIP() => ScriptApi.Invoke<string>("Server", "GetIP")!;

    /// <summary>Returns the current log level</summary>
    public static int GetLogLevel() => ScriptApi.Invoke<int>("Server", "GetLogLevel")!;

    /// <summary>Returns the current Map Package name</summary>
    public static string GetMap() => ScriptApi.Invoke<string>("Server", "GetMap")!;

    /// <summary>Returns the current Map Asset reference</summary>
    public static string GetMapAsset() => ScriptApi.Invoke<string>("Server", "GetMapAsset")!;

    /// <summary>Returns a list of the Custom Config of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyDictionary<string, object?> GetMapConfig() =>
        ScriptApi.Invoke<IReadOnlyDictionary<string, object?>>("Server", "GetMapConfig")!;

    /// <summary>Returns a list of the Spawn Points of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetMapSpawnPoints() =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<string, object?>>>("Server", "GetMapSpawnPoints")!;

    /// <summary>Returns the Max Players configured</summary>
    public static int GetMaxPlayers() => ScriptApi.Invoke<int>("Server", "GetMaxPlayers")!;

    /// <summary>Returns the Server Name</summary>
    public static string GetName() => ScriptApi.Invoke<string>("Server", "GetName")!;

    /// <summary>Returns a list of Packages running, optionally returns all Packages installed in the server</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetPackages(
        bool? onlyLoaded = null,
        PackageType? packageTypeFilter = null) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<string, object?>>>("Server", "GetPackages", ScriptArguments.Trim([onlyLoaded, packageTypeFilter], 0))!;

    /// <summary>Returns the players within the specified radius</summary>
    public static IReadOnlyList<object?> GetPlayersInRadius(Vector location, float radius, uint? dimension = null) =>
        ScriptApi.Invoke<IReadOnlyList<object?>>("Server", "GetPlayersInRadius", ScriptArguments.Trim([location, radius, dimension], 2))!;

    /// <summary>Returns the server Port</summary>
    public static int GetPort() => ScriptApi.Invoke<int>("Server", "GetPort")!;

    /// <summary>Returns the server QueryPort</summary>
    public static int GetQueryPort() => ScriptApi.Invoke<int>("Server", "GetQueryPort")!;

    /// <summary>Returns the Configured Tick Rate</summary>
    public static int GetTickRate() => ScriptApi.Invoke<int>("Server", "GetTickRate")!;

    /// <summary>Gets the Unix Epoch Time in milliseconds</summary>
    public static ulong GetTime() => ScriptApi.Invoke<ulong>("Server", "GetTime")!;

    /// <summary>Gets a value given a key</summary>
    public static object? GetValue(string key, object? fallback = null) =>
        ScriptApi.Invoke<object?>("Server", "GetValue", ScriptArguments.Trim([key, fallback], 1));

    /// <summary>Returns the server version</summary>
    public static string GetVersion() => ScriptApi.Invoke<string>("Server", "GetVersion")!;

    /// <summary>Returns if the Server is announced in the Master List</summary>
    public static bool IsAnnounced() => ScriptApi.Invoke<bool>("Server", "IsAnnounced")!;

    /// <summary>Returns if a Package is currently loaded</summary>
    public static bool IsPackageLoaded(string packageName) =>
        ScriptApi.Invoke<bool>("Server", "IsPackageLoaded", packageName)!;

    /// <summary>Returns if the server was started with --enable_unsafe_libs</summary>
    public static int IsUnsafeLibsEnabled() => ScriptApi.Invoke<int>("Server", "IsUnsafeLibsEnabled")!;

    /// <summary>Kicks the player from the server by its Account ID, ideal for kicking from inside PlayerConnect event</summary>
    public static void KickByAccountID(string playerAccountId, string reason) =>
        ScriptApi.Invoke("Server", "KickByAccountID", playerAccountId, reason);

    /// <summary>Loads a Package</summary>
    public static bool LoadPackage(string packageFolderName) =>
        ScriptApi.Invoke<bool>("Server", "LoadPackage", packageFolderName)!;

    /// <summary>Reloads a Package on the next server tick</summary>
    public static bool ReloadPackage(string packageFolderName) =>
        ScriptApi.Invoke<bool>("Server", "ReloadPackage", packageFolderName)!;

    /// <summary>Restarts the server, reloading all Packages and reconnecting all Players</summary>
    public static void Restart() => ScriptApi.Invoke("Server", "Restart");

    /// <summary>Sets the default server dimension that entities will spawn in</summary>
    public static void SetDefaultEntityDimension(uint defaultDimension) =>
        ScriptApi.Invoke("Server", "SetDefaultEntityDimension", defaultDimension);

    /// <summary>Sets the default server dimension that players will spawn in</summary>
    public static void SetDefaultPlayerDimension(uint defaultDimension) =>
        ScriptApi.Invoke("Server", "SetDefaultPlayerDimension", defaultDimension);

    /// <summary>Sets the description of the server, optionally overrides Config.toml</summary>
    public static void SetDescription(string description, bool? persistToConfigFile = null) =>
        ScriptApi.Invoke("Server", "SetDescription", ScriptArguments.Trim([description, persistToConfigFile], 1));

    /// <summary>Sets the logo of the server, optionally overrides Config.toml</summary>
    public static void SetLogo(string logoUrl, bool? persistToConfigFile = null) =>
        ScriptApi.Invoke("Server", "SetLogo", ScriptArguments.Trim([logoUrl, persistToConfigFile], 1));

    /// <summary>Sets the maximum player slots of the server, optionally overrides Config.toml</summary>
    public static void SetMaxPlayers(uint maxPlayers, bool? persistToConfigFile = null) =>
        ScriptApi.Invoke("Server", "SetMaxPlayers", ScriptArguments.Trim([maxPlayers, persistToConfigFile], 1));

    /// <summary>Sets the name of the server, optionally overrides Config.toml</summary>
    public static void SetName(string name, bool? persistToConfigFile = null) =>
        ScriptApi.Invoke("Server", "SetName", ScriptArguments.Trim([name, persistToConfigFile], 1));

    /// <summary>Sets the password of the server, optionally overrides Config.toml</summary>
    public static void SetPassword(string password, bool? persistToConfigFile = null) =>
        ScriptApi.Invoke("Server", "SetPassword", ScriptArguments.Trim([password, persistToConfigFile], 1));

    /// <summary>Sets a global value in the Server, which can be accessed from anywhere (server side)</summary>
    public static void SetValue(string key, object? value, bool? syncOnClient = null) =>
        ScriptApi.Invoke("Server", "SetValue", ScriptArguments.Trim([key, value, syncOnClient], 2));

    /// <summary>Stops the server</summary>
    public static void Stop() => ScriptApi.Invoke("Server", "Stop");

    /// <summary>Unbans a Player by its account ID</summary>
    public static void Unban(string accountId) => ScriptApi.Invoke("Server", "Unban", accountId);

    /// <summary>Unloads a Package</summary>
    public static bool UnloadPackage(string packageFolderName) =>
        ScriptApi.Invoke<bool>("Server", "UnloadPackage", packageFolderName)!;

    /// <summary>Called when the map changes</summary>
    public delegate void ChangeMapHandler(string? oldMap, string? newMap);

    /// <summary>Triggered when any package is loaded</summary>
    public delegate void PackageLoadHandler(string? packageName);

    /// <summary>Triggered when any package is unloaded</summary>
    public delegate void PackageUnloadHandler(string? packageName);

    /// <summary>Called when a player is attempting to connect to the server</summary>
    public delegate void PlayerConnectHandler(string? ip, string? playerAccountId, string? playerName, string? playerSteamId);

    /// <summary>Called when a player is disconnecting from the server</summary>
    public delegate void PlayerDisconnectHandler(string? ip, string? playerAccountId, string? playerName, string? playerSteamId, string? disconnectReason);

    /// <summary>Triggered when the server is restarting</summary>
    public delegate void RestartHandler();

    /// <summary>Triggered once when the server has started and all packages are loaded</summary>
    public delegate void StartHandler();

    /// <summary>Triggered when the server is stopping</summary>
    public delegate void StopHandler();

    /// <summary>Triggered every 33ms by default</summary>
    public delegate void TickHandler(float deltaTime);

    /// <summary>Triggered when a Value is changed with <c>Server.SetValue()</c></summary>
    public delegate void ValueChangeHandler(string? key, object? value);
}
