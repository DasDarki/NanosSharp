#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A Prop represents a Dynamic Mesh which can be spawned in the world, can be grabbed around by characters and have physics.</summary>
[ScriptClass("Prop")]
public partial class Prop : Actor, IPaintable
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

    /// <summary>Triggered when Character grabs a Prop</summary>
    public static event GrabHandler? OnGrab
    {
        add => GrabEvent.Add(value);
        remove => GrabEvent.Remove(value);
    }

    /// <summary>Triggered when this Prop hits something</summary>
    public static event HitHandler? OnHit
    {
        add => HitEvent.Add(value);
        remove => HitEvent.Remove(value);
    }

    /// <summary>When a Character interacts with this Prop (i.e. try to Grab it)</summary>
    public static event InteractHandler? OnInteract
    {
        add => InteractEvent.Add(value);
        remove => InteractEvent.Remove(value);
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

    /// <summary>When Prop takes Damage</summary>
    public static event TakeDamageHandler? OnTakeDamage
    {
        add => TakeDamageEvent.Add(value);
        remove => TakeDamageEvent.Remove(value);
    }

    /// <summary>Triggered when this Prop is ungrabbed</summary>
    public static event UnGrabHandler? OnUnGrab
    {
        add => UnGrabEvent.Add(value);
        remove => UnGrabEvent.Remove(value);
    }

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public static event ValueChangeHandler? OnValueChange
    {
        add => ValueChangeEvent.Add(value);
        remove => ValueChangeEvent.Remove(value);
    }

    private static readonly ClassEvent<ClassRegisterHandler> ClassRegisterEvent = new("Prop", "ClassRegister");
    private static readonly ClassEvent<DestroyHandler> DestroyEvent = new("Prop", "Destroy");
    private static readonly ClassEvent<DimensionChangeHandler> DimensionChangeEvent = new("Prop", "DimensionChange");
    private static readonly ClassEvent<EnterWaterHandler> EnterWaterEvent = new("Prop", "EnterWater");
    private static readonly ClassEvent<GrabHandler> GrabEvent = new("Prop", "Grab");
    private static readonly ClassEvent<HitHandler> HitEvent = new("Prop", "Hit");
    private static readonly ClassEvent<InteractHandler> InteractEvent = new("Prop", "Interact");
    private static readonly ClassEvent<LeaveWaterHandler> LeaveWaterEvent = new("Prop", "LeaveWater");
    private static readonly ClassEvent<NetworkAuthorityChangeHandler> NetworkAuthorityChangeEvent = new("Prop", "NetworkAuthorityChange");
    private static readonly ClassEvent<SpawnHandler> SpawnEvent = new("Prop", "Spawn");
    private static readonly ClassEvent<TakeDamageHandler> TakeDamageEvent = new("Prop", "TakeDamage");
    private static readonly ClassEvent<UnGrabHandler> UnGrabEvent = new("Prop", "UnGrab");
    private static readonly ClassEvent<ValueChangeHandler> ValueChangeEvent = new("Prop", "ValueChange");
    private static readonly MethodSlot GetGrabModeSlot = new("GetGrabMode");
    private static readonly MethodSlot GetHandlerSlot = new("GetHandler");
    private static readonly MethodSlot GetMassOverrideSlot = new("GetMassOverride");
    private static readonly MethodSlot GetMassScaleSlot = new("GetMassScale");
    private static readonly MethodSlot GetMaterialColorParameterSlot = new("GetMaterialColorParameter");
    private static readonly MethodSlot GetMaterialScalarParameterSlot = new("GetMaterialScalarParameter");
    private static readonly MethodSlot GetMaterialTextureParameterSlot = new("GetMaterialTextureParameter");
    private static readonly MethodSlot GetMaterialVectorParameterSlot = new("GetMaterialVectorParameter");
    private static readonly MethodSlot GetMeshSlot = new("GetMesh");
    private static readonly MethodSlot ResetMaterialSlot = new("ResetMaterial");
    private static readonly MethodSlot SetBuoyancyPontoonsSlot = new("SetBuoyancyPontoons");
    private static readonly MethodSlot SetBuoyancySettingsSlot = new("SetBuoyancySettings");
    private static readonly MethodSlot SetGrabModeSlot = new("SetGrabMode");
    private static readonly MethodSlot SetInteractionToolTipTextSlot = new("SetInteractionToolTipText");
    private static readonly MethodSlot SetMassOverrideSlot = new("SetMassOverride");
    private static readonly MethodSlot SetMassScaleSlot = new("SetMassScale");
    private static readonly MethodSlot SetMaterialSlot = new("SetMaterial");
    private static readonly MethodSlot SetMaterialColorParameterSlot = new("SetMaterialColorParameter");
    private static readonly MethodSlot SetMaterialScalarParameterSlot = new("SetMaterialScalarParameter");
    private static readonly MethodSlot SetMaterialTextureParameterSlot = new("SetMaterialTextureParameter");
    private static readonly MethodSlot SetMaterialVectorParameterSlot = new("SetMaterialVectorParameter");
    private static readonly MethodSlot SetMeshSlot = new("SetMesh");
    private static readonly MethodSlot SetPhysicalMaterialSlot = new("SetPhysicalMaterial");
    private static readonly MethodSlot SetPhysicsDampingSlot = new("SetPhysicsDamping");

    /// <summary>Gets ability to Grab this Prop</summary>
    public GrabMode GrabMode
    {
        get => GetGrabMode();
        set => SetGrabMode(value);
    }

    /// <summary>Gets the Character (if existing) which is holding this</summary>
    public Character? Handler => GetHandler();

    /// <summary>Gets the overridden mass</summary>
    public float MassOverride
    {
        get => GetMassOverride();
        set => SetMassOverride(value);
    }

    /// <summary>Gets the mass scale</summary>
    public float MassScale
    {
        get => GetMassScale();
        set => SetMassScale(value);
    }

    /// <summary>Gets the Asset name</summary>
    public string Mesh
    {
        get => GetMesh();
        set => SetMesh(value);
    }

    /// <summary>Spawns a new Prop.</summary>
    public Prop(
        Vector location,
        Rotator rotation,
        string asset,
        CollisionType? collisionType = null,
        bool? gravityEnabled = null,
        GrabMode? grabMode = null,
        CCDMode? ccdMode = null,
        SpawnMode? spawnMode = null)
    {
        SpawnSelf("Prop", ScriptArguments.Trim([location, rotation, asset, collisionType, gravityEnabled, grabMode, ccdMode, spawnMode], 3));
    }

    protected Prop()
    {
    }

    /// <summary>Gets ability to Grab this Prop</summary>
    public GrabMode GetGrabMode() => CallResult<GrabMode>(GetGrabModeSlot, 0)!;

    /// <summary>Gets the Character (if existing) which is holding this</summary>
    public Character? GetHandler() => CallResult<Character?>(GetHandlerSlot, 0);

    /// <summary>Gets the overridden mass</summary>
    public float GetMassOverride() => CallResult<float>(GetMassOverrideSlot, 0)!;

    /// <summary>Gets the mass scale</summary>
    public float GetMassScale() => CallResult<float>(GetMassScaleSlot, 0)!;

    /// <summary>Gets a Color parameter from this Actor's material</summary>
    public Color? GetMaterialColorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<Color?, string, int?, string?>(GetMaterialColorParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Scalar parameter from this Actor's material</summary>
    public float? GetMaterialScalarParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<float?, string, int?, string?>(GetMaterialScalarParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Texture parameter from this Actor's material</summary>
    public string? GetMaterialTextureParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<string?, string, int?, string?>(GetMaterialTextureParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets a Vector parameter from this Actor's material</summary>
    public Vector? GetMaterialVectorParameter(string parameterName, int? index = null, string? attachableId = null) =>
        CallResult<Vector?, string, int?, string?>(GetMaterialVectorParameterSlot, 1, parameterName, index, attachableId);

    /// <summary>Gets the Asset name</summary>
    public string GetMesh() => CallResult<string>(GetMeshSlot, 0)!;

    /// <summary>Resets the material from the specified index to the original one</summary>
    public void ResetMaterial(int? index = null, string? attachableId = null) =>
        Call(ResetMaterialSlot, 0, index, attachableId);

    /// <summary>Sets the Buoyancy Pontoons used by the Water simulation</summary>
    public void SetBuoyancyPontoons(IEnumerable<IDictionary<string, object?>>? pontoons = null) =>
        Call(SetBuoyancyPontoonsSlot, 0, pontoons);

    /// <summary>Sets how this entity behaves on Water</summary>
    public void SetBuoyancySettings(
        bool? enabled = null,
        float? buoyancyCoefficient = null,
        float? buoyancyDamp = null,
        float? maxBuoyantForce = null) =>
        Call(SetBuoyancySettingsSlot, 0, enabled, buoyancyCoefficient, buoyancyDamp, maxBuoyantForce);

    /// <summary>Sets ability to Characters to Grab this Prop</summary>
    public void SetGrabMode(GrabMode grabMode) => Call(SetGrabModeSlot, 1, grabMode);

    /// <summary>Sets the interaction tooltip text shown when a Character interacts with this Prop</summary>
    public void SetInteractionToolTipText(string toolTipText) => Call(SetInteractionToolTipTextSlot, 1, toolTipText);

    /// <summary>Override the mass (in Kg) of this Prop</summary>
    public void SetMassOverride(float mass) => Call(SetMassOverrideSlot, 1, mass);

    /// <summary>Change the mass scale used to calculate the mass of this Prop</summary>
    public void SetMassScale(float scale) => Call(SetMassScaleSlot, 1, scale);

    /// <summary>Sets the material at the specified index of this Actor</summary>
    public void SetMaterial(string materialPath, int? index = null, string? attachableId = null) =>
        Call(SetMaterialSlot, 1, materialPath, index, attachableId);

    /// <summary>Sets a Color parameter in this Actor's material</summary>
    public void SetMaterialColorParameter(
        string parameterName,
        Color color,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialColorParameterSlot, 2, parameterName, color, index, attachableId);

    /// <summary>Sets a Scalar parameter in this Actor's material</summary>
    public void SetMaterialScalarParameter(
        string parameterName,
        float scalar,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialScalarParameterSlot, 2, parameterName, scalar, index, attachableId);

    /// <summary>Sets a texture parameter in this Actor's material to an image</summary>
    public void SetMaterialTextureParameter(
        string parameterName,
        string texturePath,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialTextureParameterSlot, 2, parameterName, texturePath, index, attachableId);

    /// <summary>Sets a Vector parameter in this Actor's material</summary>
    public void SetMaterialVectorParameter(
        string parameterName,
        Vector vector,
        int? index = null,
        string? attachableId = null) =>
        Call(SetMaterialVectorParameterSlot, 2, parameterName, vector, index, attachableId);

    /// <summary>Changes the mesh in runtime</summary>
    public void SetMesh(string asset) => Call(SetMeshSlot, 1, asset);

    /// <summary>Overrides this Actor's Physical Material with a new one</summary>
    public void SetPhysicalMaterial(string physicalMaterialPath) =>
        Call(SetPhysicalMaterialSlot, 1, physicalMaterialPath);

    /// <summary>Sets the Physics damping of this Prop</summary>
    public void SetPhysicsDamping(float linearDamping, float angularDamping) =>
        Call(SetPhysicsDampingSlot, 2, linearDamping, angularDamping);

    /// <summary>Subscribes to the ClassRegister event of this entity only.</summary>
    public ScriptSubscription SubscribeClassRegister(ClassRegisterHandler handler) => Subscribe("ClassRegister", handler);

    /// <summary>Subscribes to the Destroy event of this entity only.</summary>
    public ScriptSubscription SubscribeDestroy(DestroyHandler handler) => Subscribe("Destroy", handler);

    /// <summary>Subscribes to the DimensionChange event of this entity only.</summary>
    public ScriptSubscription SubscribeDimensionChange(DimensionChangeHandler handler) => Subscribe("DimensionChange", handler);

    /// <summary>Subscribes to the EnterWater event of this entity only.</summary>
    public ScriptSubscription SubscribeEnterWater(EnterWaterHandler handler) => Subscribe("EnterWater", handler);

    /// <summary>Subscribes to the Grab event of this entity only.</summary>
    public ScriptSubscription SubscribeGrab(GrabHandler handler) => Subscribe("Grab", handler);

    /// <summary>Subscribes to the Hit event of this entity only.</summary>
    public ScriptSubscription SubscribeHit(HitHandler handler) => Subscribe("Hit", handler);

    /// <summary>Subscribes to the Interact event of this entity only.</summary>
    public ScriptSubscription SubscribeInteract(InteractHandler handler) => Subscribe("Interact", handler);

    /// <summary>Subscribes to the LeaveWater event of this entity only.</summary>
    public ScriptSubscription SubscribeLeaveWater(LeaveWaterHandler handler) => Subscribe("LeaveWater", handler);

    /// <summary>Subscribes to the NetworkAuthorityChange event of this entity only.</summary>
    public ScriptSubscription SubscribeNetworkAuthorityChange(NetworkAuthorityChangeHandler handler) => Subscribe("NetworkAuthorityChange", handler);

    /// <summary>Subscribes to the Spawn event of this entity only.</summary>
    public ScriptSubscription SubscribeSpawn(SpawnHandler handler) => Subscribe("Spawn", handler);

    /// <summary>Subscribes to the TakeDamage event of this entity only.</summary>
    public ScriptSubscription SubscribeTakeDamage(TakeDamageHandler handler) => Subscribe("TakeDamage", handler);

    /// <summary>Subscribes to the UnGrab event of this entity only.</summary>
    public ScriptSubscription SubscribeUnGrab(UnGrabHandler handler) => Subscribe("UnGrab", handler);

    /// <summary>Subscribes to the ValueChange event of this entity only.</summary>
    public ScriptSubscription SubscribeValueChange(ValueChangeHandler handler) => Subscribe("ValueChange", handler);

    /// <summary>Triggered when a new Class is registered with the Inheriting System</summary>
    public delegate void ClassRegisterHandler(object? @class);

    /// <summary>Triggered when an Entity is destroyed</summary>
    public delegate void DestroyHandler(Prop self);

    /// <summary>Triggered when an Actor changes its dimension</summary>
    public delegate void DimensionChangeHandler(Prop self, int oldDimension, int newDimension);

    /// <summary>Triggered when an Actor enters a water body</summary>
    public delegate void EnterWaterHandler(Prop self);

    /// <summary>Triggered when Character grabs a Prop</summary>
    public delegate void GrabHandler(Prop self, Character? character);

    /// <summary>Triggered when this Prop hits something</summary>
    public delegate void HitHandler(Prop self, float impactForce, Vector normalImpulse, Vector impactLocation, Vector velocity, Actor? otherActor);

    /// <summary>When a Character interacts with this Prop (i.e. try to Grab it)</summary>
    public delegate bool InteractHandler(Prop self, Character? character);

    /// <summary>Triggered when an Actor leaves a water body</summary>
    public delegate void LeaveWaterHandler(Prop self);

    /// <summary>Triggered when an Actor changes its network authority Player</summary>
    public delegate void NetworkAuthorityChangeHandler(Prop self, Player? oldNetworkAuthority, Player? newNetworkAuthority);

    /// <summary>Triggered when an Entity is spawned/created</summary>
    public delegate void SpawnHandler(Prop self);

    /// <summary>When Prop takes Damage</summary>
    public delegate void TakeDamageHandler(Prop self, int damage, string? bone, DamageType type, Vector fromDirection, Player? instigator, Actor? causer);

    /// <summary>Triggered when this Prop is ungrabbed</summary>
    public delegate void UnGrabHandler(Prop self, Character? character);

    /// <summary>Triggered when an Entity has a value changed with <c>:SetValue()</c></summary>
    public delegate void ValueChangeHandler(Prop self, string? key, object? value);
}
