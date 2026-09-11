using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>The nanos world server.</summary>
public static class Server
{
    /// <summary>Server name.</summary>
    public static string Name
    {
        get => Invoke<string>("GetName") ?? string.Empty;
        set => Invoke("SetName", value);
    }

    /// <summary>Server description.</summary>
    public static string Description
    {
        get => Invoke<string>("GetDescription") ?? string.Empty;
        set => Invoke("SetDescription", value);
    }

    /// <summary>Maximum amount of players.</summary>
    public static int MaxPlayers
    {
        get => Invoke<int>("GetMaxPlayers");
        set => Invoke("SetMaxPlayers", value);
    }

    /// <summary>Current map.</summary>
    public static string Map => Invoke<string>("GetMap") ?? string.Empty;

    /// <summary>Game version of the server.</summary>
    public static string Version => Invoke<string>("GetVersion") ?? string.Empty;

    /// <summary>Server tick rate in Hz.</summary>
    public static int TickRate => Invoke<int>("GetTickRate");

    /// <summary>Server time in milliseconds.</summary>
    public static ulong Time => Invoke<ulong>("GetTime");

    /// <summary>Settings from the custom_settings section of Config.toml.</summary>
    public static IReadOnlyDictionary<string, object?> CustomSettings =>
        Invoke<Dictionary<string, object?>>("GetCustomSettings") ?? [];

    /// <summary>Raised every server tick with the elapsed seconds.</summary>
    public static event Action<float>? Tick
    {
        add => TickEvent.Add(value);
        remove => TickEvent.Remove(value);
    }

    /// <summary>Raised when the server started.</summary>
    public static event Action? Started
    {
        add => StartEvent.Add(value);
        remove => StartEvent.Remove(value);
    }

    /// <summary>Raised when the server is stopping.</summary>
    public static event Action? Stopping
    {
        add => StopEvent.Add(value);
        remove => StopEvent.Remove(value);
    }

    private const string ClassName = "Server";
    private static readonly ClassEvent<Action<float>> TickEvent = new(ClassName, "Tick");
    private static readonly ClassEvent<Action> StartEvent = new(ClassName, "Start");
    private static readonly ClassEvent<Action> StopEvent = new(ClassName, "Stop");

    /// <summary>Lists all packages known to the server.</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetPackages(bool onlyLoaded = true) =>
        Invoke<List<Dictionary<string, object?>>>("GetPackages", onlyLoaded) ?? [];

    /// <summary>Whether a package is loaded.</summary>
    public static bool IsPackageLoaded(string package) => Invoke<bool>("IsPackageLoaded", package);

    /// <summary>Loads a package.</summary>
    public static bool LoadPackage(string package) => Invoke<bool>("LoadPackage", package);

    /// <summary>Reloads a package.</summary>
    public static bool ReloadPackage(string package) => Invoke<bool>("ReloadPackage", package);

    /// <summary>Unloads a package.</summary>
    public static bool UnloadPackage(string package) => Invoke<bool>("UnloadPackage", package);

    /// <summary>Changes the map, which restarts the scripting engine.</summary>
    public static void ChangeMap(string map) => Invoke("ChangeMap", map);

    /// <summary>Stops the server.</summary>
    public static void Stop() => Invoke("Stop");

    /// <summary>Reads a global server value.</summary>
    public static T? GetValue<T>(string key, T? fallback = default) => Invoke<T>("GetValue", key, fallback);

    /// <summary>Stores a global server value, optionally synchronized to clients.</summary>
    public static void SetValue(string key, object? value, bool synchronize = false) =>
        Invoke("SetValue", key, value, synchronize);

    private static object? Invoke(string function, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.Invoke(ClassName, function, arguments);

    private static T? Invoke<T>(string function, params ReadOnlySpan<object?> arguments) =>
        ScriptApi.Invoke<T>(ClassName, function, arguments);
}
