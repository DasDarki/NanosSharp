#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Class for playing in-game 2D and 3D sounds.</summary>
[ScriptClass("Sound")]
public partial class Sound : Actor
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

    /// <summary>Triggered when an Actor enters a water body</summary>
    public static event EnterWaterHandler? OnEnterWater
    {
        add => EnterWaterEvent.Add(value);
        remove => EnterWaterEvent.Remove(value);
    }

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public static event LeaveWaterHandler? OnLeaveWater
    {
        add => LeaveWaterEvent.Add(value);
        remove => LeaveWaterEvent.Remove(value);
    }

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public static event NetworkAuthorityChangeHandler? OnNetworkAuthorityChange
    {
        add => NetworkAuthorityChangeEvent.Add(value);
        remove => NetworkAuthorityChangeEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public static event SpawnHandler? OnSpawn
    {
        add => SpawnEvent.Add(value);
        remove => SpawnEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Sound", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Sound", "Destroy");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Sound", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Sound", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Sound", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Sound", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Sound", "ValueChange");
    private static readonly MethodSlot CallEventSlot = new("CallEvent");
    private static readonly MethodSlot FadeInSlot = new("FadeIn");
    private static readonly MethodSlot FadeOutSlot = new("FadeOut");
    private static readonly MethodSlot GetBoneTransformSlot = new("GetBoneTransform");
    private static readonly MethodSlot GetDurationSlot = new("GetDuration");
    private static readonly MethodSlot GetFalloffDistanceSlot = new("GetFalloffDistance");
    private static readonly MethodSlot GetInitialLocationSlot = new("GetInitialLocation");
    private static readonly MethodSlot GetInnerRadiusSlot = new("GetInnerRadius");
    private static readonly MethodSlot GetLowPassFilterSlot = new("GetLowPassFilter");
    private static readonly MethodSlot GetPathSlot = new("GetPath");
    private static readonly MethodSlot GetPitchSlot = new("GetPitch");
    private static readonly MethodSlot GetSoundTypeSlot = new("GetSoundType");
    private static readonly MethodSlot GetVolumeSlot = new("GetVolume");
    private static readonly MethodSlot Is2DSlot = new("Is2D");
    private static readonly MethodSlot IsPlayingSlot = new("IsPlaying");
    private static readonly MethodSlot PlaySlot = new("Play");
    private static readonly MethodSlot SetFalloffDistanceSlot = new("SetFalloffDistance");
    private static readonly MethodSlot SetInitialLocationSlot = new("SetInitialLocation");
    private static readonly MethodSlot SetInnerRadiusSlot = new("SetInnerRadius");
    private static readonly MethodSlot SetLowPassFilterSlot = new("SetLowPassFilter");
    private static readonly MethodSlot SetPausedSlot = new("SetPaused");
    private static readonly MethodSlot SetPitchSlot = new("SetPitch");
    private static readonly MethodSlot SetVolumeSlot = new("SetVolume");
    private static readonly MethodSlot StopSlot = new("Stop");
    private static readonly MethodSlot StopDelayedSlot = new("StopDelayed");

    /// <summary>Gets the duration of the Sound</summary>
    public float Duration => GetDuration();

    /// <summary>Gets the distance at which the sound becomes inaudible</summary>
    public float FalloffDistance
    {
        get => GetFalloffDistance();
        set => SetFalloffDistance(value);
    }

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector InitialLocation
    {
        get => GetInitialLocation();
        set => SetInitialLocation(value);
    }

    /// <summary>Gets the radius within which the volume is 100%</summary>
    public float InnerRadius
    {
        get => GetInnerRadius();
        set => SetInnerRadius(value);
    }

    /// <summary>Gets the low pass filter frequency</summary>
    public float LowPassFilter
    {
        get => GetLowPassFilter();
        set => SetLowPassFilter(value);
    }

    /// <summary>Gets the asset path of this Sound</summary>
    public string Path => GetPath();

    /// <summary>Gets the pitch multiplier</summary>
    public float Pitch
    {
        get => GetPitch();
        set => SetPitch(value);
    }

    /// <summary>Gets the Sound Type, used to apply the user's volume settings</summary>
    public SoundType SoundType => GetSoundType();

    /// <summary>Gets the volume multiplier</summary>
    public float Volume
    {
        get => GetVolume();
        set => SetVolume(value);
    }

    /// <summary>Gets if the sound is playing</summary>
    public bool Playing => IsPlaying();

    /// <summary>Spawns a new Sound.</summary>
    public Sound(
        Vector location,
        string asset,
        bool? is2dSound = null,
        bool? autoDestroy = null,
        SoundType? soundType = null,
        float? volume = null,
        float? pitch = null,
        float? innerRadius = null,
        float? falloffDistance = null,
        AttenuationFunction? attenuationFunction = null,
        bool? keepPlayingWhenSilent = null,
        SoundLoopMode? loopMode = null,
        bool? autoPlay = null)
    {
        SpawnSelf("Sound", ScriptArguments.Trim([location, asset, is2dSound, autoDestroy, soundType, volume, pitch, innerRadius, falloffDistance, attenuationFunction, keepPlayingWhenSilent, loopMode, autoPlay], 2));
    }

    protected Sound()
    {
    }

    /// <summary>Calls a custom event on this entity, received by Subscribe of this entity</summary>
    public void CallEvent(string eventName, params object?[] arguments) =>
        InvokeVariadic(CallEventSlot, [eventName], arguments);

    /// <summary>Plays the sound with a fade effect</summary>
    public void FadeIn(float fadeInDuration, float? fadeVolumeLevel = null, float? startTime = null) =>
        Call(FadeInSlot, 1, fadeInDuration, fadeVolumeLevel, startTime);

    /// <summary>Stops the sound with a fade effect</summary>
    public void FadeOut(float fadeOutDuration, float? fadeVolumeLevel = null, bool? destroyAfterFadeout = null) =>
        Call(FadeOutSlot, 1, fadeOutDuration, fadeVolumeLevel, destroyAfterFadeout);

    /// <summary>Gets the world transform of a bone, a table with location and rotation</summary>
    public IReadOnlyDictionary<string, object?> GetBoneTransform(string boneName) =>
        CallResult<IReadOnlyDictionary<string, object?>, string>(GetBoneTransformSlot, 1, boneName)!;

    /// <summary>Gets the duration of the Sound</summary>
    public float GetDuration() => CallResult<float>(GetDurationSlot, 0)!;

    /// <summary>Gets the distance at which the sound becomes inaudible</summary>
    public float GetFalloffDistance() => CallResult<float>(GetFalloffDistanceSlot, 0)!;

    /// <summary>Gets the location the entity was spawned at</summary>
    public Vector GetInitialLocation() => CallResult<Vector>(GetInitialLocationSlot, 0)!;

    /// <summary>Gets the radius within which the volume is 100%</summary>
    public float GetInnerRadius() => CallResult<float>(GetInnerRadiusSlot, 0)!;

    /// <summary>Gets the low pass filter frequency</summary>
    public float GetLowPassFilter() => CallResult<float>(GetLowPassFilterSlot, 0)!;

    /// <summary>Gets the asset path of this Sound</summary>
    public string GetPath() => CallResult<string>(GetPathSlot, 0)!;

    /// <summary>Gets the pitch multiplier</summary>
    public float GetPitch() => CallResult<float>(GetPitchSlot, 0)!;

    /// <summary>Gets the Sound Type, used to apply the user's volume settings</summary>
    public SoundType GetSoundType() => CallResult<SoundType>(GetSoundTypeSlot, 0)!;

    /// <summary>Gets the volume multiplier</summary>
    public float GetVolume() => CallResult<float>(GetVolumeSlot, 0)!;

    /// <summary>Gets if the sound is 2D</summary>
    public bool Is2D() => CallResult<bool>(Is2DSlot, 0)!;

    /// <summary>Gets if the sound is playing</summary>
    public bool IsPlaying() => CallResult<bool>(IsPlayingSlot, 0)!;

    /// <summary>Starts the sound</summary>
    public void Play(float? startTime = null) => Call(PlaySlot, 0, startTime);

    /// <summary>If a 3D Sound, sets the distance which the sound is inaudible</summary>
    public void SetFalloffDistance(float falloffDistance) => Call(SetFalloffDistanceSlot, 1, falloffDistance);

    /// <summary>Sets the location the entity returns to, for example when it falls out of the world</summary>
    public void SetInitialLocation(Vector location) => Call(SetInitialLocationSlot, 1, location);

    /// <summary>If a 3D Sound, sets the radius within which the volume is 100%</summary>
    public void SetInnerRadius(float innerRadius) => Call(SetInnerRadiusSlot, 1, innerRadius);

    /// <summary>Sets low-pass filter frequency. Sets 0 to disable it.</summary>
    public void SetLowPassFilter(float frequency) => Call(SetLowPassFilterSlot, 1, frequency);

    /// <summary>Pauses or resumes the sound</summary>
    public void SetPaused(bool? pause = null) => Call(SetPausedSlot, 0, pause);

    /// <summary>Sets the Sound's pitch</summary>
    public void SetPitch(float newPitch) => Call(SetPitchSlot, 1, newPitch);

    /// <summary>Sets the Sound's volume</summary>
    public void SetVolume(float newVolume) => Call(SetVolumeSlot, 1, newVolume);

    /// <summary>Stops the sound</summary>
    public void Stop() => Call(StopSlot, 0);

    /// <summary>Stops the sound after the provided delay</summary>
    public void StopDelayed(float delay) => Call(StopDelayedSlot, 1, delay);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Called for the Destroy event of this entity when a subclass overrides it.</summary>
    [EventHook("Destroy")]
    protected virtual void HandleDestroy()
    {
    }

    /// <summary>Called for the EnterWater event of this entity when a subclass overrides it.</summary>
    [EventHook("EnterWater")]
    protected virtual void HandleEnterWater()
    {
    }

    /// <summary>Called for the LeaveWater event of this entity when a subclass overrides it.</summary>
    [EventHook("LeaveWater")]
    protected virtual void HandleLeaveWater()
    {
    }

    /// <summary>Called for the NetworkAuthorityChange event of this entity when a subclass overrides it.</summary>
    [EventHook("NetworkAuthorityChange")]
    protected virtual void HandleNetworkAuthorityChange(bool isNetworkAuthority)
    {
    }

    /// <summary>Called for the Spawn event of this entity when a subclass overrides it.</summary>
    [EventHook("Spawn")]
    protected virtual void HandleSpawn()
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
    public delegate void DestroyHandler(Sound self);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Sound self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Sound self);

    /// <summary>Triggered when the local Player gets/loses network authority over this actor</summary>
    public delegate void NetworkAuthorityChangeHandler(Sound self, bool isNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Sound self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Sound self, string? key, object? value);
}
