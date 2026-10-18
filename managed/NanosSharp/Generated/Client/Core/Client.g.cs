#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Static Class present on Client side.</summary>
public static partial class Client
{
    /// <summary>Called when the Client language changes</summary>
    public static event LanguageChangeHandler? OnLanguageChange
    {
        add => LanguageChangeEvent.Add(value);
        remove => LanguageChangeEvent.Remove(value);
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

    /// <summary>Called when the local player spawns (just after the game has loaded)</summary>
    public static event SpawnLocalPlayerHandler? OnSpawnLocalPlayer
    {
        add => SpawnLocalPlayerEvent.Add(value);
        remove => SpawnLocalPlayerEvent.Remove(value);
    }

    /// <summary>Called Every Frame. Do not abuse</summary>
    public static event TickHandler? OnTick
    {
        add => TickEvent.Add(value);
        remove => TickEvent.Remove(value);
    }

    /// <summary>Called when the local player mutes or unmutes their own microphone, either with the VOIP key (on Open Mic) or from the escape menu</summary>
    public static event VOIPSelfMuteChangeHandler? OnVOIPSelfMuteChange
    {
        add => VOIPSelfMuteChangeEvent.Add(value);
        remove => VOIPSelfMuteChangeEvent.Remove(value);
    }

    /// <summary>Triggered when a Value is changed with <c>Client.SetValue()</c> or <c>Server.SetValue()</c> (for synced values)</summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    /// <summary>Called when the game is focused/unfocused</summary>
    public static event WindowFocusChangeHandler? OnWindowFocusChange
    {
        add => WindowFocusChangeEvent.Add(value);
        remove => WindowFocusChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<LanguageChangeHandler> LanguageChangeEvent = new("Client", "LanguageChange");
    private static readonly ClassEvent<PackageLoadHandler> PackageLoadEvent = new("Client", "PackageLoad");
    private static readonly ClassEvent<PackageUnloadHandler> PackageUnloadEvent = new("Client", "PackageUnload");
    private static readonly ClassEvent<SpawnLocalPlayerHandler> SpawnLocalPlayerEvent = new("Client", "SpawnLocalPlayer");
    private static readonly ClassEvent<TickHandler> TickEvent = new("Client", "Tick");
    private static readonly ClassEvent<VOIPSelfMuteChangeHandler> VOIPSelfMuteChangeEvent = new("Client", "VOIPSelfMuteChange");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Client", "ValueChange");
    private static readonly ClassEvent<WindowFocusChangeHandler> WindowFocusChangeEvent = new("Client", "WindowFocusChange");
    private static readonly StaticMethodSlot CopyToClipboardSlot = new("Client", "CopyToClipboard");
    private static readonly StaticMethodSlot DisconnectSlot = new("Client", "Disconnect");
    private static readonly StaticMethodSlot GetActorsInRadiusSlot = new("Client", "GetActorsInRadius");
    private static readonly StaticMethodSlot GetAllValuesKeysSlot = new("Client", "GetAllValuesKeys");
    private static readonly StaticMethodSlot GetConnectedServerIPSlot = new("Client", "GetConnectedServerIP");
    private static readonly StaticMethodSlot GetEntityByIDSlot = new("Client", "GetEntityByID");
    private static readonly StaticMethodSlot GetFrameTimeSlot = new("Client", "GetFrameTime");
    private static readonly StaticMethodSlot GetLanguageSlot = new("Client", "GetLanguage");
    private static readonly StaticMethodSlot GetLocalPlayerSlot = new("Client", "GetLocalPlayer");
    private static readonly StaticMethodSlot GetLogLevelSlot = new("Client", "GetLogLevel");
    private static readonly StaticMethodSlot GetMapSlot = new("Client", "GetMap");
    private static readonly StaticMethodSlot GetNearClipPlaneSlot = new("Client", "GetNearClipPlane");
    private static readonly StaticMethodSlot GetPackagesSlot = new("Client", "GetPackages");
    private static readonly StaticMethodSlot GetSettingsSlot = new("Client", "GetSettings");
    private static readonly StaticMethodSlot GetTimeSlot = new("Client", "GetTime");
    private static readonly StaticMethodSlot GetValueSlot = new("Client", "GetValue");
    private static readonly StaticMethodSlot IsPackageLoadedSlot = new("Client", "IsPackageLoaded");
    private static readonly StaticMethodSlot IsVOIPSelfMutedSlot = new("Client", "IsVOIPSelfMuted");
    private static readonly StaticMethodSlot OpenEscapeMenuSlot = new("Client", "OpenEscapeMenu");
    private static readonly StaticMethodSlot SetDebugEnabledSlot = new("Client", "SetDebugEnabled");
    private static readonly StaticMethodSlot SetEscapeMenuTextSlot = new("Client", "SetEscapeMenuText");
    private static readonly StaticMethodSlot SetHighlightColorSlot = new("Client", "SetHighlightColor");
    private static readonly StaticMethodSlot SetNearClipPlaneSlot = new("Client", "SetNearClipPlane");
    private static readonly StaticMethodSlot SetOutlineColorSlot = new("Client", "SetOutlineColor");
    private static readonly StaticMethodSlot SetSettingGammaOverrideSlot = new("Client", "SetSettingGammaOverride");
    private static readonly StaticMethodSlot SetValueSlot = new("Client", "SetValue");
    private static readonly StaticMethodSlot ShowNotificationSlot = new("Client", "ShowNotification");

    /// <summary>Gets a list of all values keys</summary>
    public static IReadOnlyList<string> AllValuesKeys => GetAllValuesKeys();

    /// <summary>Gets the current connected server IP:Port</summary>
    public static string ConnectedServerIP => GetConnectedServerIP();

    /// <summary>Gets the current Frame Time</summary>
    public static float FrameTime => GetFrameTime();

    /// <summary>Gets the current language of the Client</summary>
    public static string Language => GetLanguage();

    /// <summary>Gets the local Player</summary>
    public static Player? LocalPlayer => GetLocalPlayer();

    /// <summary>Returns the current log level</summary>
    public static int LogLevel => GetLogLevel();

    /// <summary>Returns the current map package name</summary>
    public static string Map => GetMap();

    /// <summary>Gets the Near Clip Plane value</summary>
    public static float NearClipPlane
    {
        get => GetNearClipPlane();
        set => SetNearClipPlane(value);
    }

    /// <summary>Gets the local player's game Settings</summary>
    public static IReadOnlyDictionary<string, object?> Settings => GetSettings();

    /// <summary>Gets the Unix Epoch Time in milliseconds</summary>
    public static ulong Time => GetTime();

    /// <summary>Gets whether the local player has muted their own microphone</summary>
    public static bool VOIPSelfMuted => IsVOIPSelfMuted();

    /// <summary>Copies a text to Clipboard</summary>
    public static void CopyToClipboard(string text) => ScriptApi.Call(CopyToClipboardSlot, 1, text);

    /// <summary>Disconnects from the server</summary>
    public static void Disconnect() => ScriptApi.Call(DisconnectSlot, 0);

    /// <summary>Returns the actors within the specified radius</summary>
    public static IReadOnlyList<object?> GetActorsInRadius(
        Vector location,
        float radius,
        IEnumerable<string>? onlyClasses = null) =>
        ScriptApi.CallResult<IReadOnlyList<object?>, Vector, float, IEnumerable<string>?>(GetActorsInRadiusSlot, 2, location, radius, onlyClasses)!;

    /// <summary>Gets a list of all values keys</summary>
    public static IReadOnlyList<string> GetAllValuesKeys() =>
        ScriptApi.CallResult<IReadOnlyList<string>>(GetAllValuesKeysSlot, 0)!;

    /// <summary>Gets the current connected server IP:Port</summary>
    public static string GetConnectedServerIP() => ScriptApi.CallResult<string>(GetConnectedServerIPSlot, 0)!;

    /// <summary>Returns the entity with the specified ID</summary>
    public static object? GetEntityByID(uint entityId) =>
        ScriptApi.CallResult<object?, uint>(GetEntityByIDSlot, 1, entityId);

    /// <summary>Gets the current Frame Time</summary>
    public static float GetFrameTime() => ScriptApi.CallResult<float>(GetFrameTimeSlot, 0)!;

    /// <summary>Gets the current language of the Client</summary>
    public static string GetLanguage() => ScriptApi.CallResult<string>(GetLanguageSlot, 0)!;

    /// <summary>Gets the local Player</summary>
    public static Player? GetLocalPlayer() => ScriptApi.CallResult<Player?>(GetLocalPlayerSlot, 0);

    /// <summary>Returns the current log level</summary>
    public static int GetLogLevel() => ScriptApi.CallResult<int>(GetLogLevelSlot, 0)!;

    /// <summary>Returns the current map package name</summary>
    public static string GetMap() => ScriptApi.CallResult<string>(GetMapSlot, 0)!;

    /// <summary>Gets the Near Clip Plane value</summary>
    public static float GetNearClipPlane() => ScriptApi.CallResult<float>(GetNearClipPlaneSlot, 0)!;

    /// <summary>Returns a list of Packages running</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetPackages(
        PackageType? packageTypeFilter = null) =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>, PackageType?>(GetPackagesSlot, 0, packageTypeFilter)!;

    /// <summary>Gets the local player's game Settings</summary>
    public static IReadOnlyDictionary<string, object?> GetSettings() =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>>(GetSettingsSlot, 0)!;

    /// <summary>Gets the Unix Epoch Time in milliseconds</summary>
    public static ulong GetTime() => ScriptApi.CallResult<ulong>(GetTimeSlot, 0)!;

    /// <summary>Gets a value given a key</summary>
    public static object? GetValue(string key, object? fallback = null) =>
        ScriptApi.CallResult<object?, string, object?>(GetValueSlot, 1, key, fallback);

    /// <summary>Returns if a Package is currently loaded</summary>
    public static bool IsPackageLoaded(string packageName) =>
        ScriptApi.CallResult<bool, string>(IsPackageLoadedSlot, 1, packageName)!;

    /// <summary>Gets whether the local player has muted their own microphone</summary>
    public static bool IsVOIPSelfMuted() => ScriptApi.CallResult<bool>(IsVOIPSelfMutedSlot, 0)!;

    /// <summary>Opens the escape menu</summary>
    public static void OpenEscapeMenu(bool? openSettings = null) => ScriptApi.Call(OpenEscapeMenuSlot, 0, openSettings);

    /// <summary>Enables/Disables Debug settings and Client Console to be used</summary>
    public static void SetDebugEnabled(bool enableDebug) => ScriptApi.Call(SetDebugEnabledSlot, 1, enableDebug);

    /// <summary>Sets the Escape Menu text</summary>
    public static void SetEscapeMenuText(string text) => ScriptApi.Call(SetEscapeMenuTextSlot, 1, text);

    /// <summary>Changes the Highlight Color for highlighted actors at a specific Index</summary>
    public static void SetHighlightColor(Color highlightColor, uint? index = null, HighlightMode? mode = null) =>
        ScriptApi.Call(SetHighlightColorSlot, 1, highlightColor, index, mode);

    /// <summary>Sets the Near Clip Plane</summary>
    public static void SetNearClipPlane(float nearClipPlane) => ScriptApi.Call(SetNearClipPlaneSlot, 1, nearClipPlane);

    /// <summary>Changes the Outline Color for outlined actors at a specific Index</summary>
    public static void SetOutlineColor(Color outlineColor, uint? index = null, float? thickness = null) =>
        ScriptApi.Call(SetOutlineColorSlot, 1, outlineColor, index, thickness);

    /// <summary>Overrides the gamma setting and prevents the client from changing it</summary>
    public static void SetSettingGammaOverride(bool @override, float? newGamma = null) =>
        ScriptApi.Call(SetSettingGammaOverrideSlot, 1, @override, newGamma);

    /// <summary>Sets a global value in the Client, which can be accessed from anywhere (client side)</summary>
    public static void SetValue(string key, object? value) => ScriptApi.Call(SetValueSlot, 2, key, value);

    /// <summary>Shows a notification using the built-in notification system.</summary>
    public static void ShowNotification(
        string text,
        NotificationType? notificationType = null,
        bool? addToNotificationList = null,
        float? duration = null) =>
        ScriptApi.Call(ShowNotificationSlot, 1, text, notificationType, addToNotificationList, duration);

    /// <summary>Called when the Client language changes</summary>
    public delegate void LanguageChangeHandler(string? language);

    /// <summary>Triggered when any package is loaded</summary>
    public delegate void PackageLoadHandler(string? packageName);

    /// <summary>Triggered when any package is unloaded</summary>
    public delegate void PackageUnloadHandler(string? packageName);

    /// <summary>Called when the local player spawns (just after the game has loaded)</summary>
    public delegate void SpawnLocalPlayerHandler(Player? localPlayer);

    /// <summary>Called Every Frame. Do not abuse</summary>
    public delegate void TickHandler(float deltaTime);

    /// <summary>Called when the local player mutes or unmutes their own microphone, either with the VOIP key (on Open Mic) or from the escape menu</summary>
    public delegate void VOIPSelfMuteChangeHandler(bool isMuted);

    /// <summary>Triggered when a Value is changed with <c>Client.SetValue()</c> or <c>Server.SetValue()</c> (for synced values)</summary>
    public delegate void ValueChangeHandler(string? key, object? value);

    /// <summary>Called when the game is focused/unfocused</summary>
    public delegate void WindowFocusChangeHandler(bool isFocused);
}
