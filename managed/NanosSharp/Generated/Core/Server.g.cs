#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
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
    private static readonly StaticMethodSlot AddMapSpawnPointSlot = new("Server", "AddMapSpawnPoint");
    private static readonly StaticMethodSlot BanByAccountIDSlot = new("Server", "BanByAccountID");
    private static readonly StaticMethodSlot ChangeMapSlot = new("Server", "ChangeMap");
    private static readonly StaticMethodSlot GetActorsInRadiusSlot = new("Server", "GetActorsInRadius");
    private static readonly StaticMethodSlot GetAllValuesKeysSlot = new("Server", "GetAllValuesKeys");
    private static readonly StaticMethodSlot GetConnectionCountSlot = new("Server", "GetConnectionCount");
    private static readonly StaticMethodSlot GetCustomSettingsSlot = new("Server", "GetCustomSettings");
    private static readonly StaticMethodSlot GetDefaultEntityDimensionSlot = new("Server", "GetDefaultEntityDimension");
    private static readonly StaticMethodSlot GetDefaultPlayerDimensionSlot = new("Server", "GetDefaultPlayerDimension");
    private static readonly StaticMethodSlot GetDescriptionSlot = new("Server", "GetDescription");
    private static readonly StaticMethodSlot GetEntityByIDSlot = new("Server", "GetEntityByID");
    private static readonly StaticMethodSlot GetIPSlot = new("Server", "GetIP");
    private static readonly StaticMethodSlot GetLogLevelSlot = new("Server", "GetLogLevel");
    private static readonly StaticMethodSlot GetMapSlot = new("Server", "GetMap");
    private static readonly StaticMethodSlot GetMapAssetSlot = new("Server", "GetMapAsset");
    private static readonly StaticMethodSlot GetMapConfigSlot = new("Server", "GetMapConfig");
    private static readonly StaticMethodSlot GetMapSpawnPointsSlot = new("Server", "GetMapSpawnPoints");
    private static readonly StaticMethodSlot GetMaxPlayersSlot = new("Server", "GetMaxPlayers");
    private static readonly StaticMethodSlot GetNameSlot = new("Server", "GetName");
    private static readonly StaticMethodSlot GetPackagesSlot = new("Server", "GetPackages");
    private static readonly StaticMethodSlot GetPlayersInRadiusSlot = new("Server", "GetPlayersInRadius");
    private static readonly StaticMethodSlot GetPortSlot = new("Server", "GetPort");
    private static readonly StaticMethodSlot GetQueryPortSlot = new("Server", "GetQueryPort");
    private static readonly StaticMethodSlot GetTickRateSlot = new("Server", "GetTickRate");
    private static readonly StaticMethodSlot GetTimeSlot = new("Server", "GetTime");
    private static readonly StaticMethodSlot GetValueSlot = new("Server", "GetValue");
    private static readonly StaticMethodSlot GetVersionSlot = new("Server", "GetVersion");
    private static readonly StaticMethodSlot IsAnnouncedSlot = new("Server", "IsAnnounced");
    private static readonly StaticMethodSlot IsPackageLoadedSlot = new("Server", "IsPackageLoaded");
    private static readonly StaticMethodSlot IsUnsafeLibsEnabledSlot = new("Server", "IsUnsafeLibsEnabled");
    private static readonly StaticMethodSlot KickByAccountIDSlot = new("Server", "KickByAccountID");
    private static readonly StaticMethodSlot LoadPackageSlot = new("Server", "LoadPackage");
    private static readonly StaticMethodSlot ReloadPackageSlot = new("Server", "ReloadPackage");
    private static readonly StaticMethodSlot RestartSlot = new("Server", "Restart");
    private static readonly StaticMethodSlot SetDefaultEntityDimensionSlot = new("Server", "SetDefaultEntityDimension");
    private static readonly StaticMethodSlot SetDefaultPlayerDimensionSlot = new("Server", "SetDefaultPlayerDimension");
    private static readonly StaticMethodSlot SetDescriptionSlot = new("Server", "SetDescription");
    private static readonly StaticMethodSlot SetLogoSlot = new("Server", "SetLogo");
    private static readonly StaticMethodSlot SetMaxPlayersSlot = new("Server", "SetMaxPlayers");
    private static readonly StaticMethodSlot SetNameSlot = new("Server", "SetName");
    private static readonly StaticMethodSlot SetPasswordSlot = new("Server", "SetPassword");
    private static readonly StaticMethodSlot SetValueSlot = new("Server", "SetValue");
    private static readonly StaticMethodSlot StopSlot = new("Server", "Stop");
    private static readonly StaticMethodSlot UnbanSlot = new("Server", "Unban");
    private static readonly StaticMethodSlot UnloadPackageSlot = new("Server", "UnloadPackage");

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
        ScriptApi.Call(AddMapSpawnPointSlot, 1, location, rotation);

    /// <summary>Ban the player from the server by its Account ID, ideal for banning from inside PlayerConnect event</summary>
    public static void BanByAccountID(string playerAccountId, string reason) =>
        ScriptApi.Call(BanByAccountIDSlot, 2, playerAccountId, reason);

    /// <summary>Restarts the server in a new Map, restarts all packages and reconnects all players</summary>
    public static void ChangeMap(string mapPath) => ScriptApi.Call(ChangeMapSlot, 1, mapPath);

    /// <summary>Returns the actors within the specified radius</summary>
    public static IReadOnlyList<object?> GetActorsInRadius(
        Vector location,
        float radius,
        IEnumerable<string>? onlyClasses = null,
        uint? dimension = null) =>
        ScriptApi.CallResult<IReadOnlyList<object?>, Vector, float, IEnumerable<string>?, uint?>(GetActorsInRadiusSlot, 2, location, radius, onlyClasses, dimension)!;

    /// <summary>Gets a list of all values keys</summary>
    public static IReadOnlyList<string> GetAllValuesKeys() =>
        ScriptApi.CallResult<IReadOnlyList<string>>(GetAllValuesKeysSlot, 0)!;

    /// <summary>Returns the number of connected players (including players in the process of connecting)</summary>
    public static int GetConnectionCount() => ScriptApi.CallResult<int>(GetConnectionCountSlot, 0)!;

    /// <summary>Returns the Server Custom Settings passed as parameter or New Game screen</summary>
    public static IReadOnlyDictionary<string, object?> GetCustomSettings() =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>>(GetCustomSettingsSlot, 0)!;

    /// <summary>Returns the default entity dimension</summary>
    public static uint GetDefaultEntityDimension() => ScriptApi.CallResult<uint>(GetDefaultEntityDimensionSlot, 0)!;

    /// <summary>Returns the default player dimension</summary>
    public static uint GetDefaultPlayerDimension() => ScriptApi.CallResult<uint>(GetDefaultPlayerDimensionSlot, 0)!;

    /// <summary>Returns the Server Description</summary>
    public static string GetDescription() => ScriptApi.CallResult<string>(GetDescriptionSlot, 0)!;

    /// <summary>Returns the entity with the specified ID</summary>
    public static object? GetEntityByID(uint entityId) =>
        ScriptApi.CallResult<object?, uint>(GetEntityByIDSlot, 1, entityId);

    /// <summary>Returns the server Public IP</summary>
    public static string GetIP() => ScriptApi.CallResult<string>(GetIPSlot, 0)!;

    /// <summary>Returns the current log level</summary>
    public static int GetLogLevel() => ScriptApi.CallResult<int>(GetLogLevelSlot, 0)!;

    /// <summary>Returns the current Map Package name</summary>
    public static string GetMap() => ScriptApi.CallResult<string>(GetMapSlot, 0)!;

    /// <summary>Returns the current Map Asset reference</summary>
    public static string GetMapAsset() => ScriptApi.CallResult<string>(GetMapAssetSlot, 0)!;

    /// <summary>Returns a list of the Custom Config of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyDictionary<string, object?> GetMapConfig() =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>>(GetMapConfigSlot, 0)!;

    /// <summary>Returns a list of the Spawn Points of the current map (defined in the map package's Package.toml)</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetMapSpawnPoints() =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>>(GetMapSpawnPointsSlot, 0)!;

    /// <summary>Returns the Max Players configured</summary>
    public static int GetMaxPlayers() => ScriptApi.CallResult<int>(GetMaxPlayersSlot, 0)!;

    /// <summary>Returns the Server Name</summary>
    public static string GetName() => ScriptApi.CallResult<string>(GetNameSlot, 0)!;

    /// <summary>Returns a list of Packages running, optionally returns all Packages installed in the server</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetPackages(
        bool? onlyLoaded = null,
        PackageType? packageTypeFilter = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, bool?, PackageType?>(GetPackagesSlot, 0, onlyLoaded, packageTypeFilter)!;

    /// <summary>Returns the players within the specified radius</summary>
    public static IReadOnlyList<object?> GetPlayersInRadius(Vector location, float radius, uint? dimension = null) =>
        ScriptApi.CallResult<IReadOnlyList<object?>, Vector, float, uint?>(GetPlayersInRadiusSlot, 2, location, radius, dimension)!;

    /// <summary>Returns the server Port</summary>
    public static int GetPort() => ScriptApi.CallResult<int>(GetPortSlot, 0)!;

    /// <summary>Returns the server QueryPort</summary>
    public static int GetQueryPort() => ScriptApi.CallResult<int>(GetQueryPortSlot, 0)!;

    /// <summary>Returns the Configured Tick Rate</summary>
    public static int GetTickRate() => ScriptApi.CallResult<int>(GetTickRateSlot, 0)!;

    /// <summary>Gets the Unix Epoch Time in milliseconds</summary>
    public static ulong GetTime() => ScriptApi.CallResult<ulong>(GetTimeSlot, 0)!;

    /// <summary>Gets a value given a key</summary>
    public static object? GetValue(string key, object? fallback = null) =>
        ScriptApi.CallResult<object?, string, object?>(GetValueSlot, 1, key, fallback);

    /// <summary>Returns the server version</summary>
    public static string GetVersion() => ScriptApi.CallResult<string>(GetVersionSlot, 0)!;

    /// <summary>Returns if the Server is announced in the Master List</summary>
    public static bool IsAnnounced() => ScriptApi.CallResult<bool>(IsAnnouncedSlot, 0)!;

    /// <summary>Returns if a Package is currently loaded</summary>
    public static bool IsPackageLoaded(string packageName) =>
        ScriptApi.CallResult<bool, string>(IsPackageLoadedSlot, 1, packageName)!;

    /// <summary>Returns if the server was started with --enable_unsafe_libs</summary>
    public static int IsUnsafeLibsEnabled() => ScriptApi.CallResult<int>(IsUnsafeLibsEnabledSlot, 0)!;

    /// <summary>Kicks the player from the server by its Account ID, ideal for kicking from inside PlayerConnect event</summary>
    public static void KickByAccountID(string playerAccountId, string reason) =>
        ScriptApi.Call(KickByAccountIDSlot, 2, playerAccountId, reason);

    /// <summary>Loads a Package</summary>
    public static bool LoadPackage(string packageFolderName) =>
        ScriptApi.CallResult<bool, string>(LoadPackageSlot, 1, packageFolderName)!;

    /// <summary>Reloads a Package on the next server tick</summary>
    public static bool ReloadPackage(string packageFolderName) =>
        ScriptApi.CallResult<bool, string>(ReloadPackageSlot, 1, packageFolderName)!;

    /// <summary>Restarts the server, reloading all Packages and reconnecting all Players</summary>
    public static void Restart() => ScriptApi.Call(RestartSlot, 0);

    /// <summary>Sets the default server dimension that entities will spawn in</summary>
    public static void SetDefaultEntityDimension(uint defaultDimension) =>
        ScriptApi.Call(SetDefaultEntityDimensionSlot, 1, defaultDimension);

    /// <summary>Sets the default server dimension that players will spawn in</summary>
    public static void SetDefaultPlayerDimension(uint defaultDimension) =>
        ScriptApi.Call(SetDefaultPlayerDimensionSlot, 1, defaultDimension);

    /// <summary>Sets the description of the server, optionally overrides Config.toml</summary>
    public static void SetDescription(string description, bool? persistToConfigFile = null) =>
        ScriptApi.Call(SetDescriptionSlot, 1, description, persistToConfigFile);

    /// <summary>Sets the logo of the server, optionally overrides Config.toml</summary>
    public static void SetLogo(string logoUrl, bool? persistToConfigFile = null) =>
        ScriptApi.Call(SetLogoSlot, 1, logoUrl, persistToConfigFile);

    /// <summary>Sets the maximum player slots of the server, optionally overrides Config.toml</summary>
    public static void SetMaxPlayers(uint maxPlayers, bool? persistToConfigFile = null) =>
        ScriptApi.Call(SetMaxPlayersSlot, 1, maxPlayers, persistToConfigFile);

    /// <summary>Sets the name of the server, optionally overrides Config.toml</summary>
    public static void SetName(string name, bool? persistToConfigFile = null) =>
        ScriptApi.Call(SetNameSlot, 1, name, persistToConfigFile);

    /// <summary>Sets the password of the server, optionally overrides Config.toml</summary>
    public static void SetPassword(string password, bool? persistToConfigFile = null) =>
        ScriptApi.Call(SetPasswordSlot, 1, password, persistToConfigFile);

    /// <summary>Sets a global value in the Server, which can be accessed from anywhere (server side)</summary>
    public static void SetValue(string key, object? value, bool? syncOnClient = null) =>
        ScriptApi.Call(SetValueSlot, 2, key, value, syncOnClient);

    /// <summary>Stops the server</summary>
    public static void Stop() => ScriptApi.Call(StopSlot, 0);

    /// <summary>Unbans a Player by its account ID</summary>
    public static void Unban(string accountId) => ScriptApi.Call(UnbanSlot, 1, accountId);

    /// <summary>Unloads a Package</summary>
    public static bool UnloadPackage(string packageFolderName) =>
        ScriptApi.CallResult<bool, string>(UnloadPackageSlot, 1, packageFolderName)!;

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
