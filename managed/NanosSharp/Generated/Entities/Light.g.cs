#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Light represents a Lighting source.</summary>
[ScriptClass("Light")]
public partial class Light : Actor
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

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public static event DimensionChangeHandler? OnDimensionChange
    {
        add => DimensionChangeEvent.Add(value);
        remove => DimensionChangeEvent.Remove(value);
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

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
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

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Light", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Light", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Light", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Light", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Light", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Light", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Light", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Light", "ValueChange");
    private static readonly MethodSlot GetAttenuationRadiusSlot = new("GetAttenuationRadius");
    private static readonly MethodSlot GetCastShadowsSlot = new("GetCastShadows");
    private static readonly MethodSlot GetColorSlot = new("GetColor");
    private static readonly MethodSlot GetIntensitySlot = new("GetIntensity");
    private static readonly MethodSlot SetAttenuationRadiusSlot = new("SetAttenuationRadius");
    private static readonly MethodSlot SetCastShadowsSlot = new("SetCastShadows");
    private static readonly MethodSlot SetColorSlot = new("SetColor");
    private static readonly MethodSlot SetIntensitySlot = new("SetIntensity");
    private static readonly MethodSlot SetTextureLightProfileSlot = new("SetTextureLightProfile");

    /// <summary>Gets the distance this Light reaches</summary>
    public float AttenuationRadius
    {
        get => GetAttenuationRadius();
        set => SetAttenuationRadius(value);
    }

    /// <summary>Gets if this Light casts shadows</summary>
    public bool CastShadows
    {
        get => GetCastShadows();
        set => SetCastShadows(value);
    }

    /// <summary>Gets the color of this Light</summary>
    public Color Color
    {
        get => GetColor();
        set => SetColor(value);
    }

    /// <summary>Gets the brightness of this Light</summary>
    public float Intensity
    {
        get => GetIntensity();
        set => SetIntensity(value);
    }

    /// <summary>Spawns a new Light.</summary>
    public Light(
        Vector location,
        Rotator? rotation = null,
        Color? color = null,
        LightType? lightType = null,
        float? intensity = null,
        float? attenuationRadius = null,
        float? coneAngle = null,
        float? innerConeAnglePercent = null,
        float? maxDrawDistance = null,
        bool? useInverseSquaredFalloff = null,
        bool? castShadows = null,
        bool? visible = null,
        float? sourceRadius = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Light", ScriptArguments.Trim([location, rotation, color, lightType, intensity, attenuationRadius, coneAngle, innerConeAnglePercent, maxDrawDistance, useInverseSquaredFalloff, castShadows, visible, sourceRadius, spawnMode], 1));
    }

    protected Light()
    {
    }

    /// <summary>Gets the distance this Light reaches</summary>
    public float GetAttenuationRadius() => CallResult<float>(GetAttenuationRadiusSlot, 0)!;

    /// <summary>Gets if this Light casts shadows</summary>
    public bool GetCastShadows() => CallResult<bool>(GetCastShadowsSlot, 0)!;

    /// <summary>Gets the color of this Light</summary>
    public Color GetColor() => CallResult<Color>(GetColorSlot, 0)!;

    /// <summary>Gets the brightness of this Light</summary>
    public float GetIntensity() => CallResult<float>(GetIntensitySlot, 0)!;

    /// <summary>Sets the distance this Light reaches</summary>
    public void SetAttenuationRadius(float attenuationRadius) => Call(SetAttenuationRadiusSlot, 1, attenuationRadius);

    /// <summary>Sets if this Light casts shadows</summary>
    public void SetCastShadows(bool isShadowsEnabled) => Call(SetCastShadowsSlot, 1, isShadowsEnabled);

    /// <summary>Sets the light color</summary>
    public void SetColor(Color color) => Call(SetColorSlot, 1, color);

    /// <summary>Sets the brightness of this Light</summary>
    public void SetIntensity(float intensity) => Call(SetIntensitySlot, 1, intensity);

    /// <summary>Sets the light Texture Profile</summary>
    public void SetTextureLightProfile(LightProfile lightProfile) => Call(SetTextureLightProfileSlot, 1, lightProfile);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

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

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Light self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Light self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Light self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Light self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Light self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Light self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Light self, string? key, object? value);
}
