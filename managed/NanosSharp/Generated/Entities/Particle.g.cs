#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>Class to spawn Particle Systems used to create effects in the world.</summary>
[ScriptClass("Particle")]
public partial class Particle : Actor
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

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Particle", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Particle", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Particle", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Particle", "EnterWater");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Particle", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Particle", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Particle", "Spawn");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Particle", "ValueChange");
    private static readonly MethodSlot ActivateSlot = new("Activate");
    private static readonly MethodSlot DeactivateSlot = new("Deactivate");
    private static readonly MethodSlot IsActiveSlot = new("IsActive");
    private static readonly MethodSlot SetParameterBoolSlot = new("SetParameterBool");
    private static readonly MethodSlot SetParameterColorSlot = new("SetParameterColor");
    private static readonly MethodSlot SetParameterFloatSlot = new("SetParameterFloat");
    private static readonly MethodSlot SetParameterIntSlot = new("SetParameterInt");
    private static readonly MethodSlot SetParameterMaterialSlot = new("SetParameterMaterial");
    private static readonly MethodSlot SetParameterMaterialFromTextureSlot = new("SetParameterMaterialFromTexture");
    private static readonly MethodSlot SetParameterVectorSlot = new("SetParameterVector");

    /// <summary>Returns if this Particle is active</summary>
    public bool Active => IsActive();

    /// <summary>Spawns a new Particle.</summary>
    public Particle(
        Vector location,
        Rotator rotation,
        string asset,
        bool? autoDestroy = null,
        bool? autoActivate = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Particle", ScriptArguments.Trim([location, rotation, asset, autoDestroy, autoActivate, spawnMode], 3));
    }

    protected Particle()
    {
    }

    /// <summary>Activates the Emitter</summary>
    public void Activate(bool? shouldReset = null) => Invoke(ActivateSlot, ScriptArguments.Trim([shouldReset], 0));

    /// <summary>Deactivate the Emitter</summary>
    public void Deactivate() => Invoke(DeactivateSlot);

    /// <summary>Returns if this Particle is active</summary>
    public bool IsActive() => Invoke<bool>(IsActiveSlot)!;

    /// <summary>Sets a boolean parameter in this Particle System</summary>
    public void SetParameterBool(string parameter, bool value) => Invoke(SetParameterBoolSlot, parameter, value);

    /// <summary>Sets a Color parameter in this Particle System</summary>
    public void SetParameterColor(string parameter, Color value) => Invoke(SetParameterColorSlot, parameter, value);

    /// <summary>Sets a float parameter in this Particle System</summary>
    public void SetParameterFloat(string parameter, float value) => Invoke(SetParameterFloatSlot, parameter, value);

    /// <summary>Sets an integer parameter in this Particle System</summary>
    public void SetParameterInt(string parameter, int value) => Invoke(SetParameterIntSlot, parameter, value);

    /// <summary>Sets a Material parameter in this Particle System</summary>
    public void SetParameterMaterial(string parameter, string value) =>
        Invoke(SetParameterMaterialSlot, parameter, value);

    /// <summary>Sets a Material from a Texture parameter in this Particle System</summary>
    public void SetParameterMaterialFromTexture(string parameter, string value) =>
        Invoke(SetParameterMaterialFromTextureSlot, parameter, value);

    /// <summary>Sets a Vector parameter in this Particle System</summary>
    public void SetParameterVector(string parameter, Vector value) => Invoke(SetParameterVectorSlot, parameter, value);

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
    public delegate void DestroyHandler(Particle self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Particle self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Particle self);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Particle self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Particle self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Particle self);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Particle self, string? key, object? value);
}
