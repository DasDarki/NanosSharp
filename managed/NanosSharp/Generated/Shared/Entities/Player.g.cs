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

    /// <summary>Triggered when Player starts controlling a Pawn (Character)</summary>
    public static event PossessHandler? OnPossess
    {
        add => PossessEvent.Add(value);
        remove => PossessEvent.Remove(value);
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
    private static readonly ClassEvent<PossessHandler> PossessEvent = new("Player", "Possess");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Player", "Spawn");
    private static readonly ClassEvent<UnPossessHandler> UnPossessEvent = new("Player", "UnPossess");
    private static readonly ClassEvent<VOIPGlobalChannelSettingChangeHandler> VOIPGlobalChannelSettingChangeEvent = new("Player", "VOIPGlobalChannelSettingChange");
    private static readonly ClassEvent<VOIPLocalSettingChangeHandler> VOIPLocalSettingChangeEvent = new("Player", "VOIPLocalSettingChange");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Player", "ValueChange");
    private static readonly MethodSlot AttachCameraToSlot = new("AttachCameraTo");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot GetAccountIDSlot = new("GetAccountID");
    private static readonly MethodSlot GetAccountIconURLSlot = new("GetAccountIconURL");
    private static readonly MethodSlot GetAccountNameSlot = new("GetAccountName");
    private static readonly MethodSlot GetControlledCharacterSlot = new("GetControlledCharacter");
    private static readonly MethodSlot GetDimensionSlot = new("GetDimension");
    private static readonly MethodSlot GetNameSlot = new("GetName");
    private static readonly MethodSlot GetPingSlot = new("GetPing");
    private static readonly MethodSlot GetSteamIDSlot = new("GetSteamID");
    private static readonly MethodSlot GetVOIPChannelSlot = new("GetVOIPChannel");
    private static readonly MethodSlot GetVOIPListeningChannelsSlot = new("GetVOIPListeningChannels");
    private static readonly MethodSlot GetVOIPSettingSlot = new("GetVOIPSetting");
    private static readonly MethodSlot GetVOIPSpeakingChannelsSlot = new("GetVOIPSpeakingChannels");
    private static readonly MethodSlot IsVOIPMutedSlot = new("IsVOIPMuted");
    private static readonly MethodSlot ResetCameraSlot = new("ResetCamera");
    private static readonly MethodSlot RotateCameraToSlot = new("RotateCameraTo");
    private static readonly MethodSlot SetCameraArmLengthSlot = new("SetCameraArmLength");
    private static readonly MethodSlot SetCameraFOVSlot = new("SetCameraFOV");
    private static readonly MethodSlot SetCameraLocationSlot = new("SetCameraLocation");
    private static readonly MethodSlot SetCameraRotationSlot = new("SetCameraRotation");
    private static readonly MethodSlot SetCameraSocketOffsetSlot = new("SetCameraSocketOffset");
    private static readonly MethodSlot SetCameraSpeedSettingsSlot = new("SetCameraSpeedSettings");
    private static readonly MethodSlot SetManualCameraFadeSlot = new("SetManualCameraFade");
    private static readonly MethodSlot SetVOIPGlobalHighPassFilterSlot = new("SetVOIPGlobalHighPassFilter");
    private static readonly MethodSlot SetVOIPGlobalLowPassFilterSlot = new("SetVOIPGlobalLowPassFilter");
    private static readonly MethodSlot SetVOIPGlobalVolumeSlot = new("SetVOIPGlobalVolume");
    private static readonly MethodSlot SetVOIPLocalMaxDistanceSlot = new("SetVOIPLocalMaxDistance");
    private static readonly MethodSlot SetVOIPLocalVolumeSlot = new("SetVOIPLocalVolume");
    private static readonly MethodSlot SetVOIPMutedSlot = new("SetVOIPMuted");
    private static readonly MethodSlot SetVOIPSettingSlot = new("SetVOIPSetting");
    private static readonly MethodSlot SetVOIPVolumeSlot = new("SetVOIPVolume");
    private static readonly MethodSlot SpectateSlot = new("Spectate");
    private static readonly MethodSlot StartCameraFadeSlot = new("StartCameraFade");
    private static readonly MethodSlot StopCameraFadeSlot = new("StopCameraFade");
    private static readonly MethodSlot TranslateCameraToSlot = new("TranslateCameraTo");

    /// <summary>Gets the Player's nanos world account ID</summary>
    public string AccountID => GetAccountID();

    /// <summary>Return a URL which can be used through WebUI and Widgets to display the Player's Steam Avatar (64x64)</summary>
    public string AccountIconURL => GetAccountIconURL();

    /// <summary>Gets the Player's nanos world account name</summary>
    public string AccountName => GetAccountName();

    /// <summary>Gets the Pawn (Character) this Player is currently possessing</summary>
    public object? ControlledCharacter => GetControlledCharacter();

    /// <summary>Gets the Player's ping</summary>
    public int Ping => GetPing();

    /// <summary>Gets the Player's Steam ID</summary>
    public string SteamID => GetSteamID();

    /// <summary>Returns a list of VOIP channels this player is listening to</summary>
    public IReadOnlyList<uint> VOIPListeningChannels => GetVOIPListeningChannels();

    /// <summary>Gets the voice setting of the Player</summary>
    public VOIPSetting VOIPSetting
    {
        get => GetVOIPSetting();
        set => SetVOIPSetting(value);
    }

    /// <summary>Returns a list of VOIP channels this player is currently speaking to</summary>
    public IReadOnlyList<uint> VOIPSpeakingChannels => GetVOIPSpeakingChannels();

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
        Call(AttachCameraToSlot, 1, actor, socketOffset, blendSpeed);

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Gets the Player's nanos world account ID</summary>
    public string GetAccountID() => CallResult<string>(GetAccountIDSlot, 0)!;

    /// <summary>Return a URL which can be used through WebUI and Widgets to display the Player's Steam Avatar (64x64)</summary>
    public string GetAccountIconURL() => CallResult<string>(GetAccountIconURLSlot, 0)!;

    /// <summary>Gets the Player's nanos world account name</summary>
    public string GetAccountName() => CallResult<string>(GetAccountNameSlot, 0)!;

    /// <summary>Gets the Pawn (Character) this Player is currently possessing</summary>
    public object? GetControlledCharacter() => CallResult<object?>(GetControlledCharacterSlot, 0);

    /// <summary>Gets this Player's dimension</summary>
    public uint GetDimension() => CallResult<uint>(GetDimensionSlot, 0)!;

    /// <summary>Gets the Player's name</summary>
    public string GetName() => CallResult<string>(GetNameSlot, 0)!;

    /// <summary>Gets the Player's ping</summary>
    public int GetPing() => CallResult<int>(GetPingSlot, 0)!;

    /// <summary>Gets the Player's Steam ID</summary>
    public string GetSteamID() => CallResult<string>(GetSteamIDSlot, 0)!;

    /// <summary>Gets the voice channel of the Player</summary>
    public uint GetVOIPChannel() => CallResult<uint>(GetVOIPChannelSlot, 0)!;

    /// <summary>Returns a list of VOIP channels this player is listening to</summary>
    public IReadOnlyList<uint> GetVOIPListeningChannels() =>
        CallResult<IReadOnlyList<uint>>(GetVOIPListeningChannelsSlot, 0)!;

    /// <summary>Gets the voice setting of the Player</summary>
    public VOIPSetting GetVOIPSetting() => CallResult<VOIPSetting>(GetVOIPSettingSlot, 0)!;

    /// <summary>Returns a list of VOIP channels this player is currently speaking to</summary>
    public IReadOnlyList<uint> GetVOIPSpeakingChannels() =>
        CallResult<IReadOnlyList<uint>>(GetVOIPSpeakingChannelsSlot, 0)!;

    /// <summary>Returns whether VOIP is muted for this player</summary>
    public bool IsVOIPMuted() => CallResult<bool>(IsVOIPMutedSlot, 0)!;

    /// <summary>Resets the Camera to default state (Unspectate and Detaches)</summary>
    public void ResetCamera() => Call(ResetCameraSlot, 0);

    /// <summary>Smoothly moves the Player's Camera Rotation</summary>
    public void RotateCameraTo(Rotator rotation, float? time = null, float? exp = null) =>
        Call(RotateCameraToSlot, 1, rotation, time, exp);

    /// <summary>Sets the Player's Camera Arm Length (Spring Arm length)</summary>
    public void SetCameraArmLength(float length, bool? force = null) => Call(SetCameraArmLengthSlot, 1, length, force);

    /// <summary>Sets the Player's Camera FOV (if not possessing a Character)</summary>
    public void SetCameraFOV(float? fov = null) => Call(SetCameraFOVSlot, 0, fov);

    /// <summary>Sets the Player's Camera Location (only works if not possessing any Character)</summary>
    public void SetCameraLocation(Vector location, float? argument2 = null, float? argument3 = null) =>
        Call(SetCameraLocationSlot, 1, location, argument2, argument3);

    /// <summary>Sets the Player's Camera Rotation</summary>
    public void SetCameraRotation(Rotator rotation, float? argument2 = null, float? argument3 = null) =>
        Call(SetCameraRotationSlot, 1, rotation, argument2, argument3);

    /// <summary>Sets the Player's Camera Socket Offset (Spring Arm Offset)</summary>
    public void SetCameraSocketOffset(Vector socketOffset) => Call(SetCameraSocketOffsetSlot, 1, socketOffset);

    /// <summary>Sets the Player's Camera Speed Settings (if not possessing a Character)</summary>
    public void SetCameraSpeedSettings(
        float? maxSpeed = null,
        float? acceleration = null,
        float? deceleration = null,
        float? turningBoost = null) =>
        Call(SetCameraSpeedSettingsSlot, 0, maxSpeed, acceleration, deceleration, turningBoost);

    /// <summary>Turns on camera fading at the given opacity</summary>
    public void SetManualCameraFade(float inFadeAmount, Color color, bool inFadeAudio) =>
        Call(SetManualCameraFadeSlot, 3, inFadeAmount, color, inFadeAudio);

    /// <summary>Sets the global VOIP high-pass filter threshold frequency</summary>
    public void SetVOIPGlobalHighPassFilter(ushort threshold) => Call(SetVOIPGlobalHighPassFilterSlot, 1, threshold);

    /// <summary>Sets the global VOIP low-pass filter threshold frequency</summary>
    public void SetVOIPGlobalLowPassFilter(ushort threshold) => Call(SetVOIPGlobalLowPassFilterSlot, 1, threshold);

    /// <summary>Sets the global VOIP volume. It only affects the global channels</summary>
    public void SetVOIPGlobalVolume(float volume) => Call(SetVOIPGlobalVolumeSlot, 1, volume);

    /// <summary>Sets the local/proximity VOIP max distance. This also affects networking usage</summary>
    public void SetVOIPLocalMaxDistance(ushort maxDistance) => Call(SetVOIPLocalMaxDistanceSlot, 1, maxDistance);

    /// <summary>Sets the local/proximity VOIP input/output volume. It only affects the local/proximity channel</summary>
    public void SetVOIPLocalVolume(float volume) => Call(SetVOIPLocalVolumeSlot, 1, volume);

    /// <summary>Mutes or unmutes VOIP for this player</summary>
    public void SetVOIPMuted(bool isMuted) => Call(SetVOIPMutedSlot, 1, isMuted);

    /// <summary>Sets the voice setting of the Player</summary>
    public void SetVOIPSetting(VOIPSetting setting) => Call(SetVOIPSettingSlot, 1, setting);

    /// <summary>Sets the voice volume of the Player</summary>
    public void SetVOIPVolume(float volume) => Call(SetVOIPVolumeSlot, 1, volume);

    /// <summary>Makes this Player spectate another Player</summary>
    public void Spectate(Player player, float? blendSpeed = null) => Call(SpectateSlot, 1, player, blendSpeed);

    /// <summary>Does a camera fade to/from a solid color</summary>
    public void StartCameraFade(
        float fromAlpha,
        float toAlpha,
        float duration,
        Color toColor,
        bool? shouldFadeAudio = null,
        bool? holdWhenFinished = null) =>
        Call(StartCameraFadeSlot, 4, fromAlpha, toAlpha, duration, toColor, shouldFadeAudio, holdWhenFinished);

    /// <summary>Stops camera fading.</summary>
    public void StopCameraFade() => Call(StopCameraFadeSlot, 0);

    /// <summary>Smoothly moves the Player's Camera Location (only works if not possessing any Character)</summary>
    public void TranslateCameraTo(Vector location, float? time = null, float? exp = null) =>
        Call(TranslateCameraToSlot, 1, location, time, exp);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the Possess event of this entity only.</summary>
    public ScriptSubscription SubscribePossess(PossessHandler handler) => Subscribe("Possess", handler);

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

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the Possess event of this entity when a subclass overrides it.</summary>
    [EventHook("Possess")]
    protected virtual void HandlePossess(Entity? pawn)
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
    {
    }

    /// <summary>Called for the UnPossess event of this entity when a subclass overrides it.</summary>
    [EventHook("UnPossess")]
    protected virtual void HandleUnPossess(Entity? pawn)
    {
    }

    /// <summary>Called for the VOIPGlobalChannelSettingChange event of this entity when a subclass overrides it.</summary>
    [EventHook("VOIPGlobalChannelSettingChange")]
    protected virtual void HandleVOIPGlobalChannelSettingChange(int channel, VOIPSetting oldSetting, VOIPSetting newSetting)
    {
    }

    /// <summary>Called for the VOIPLocalSettingChange event of this entity when a subclass overrides it.</summary>
    [EventHook("VOIPLocalSettingChange")]
    protected virtual void HandleVOIPLocalSettingChange(VOIPSetting oldSetting, VOIPSetting newSetting)
    {
    }

    /// <summary>Called for the ValueChange event of this entity when a subclass overrides it.</summary>
    [EventHook("ValueChange")]
    protected virtual void HandleValueChange(string? key, object? value)
    {
    }

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Player self);

    /// <summary>Triggered when Player starts controlling a Pawn (Character)</summary>
    public delegate void PossessHandler(Player self, Entity? pawn);

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
