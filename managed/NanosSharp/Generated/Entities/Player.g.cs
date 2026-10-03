#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Players are Entities that represents the individual behind the mouse and keyboard. Players are spawned automatically when connected to the server.</summary>
[ScriptClass("Player")]
public partial class Player : Entity
{
    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public static event ClassRegisterHandler? OnClassRegister
    {
        add => ClassRegisterEvent.Add(value);
        remove => ClassRegisterEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is destroyed</summary>
    public static event DestroyHandler? OnDestroy
    {
        add => DestroyEvent.Add(value);
        remove => DestroyEvent.Remove(value);
    }

    /// <summary>Triggered when a Player changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
    }

    /// <summary>Triggered when Player starts controlling a Pawn (Character)</summary>
    public static event PossessHandler? OnPossess
    {
        add => PossessEvent.Add(value);
        remove => PossessEvent.Remove(value);
    }

    /// <summary>Triggered when Player is ready (the client fully joined, loaded the map and all entities and is ready to play)</summary>
    public static event ReadyHandler? OnReady
    {
        add => ReadyEvent.Add(value);
        remove => ReadyEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>A Pawn (Character) was unpossessed from the Player</summary>
    public static event UnPossessHandler? OnUnPossess
    {
        add => UnPossessEvent.Add(value);
        remove => UnPossessEvent.Remove(value);
    }

    /// <summary>Triggered when a global VOIP channel setting changes for this player</summary>
    public static event VOIPGlobalChannelSettingChangeHandler? OnVOIPGlobalChannelSettingChange
    {
        add => VOIPGlobalChannelSettingChangeEvent.Add(value);
        remove => VOIPGlobalChannelSettingChangeEvent.Remove(value);
    }

    /// <summary>Triggered when the local VOIP setting changes for this player</summary>
    public static event VOIPLocalSettingChangeHandler? OnVOIPLocalSettingChange
    {
        add => VOIPLocalSettingChangeEvent.Add(value);
        remove => VOIPLocalSettingChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Player", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Player", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Player", "DimensionChange");
    private static readonly ClassEvent<PossessHandler> PossessEvent = new("Player", "Possess");
    private static readonly ClassEvent<ReadyHandler> ReadyEvent = new("Player", "Ready");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Player", "Spawn");
    private static readonly ClassEvent<UnPossessHandler> UnPossessEvent = new("Player", "UnPossess");
    private static readonly ClassEvent<VOIPGlobalChannelSettingChangeHandler> VOIPGlobalChannelSettingChangeEvent = new("Player", "VOIPGlobalChannelSettingChange");
    private static readonly ClassEvent<VOIPLocalSettingChangeHandler> VOIPLocalSettingChangeEvent = new("Player", "VOIPLocalSettingChange");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Player", "ValueChange");
    private static readonly MethodSlot AttachCameraToSlot = new("AttachCameraTo");
    private static readonly MethodSlot BanSlot = new("Ban");
    private static readonly MethodSlot ConnectSlot = new("Connect");
    private static readonly MethodSlot GetAccountIDSlot = new("GetAccountID");
    private static readonly MethodSlot GetAccountIconURLSlot = new("GetAccountIconURL");
    private static readonly MethodSlot GetAccountNameSlot = new("GetAccountName");
    private static readonly MethodSlot GetControlledCharacterSlot = new("GetControlledCharacter");
    private static readonly MethodSlot GetDimensionSlot = new("GetDimension");
    private static readonly MethodSlot GetIPSlot = new("GetIP");
    private static readonly MethodSlot GetNameSlot = new("GetName");
    private static readonly MethodSlot GetPingSlot = new("GetPing");
    private static readonly MethodSlot GetSteamIDSlot = new("GetSteamID");
    private static readonly MethodSlot GetVOIPListeningChannelsSlot = new("GetVOIPListeningChannels");
    private static readonly MethodSlot GetVOIPSpeakingChannelsSlot = new("GetVOIPSpeakingChannels");
    private static readonly MethodSlot IsHostSlot = new("IsHost");
    private static readonly MethodSlot IsVOIPMutedSlot = new("IsVOIPMuted");
    private static readonly MethodSlot KickSlot = new("Kick");
    private static readonly MethodSlot PossessSlot = new("Possess");
    private static readonly MethodSlot ResetCameraSlot = new("ResetCamera");
    private static readonly MethodSlot RotateCameraToSlot = new("RotateCameraTo");
    private static readonly MethodSlot SetCameraArmLengthSlot = new("SetCameraArmLength");
    private static readonly MethodSlot SetCameraFOVSlot = new("SetCameraFOV");
    private static readonly MethodSlot SetCameraLocationSlot = new("SetCameraLocation");
    private static readonly MethodSlot SetCameraRotationSlot = new("SetCameraRotation");
    private static readonly MethodSlot SetCameraSocketOffsetSlot = new("SetCameraSocketOffset");
    private static readonly MethodSlot SetCameraSpeedSettingsSlot = new("SetCameraSpeedSettings");
    private static readonly MethodSlot SetDimensionSlot = new("SetDimension");
    private static readonly MethodSlot SetDistanceOptimizationMultiplierSlot = new("SetDistanceOptimizationMultiplier");
    private static readonly MethodSlot SetManualCameraFadeSlot = new("SetManualCameraFade");
    private static readonly MethodSlot SetNameSlot = new("SetName");
    private static readonly MethodSlot SetVOIPGlobalAllChannelsSettingSlot = new("SetVOIPGlobalAllChannelsSetting");
    private static readonly MethodSlot SetVOIPGlobalChannelSettingSlot = new("SetVOIPGlobalChannelSetting");
    private static readonly MethodSlot SetVOIPGlobalHighPassFilterSlot = new("SetVOIPGlobalHighPassFilter");
    private static readonly MethodSlot SetVOIPGlobalLowPassFilterSlot = new("SetVOIPGlobalLowPassFilter");
    private static readonly MethodSlot SetVOIPGlobalVolumeSlot = new("SetVOIPGlobalVolume");
    private static readonly MethodSlot SetVOIPLocalMaxDistanceSlot = new("SetVOIPLocalMaxDistance");
    private static readonly MethodSlot SetVOIPLocalSettingSlot = new("SetVOIPLocalSetting");
    private static readonly MethodSlot SetVOIPLocalVolumeSlot = new("SetVOIPLocalVolume");
    private static readonly MethodSlot SetVOIPMutedSlot = new("SetVOIPMuted");
    private static readonly MethodSlot SpectateSlot = new("Spectate");
    private static readonly MethodSlot StartCameraFadeSlot = new("StartCameraFade");
    private static readonly MethodSlot StopCameraFadeSlot = new("StopCameraFade");
    private static readonly MethodSlot TranslateCameraToSlot = new("TranslateCameraTo");
    private static readonly MethodSlot UnPossessSlot = new("UnPossess");

    /// <summary>Gets the Player's nanos world account ID</summary>
    public string AccountID => GetAccountID();

    /// <summary>Return a URL which can be used through WebUI and Widgets to display the Player's Steam Avatar (64x64)</summary>
    public string AccountIconURL => GetAccountIconURL();

    /// <summary>Gets the Player's nanos world account name</summary>
    public string AccountName => GetAccountName();

    /// <summary>Gets the Pawn (Character) this Player is currently possessing</summary>
    public object? ControlledCharacter => GetControlledCharacter();

    /// <summary>Gets this Player's dimension</summary>
    public uint Dimension
    {
        get => GetDimension();
        set => SetDimension(value);
    }

    /// <summary>Gets the Player's IP address</summary>
    public string IP => GetIP();

    /// <summary>Gets the Player's name</summary>
    public string Name
    {
        get => GetName();
        set => SetName(value);
    }

    /// <summary>Gets the Player's ping</summary>
    public int Ping => GetPing();

    /// <summary>Gets the Player's Steam ID</summary>
    public string SteamID => GetSteamID();

    /// <summary>Returns a list of VOIP channels this player is listening to</summary>
    public IReadOnlyList<uint> VOIPListeningChannels => GetVOIPListeningChannels();

    /// <summary>Returns a list of VOIP channels this player is currently speaking to</summary>
    public IReadOnlyList<uint> VOIPSpeakingChannels => GetVOIPSpeakingChannels();

    /// <summary>Gets if this Player is the host of a P2P (listen) server</summary>
    public bool Host => IsHost();

    /// <summary>Returns whether VOIP is muted for this player</summary>
    public bool VOIPMuted
    {
        get => IsVOIPMuted();
        set => SetVOIPMuted(value);
    }

    protected Player()
    {
    }

    /// <summary>Attaches the Player's Camera to an Actor</summary>
    public void AttachCameraTo(Entity actor, Vector? socketOffset = null, float? blendSpeed = null) =>
        Invoke(AttachCameraToSlot, ScriptArguments.Trim([actor, socketOffset, blendSpeed], 1));

    /// <summary>Bans the player from the server</summary>
    public void Ban(string reason) => Invoke(BanSlot, reason);

    /// <summary>Redirects the player to another server</summary>
    public void Connect(string ip, string? password = null) =>
        Invoke(ConnectSlot, ScriptArguments.Trim([ip, password], 1));

    /// <summary>Gets the Player's nanos world account ID</summary>
    public string GetAccountID() => Invoke<string>(GetAccountIDSlot)!;

    /// <summary>Return a URL which can be used through WebUI and Widgets to display the Player's Steam Avatar (64x64)</summary>
    public string GetAccountIconURL() => Invoke<string>(GetAccountIconURLSlot)!;

    /// <summary>Gets the Player's nanos world account name</summary>
    public string GetAccountName() => Invoke<string>(GetAccountNameSlot)!;

    /// <summary>Gets the Pawn (Character) this Player is currently possessing</summary>
    public object? GetControlledCharacter() => Invoke<object?>(GetControlledCharacterSlot);

    /// <summary>Gets this Player's dimension</summary>
    public uint GetDimension() => Invoke<uint>(GetDimensionSlot)!;

    /// <summary>Gets the Player's IP address</summary>
    public string GetIP() => Invoke<string>(GetIPSlot)!;

    /// <summary>Gets the Player's name</summary>
    public string GetName() => Invoke<string>(GetNameSlot)!;

    /// <summary>Gets the Player's ping</summary>
    public int GetPing() => Invoke<int>(GetPingSlot)!;

    /// <summary>Gets the Player's Steam ID</summary>
    public string GetSteamID() => Invoke<string>(GetSteamIDSlot)!;

    /// <summary>Returns a list of VOIP channels this player is listening to</summary>
    public IReadOnlyList<uint> GetVOIPListeningChannels() => Invoke<IReadOnlyList<uint>>(GetVOIPListeningChannelsSlot)!;

    /// <summary>Returns a list of VOIP channels this player is currently speaking to</summary>
    public IReadOnlyList<uint> GetVOIPSpeakingChannels() => Invoke<IReadOnlyList<uint>>(GetVOIPSpeakingChannelsSlot)!;

    /// <summary>Gets if this Player is the host of a P2P (listen) server</summary>
    public bool IsHost() => Invoke<bool>(IsHostSlot)!;

    /// <summary>Returns whether VOIP is muted for this player</summary>
    public bool IsVOIPMuted() => Invoke<bool>(IsVOIPMutedSlot)!;

    /// <summary>Kicks the player from the server</summary>
    public void Kick(string reason) => Invoke(KickSlot, reason);

    /// <summary>Makes this Player possess and control a Pawn (Character)</summary>
    public void Possess(Entity newPawn, float? blendTime = null, float? exp = null) =>
        Invoke(PossessSlot, ScriptArguments.Trim([newPawn, blendTime, exp], 1));

    /// <summary>Resets the Camera to default state (Unspectate and Detaches)</summary>
    public void ResetCamera() => Invoke(ResetCameraSlot);

    /// <summary>Smoothly moves the Player's Camera Rotation</summary>
    public void RotateCameraTo(Rotator rotation, float? time = null, float? exp = null) =>
        Invoke(RotateCameraToSlot, ScriptArguments.Trim([rotation, time, exp], 1));

    /// <summary>Sets the Player's Camera Arm Length (Spring Arm length)</summary>
    public void SetCameraArmLength(float length, bool? force = null) =>
        Invoke(SetCameraArmLengthSlot, ScriptArguments.Trim([length, force], 1));

    /// <summary>Sets the Player's Camera FOV (if not possessing a Character)</summary>
    public void SetCameraFOV(float? fov = null) => Invoke(SetCameraFOVSlot, ScriptArguments.Trim([fov], 0));

    /// <summary>Sets the Player's Camera Location (only works if not possessing any Character)</summary>
    public void SetCameraLocation(Vector location, float? argument2 = null, float? argument3 = null) =>
        Invoke(SetCameraLocationSlot, ScriptArguments.Trim([location, argument2, argument3], 1));

    /// <summary>Sets the Player's Camera Rotation</summary>
    public void SetCameraRotation(Rotator rotation, float? argument2 = null, float? argument3 = null) =>
        Invoke(SetCameraRotationSlot, ScriptArguments.Trim([rotation, argument2, argument3], 1));

    /// <summary>Sets the Player's Camera Socket Offset (Spring Arm Offset)</summary>
    public void SetCameraSocketOffset(Vector socketOffset) => Invoke(SetCameraSocketOffsetSlot, socketOffset);

    /// <summary>Sets the Player's Camera Speed Settings (if not possessing a Character)</summary>
    public void SetCameraSpeedSettings(
        float? maxSpeed = null,
        float? acceleration = null,
        float? deceleration = null,
        float? turningBoost = null) =>
        Invoke(SetCameraSpeedSettingsSlot, ScriptArguments.Trim([maxSpeed, acceleration, deceleration, turningBoost], 0));

    /// <summary>Sets this Player's dimension</summary>
    public void SetDimension(uint dimension) => Invoke(SetDimensionSlot, dimension);

    /// <summary>Sets the multiplier for how much this Player's is affected by distance optimization from other actors</summary>
    public void SetDistanceOptimizationMultiplier(float multiplier) =>
        Invoke(SetDistanceOptimizationMultiplierSlot, multiplier);

    /// <summary>Turns on camera fading at the given opacity</summary>
    public void SetManualCameraFade(float inFadeAmount, Color color, bool inFadeAudio) =>
        Invoke(SetManualCameraFadeSlot, inFadeAmount, color, inFadeAudio);

    /// <summary>Sets the player's name</summary>
    public void SetName(string playerName) => Invoke(SetNameSlot, playerName);

    /// <summary>Sets the VOIP setting for all global channels</summary>
    public void SetVOIPGlobalAllChannelsSetting(VOIPSetting setting) =>
        Invoke(SetVOIPGlobalAllChannelsSettingSlot, setting);

    /// <summary>Sets the VOIP setting for a specific global channel</summary>
    public void SetVOIPGlobalChannelSetting(byte channel, VOIPSetting setting) =>
        Invoke(SetVOIPGlobalChannelSettingSlot, channel, setting);

    /// <summary>Sets the global VOIP high-pass filter threshold frequency</summary>
    public void SetVOIPGlobalHighPassFilter(ushort threshold) => Invoke(SetVOIPGlobalHighPassFilterSlot, threshold);

    /// <summary>Sets the global VOIP low-pass filter threshold frequency</summary>
    public void SetVOIPGlobalLowPassFilter(ushort threshold) => Invoke(SetVOIPGlobalLowPassFilterSlot, threshold);

    /// <summary>Sets the global VOIP volume. It only affects the global channels</summary>
    public void SetVOIPGlobalVolume(float volume) => Invoke(SetVOIPGlobalVolumeSlot, volume);

    /// <summary>Sets the local/proximity VOIP max distance. This also affects networking usage</summary>
    public void SetVOIPLocalMaxDistance(ushort maxDistance) => Invoke(SetVOIPLocalMaxDistanceSlot, maxDistance);

    /// <summary>Sets the VOIP setting for the local/proximity</summary>
    public void SetVOIPLocalSetting(VOIPSetting setting) => Invoke(SetVOIPLocalSettingSlot, setting);

    /// <summary>Sets the local/proximity VOIP input/output volume. It only affects the local/proximity channel</summary>
    public void SetVOIPLocalVolume(float volume) => Invoke(SetVOIPLocalVolumeSlot, volume);

    /// <summary>Mutes or unmutes VOIP for this player</summary>
    public void SetVOIPMuted(bool isMuted) => Invoke(SetVOIPMutedSlot, isMuted);

    /// <summary>Makes this Player spectate another Player</summary>
    public void Spectate(Player player, float? blendSpeed = null) =>
        Invoke(SpectateSlot, ScriptArguments.Trim([player, blendSpeed], 1));

    /// <summary>Does a camera fade to/from a solid color</summary>
    public void StartCameraFade(
        float fromAlpha,
        float toAlpha,
        float duration,
        Color toColor,
        bool? shouldFadeAudio = null,
        bool? holdWhenFinished = null) =>
        Invoke(StartCameraFadeSlot, ScriptArguments.Trim([fromAlpha, toAlpha, duration, toColor, shouldFadeAudio, holdWhenFinished], 4));

    /// <summary>Stops camera fading.</summary>
    public void StopCameraFade() => Invoke(StopCameraFadeSlot);

    /// <summary>Smoothly moves the Player's Camera Location (only works if not possessing any Character)</summary>
    public void TranslateCameraTo(Vector location, float? time = null, float? exp = null) =>
        Invoke(TranslateCameraToSlot, ScriptArguments.Trim([location, time, exp], 1));

    /// <summary>Release the Player from the Character</summary>
    public void UnPossess() => Invoke(UnPossessSlot);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the Possess event of this entity only.</summary>
    public ScriptSubscription SubscribePossess(PossessHandler handler) => Subscribe("Possess", handler);

    /// <summary>Subscribes to the Ready event of this entity only.</summary>
    public ScriptSubscription SubscribeReady(ReadyHandler handler) => Subscribe("Ready", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the UnPossess event of this entity only.</summary>
    public ScriptSubscription SubscribeUnPossess(UnPossessHandler handler) => Subscribe("UnPossess", handler);

    /// <summary>Subscribes to the VOIPGlobalChannelSettingChange event of this entity only.</summary>
    public ScriptSubscription SubscribeVOIPGlobalChannelSettingChange(VOIPGlobalChannelSettingChangeHandler handler) => Subscribe("VOIPGlobalChannelSettingChange", handler);

    /// <summary>Subscribes to the VOIPLocalSettingChange event of this entity only.</summary>
    public ScriptSubscription SubscribeVOIPLocalSettingChange(VOIPLocalSettingChangeHandler handler) => Subscribe("VOIPLocalSettingChange", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Player self);

    /// <summary>Triggered when a Player changes its dimension</summary>
    public delegate void DimensionChangeHandler(Player self, int oldDimension, int newDimension);

    /// <summary>Triggered when Player starts controlling a Pawn (Character)</summary>
    public delegate void PossessHandler(Player self, Entity? pawn);

    /// <summary>Triggered when Player is ready (the client fully joined, loaded the map and all entities and is ready to play)</summary>
    public delegate void ReadyHandler(Player self);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Player self);

    /// <summary>A Pawn (Character) was unpossessed from the Player</summary>
    public delegate void UnPossessHandler(Player self, Entity? pawn);

    /// <summary>Triggered when a global VOIP channel setting changes for this player</summary>
    public delegate void VOIPGlobalChannelSettingChangeHandler(Player self, int channel, VOIPSetting oldSetting, VOIPSetting newSetting);

    /// <summary>Triggered when the local VOIP setting changes for this player</summary>
    public delegate void VOIPLocalSettingChangeHandler(Player self, VOIPSetting oldSetting, VOIPSetting newSetting);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Player self, string? key, object? value);
}
