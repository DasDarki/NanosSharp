#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>An Actor is an object which can be spawned and destroyed through Lua.Actors support 3D transformations such as translation (location), rotation, and scale.An Actor is the base for several entities, and all those entities share the same Methods and Events described in this page.</summary>
public abstract partial class Actor : Entity
{
    private static readonly MethodSlot AddActorTagSlot = new("AddActorTag");
    private static readonly MethodSlot AddAngularImpulseSlot = new("AddAngularImpulse");
    private static readonly MethodSlot AddImpulseSlot = new("AddImpulse");
    private static readonly MethodSlot AttachToSlot = new("AttachTo");
    private static readonly MethodSlot DetachSlot = new("Detach");
    private static readonly MethodSlot GetActorTagsSlot = new("GetActorTags");
    private static readonly MethodSlot GetAngularForceSlot = new("GetAngularForce");
    private static readonly MethodSlot GetAngularVelocitySlot = new("GetAngularVelocity");
    private static readonly MethodSlot GetAttachedEntitiesSlot = new("GetAttachedEntities");
    private static readonly MethodSlot GetAttachedToSlot = new("GetAttachedTo");
    private static readonly MethodSlot GetAttachedToSocketSlot = new("GetAttachedToSocket");
    private static readonly MethodSlot GetBoundsSlot = new("GetBounds");
    private static readonly MethodSlot GetCollisionSlot = new("GetCollision");
    private static readonly MethodSlot GetDimensionSlot = new("GetDimension");
    private static readonly MethodSlot GetDistanceFromCameraSlot = new("GetDistanceFromCamera");
    private static readonly MethodSlot GetForceSlot = new("GetForce");
    private static readonly MethodSlot GetLifeSpanSlot = new("GetLifeSpan");
    private static readonly MethodSlot GetLocationSlot = new("GetLocation");
    private static readonly MethodSlot GetMassSlot = new("GetMass");
    private static readonly MethodSlot GetNetworkAuthoritySlot = new("GetNetworkAuthority");
    private static readonly MethodSlot GetRelativeLocationSlot = new("GetRelativeLocation");
    private static readonly MethodSlot GetRelativeRotationSlot = new("GetRelativeRotation");
    private static readonly MethodSlot GetRotationSlot = new("GetRotation");
    private static readonly MethodSlot GetScaleSlot = new("GetScale");
    private static readonly MethodSlot GetScreenPercentageSlot = new("GetScreenPercentage");
    private static readonly MethodSlot GetSocketTransformSlot = new("GetSocketTransform");
    private static readonly MethodSlot GetVelocitySlot = new("GetVelocity");
    private static readonly MethodSlot HasNetworkAuthoritySlot = new("HasNetworkAuthority");
    private static readonly MethodSlot IsGravityEnabledSlot = new("IsGravityEnabled");
    private static readonly MethodSlot IsInWaterSlot = new("IsInWater");
    private static readonly MethodSlot IsNetworkDistributedSlot = new("IsNetworkDistributed");
    private static readonly MethodSlot IsVisibleSlot = new("IsVisible");
    private static readonly MethodSlot RemoveActorTagSlot = new("RemoveActorTag");
    private static readonly MethodSlot RotateToSlot = new("RotateTo");
    private static readonly MethodSlot SetAngularForceSlot = new("SetAngularForce");
    private static readonly MethodSlot SetCastShadowSlot = new("SetCastShadow");
    private static readonly MethodSlot SetCollisionSlot = new("SetCollision");
    private static readonly MethodSlot SetDimensionSlot = new("SetDimension");
    private static readonly MethodSlot SetDistanceOptimizationMultiplierSlot = new("SetDistanceOptimizationMultiplier");
    private static readonly MethodSlot SetForceSlot = new("SetForce");
    private static readonly MethodSlot SetGravityEnabledSlot = new("SetGravityEnabled");
    private static readonly MethodSlot SetHighlightEnabledSlot = new("SetHighlightEnabled");
    private static readonly MethodSlot SetLifeSpanSlot = new("SetLifeSpan");
    private static readonly MethodSlot SetLocationSlot = new("SetLocation");
    private static readonly MethodSlot SetNetworkAuthoritySlot = new("SetNetworkAuthority");
    private static readonly MethodSlot SetNetworkAuthorityAutoDistributedSlot = new("SetNetworkAuthorityAutoDistributed");
    private static readonly MethodSlot SetOutlineEnabledSlot = new("SetOutlineEnabled");
    private static readonly MethodSlot SetRelativeLocationSlot = new("SetRelativeLocation");
    private static readonly MethodSlot SetRelativeRotationSlot = new("SetRelativeRotation");
    private static readonly MethodSlot SetRenderCullDistanceSlot = new("SetRenderCullDistance");
    private static readonly MethodSlot SetRotationSlot = new("SetRotation");
    private static readonly MethodSlot SetScaleSlot = new("SetScale");
    private static readonly MethodSlot SetVisibilitySlot = new("SetVisibility");
    private static readonly MethodSlot TranslateToSlot = new("TranslateTo");
    private static readonly MethodSlot WasRecentlyRenderedSlot = new("WasRecentlyRendered");

    /// <summary>Gets all Unreal Actor Tags on this Actor</summary>
    public IReadOnlyList<string> ActorTags => GetActorTags();

    /// <summary>Gets this Actor's angular force (set by <c>SetAngularForce()</c>)</summary>
    public Vector AngularForce
    {
        get => GetAngularForce();
        set => SetAngularForce(value);
    }

    /// <summary>Returns the angular velocity of this Actor</summary>
    public Vector AngularVelocity => GetAngularVelocity();

    /// <summary>Gets the Actor this Actor is attached to</summary>
    public Actor? AttachedTo => GetAttachedTo();

    /// <summary>Gets the bone or socket name this Actor is attached to</summary>
    public string AttachedToSocket => GetAttachedToSocket();

    /// <summary>Gets this Actor's bounds, useful to know its size in the world</summary>
    public IReadOnlyDictionary<string, object?> Bounds => GetBounds();

    /// <summary>Gets this Actor's collision type</summary>
    public CollisionType Collision
    {
        get => GetCollision();
        set => SetCollision(value);
    }

    /// <summary>Gets this Actor's dimension</summary>
    public uint Dimension
    {
        get => GetDimension();
        set => SetDimension(value);
    }

    /// <summary>Gets the distance of this Actor from the Camera</summary>
    public float DistanceFromCamera => GetDistanceFromCamera();

    /// <summary>Gets this Actor's force (set by <c>SetForce()</c>)</summary>
    public Vector Force
    {
        get => GetForce();
        set => SetForce(value);
    }

    /// <summary>Gets the time (in seconds) left before this Actor is destroyed</summary>
    public float LifeSpan
    {
        get => GetLifeSpan();
        set => SetLifeSpan(value);
    }

    /// <summary>Gets this Actor's location in the game world</summary>
    public Vector Location
    {
        get => GetLocation();
        set => SetLocation(value);
    }

    /// <summary>Returns the mass of this Actor</summary>
    public float Mass => GetMass();

    /// <summary>Gets this Actor's Network Authority Player</summary>
    public Player? NetworkAuthority
    {
        get => GetNetworkAuthority();
        set => SetNetworkAuthority(value);
    }

    /// <summary>Gets this Actor's Relative Location if it's attached</summary>
    public Vector RelativeLocation
    {
        get => GetRelativeLocation();
        set => SetRelativeLocation(value);
    }

    /// <summary>Gets this Actor's Relative Rotation if it's attached</summary>
    public Rotator RelativeRotation
    {
        get => GetRelativeRotation();
        set => SetRelativeRotation(value);
    }

    /// <summary>Gets this Actor's angle in the game world</summary>
    public Rotator Rotation
    {
        get => GetRotation();
        set => SetRotation(value);
    }

    /// <summary>Gets this Actor's scale</summary>
    public Vector Scale
    {
        get => GetScale();
        set => SetScale(value);
    }

    /// <summary>Gets the percentage of this Actor size in the screen</summary>
    public float ScreenPercentage => GetScreenPercentage();

    /// <summary>Gets this Actor's current velocity</summary>
    public Vector Velocity => GetVelocity();

    /// <summary>Returns true if gravity is enabled on this Actor</summary>
    public bool GravityEnabled
    {
        get => IsGravityEnabled();
        set => SetGravityEnabled(value);
    }

    /// <summary>Returns true if this Actor is in water</summary>
    public bool InWater => IsInWater();

    /// <summary>Returns true if this Actor is currently network distributed</summary>
    public bool NetworkDistributed => IsNetworkDistributed();

    /// <summary>Returns true if this Actor is visible</summary>
    public bool Visible => IsVisible();

    /// <summary>Adds an Unreal Actor Tag to this Actor</summary>
    public void AddActorTag(string tag) => Invoke(AddActorTagSlot, tag);

    /// <summary>Add an angular impulse to this Actor. Good for one time instant burst</summary>
    public void AddAngularImpulse(Vector angularImpulse, bool? velocityChange = null) =>
        Invoke(AddAngularImpulseSlot, ScriptArguments.Trim([angularImpulse, velocityChange], 1));

    /// <summary>Add an impulse to this Actor. Good for one time instant burst</summary>
    public void AddImpulse(Vector impulse, bool? velocityChange = null) =>
        Invoke(AddImpulseSlot, ScriptArguments.Trim([impulse, velocityChange], 1));

    /// <summary>Attaches this Actor to any other Actor, optionally at a specific bone</summary>
    public bool AttachTo(
        Entity other,
        AttachmentRule? attachmentRule = null,
        string? boneName = null,
        float? lifespanWhenDetached = null,
        bool? useAbsoluteRotation = null) =>
        Invoke<bool>(AttachToSlot, ScriptArguments.Trim([other, attachmentRule, boneName, lifespanWhenDetached, useAbsoluteRotation], 1))!;

    /// <summary>Detaches this Actor from AttachedTo Actor</summary>
    public void Detach() => Invoke(DetachSlot);

    /// <summary>Gets all Unreal Actor Tags on this Actor</summary>
    public IReadOnlyList<string> GetActorTags() => Invoke<IReadOnlyList<string>>(GetActorTagsSlot)!;

    /// <summary>Gets this Actor's angular force (set by <c>SetAngularForce()</c>)</summary>
    public Vector GetAngularForce() => Invoke<Vector>(GetAngularForceSlot)!;

    /// <summary>Returns the angular velocity of this Actor</summary>
    public Vector GetAngularVelocity() => Invoke<Vector>(GetAngularVelocitySlot)!;

    /// <summary>Gets all Actors attached to this Actor</summary>
    public IReadOnlyList<object?> GetAttachedEntities(bool? recursively = null) =>
        Invoke<IReadOnlyList<object?>>(GetAttachedEntitiesSlot, ScriptArguments.Trim([recursively], 0))!;

    /// <summary>Gets the Actor this Actor is attached to</summary>
    public Actor? GetAttachedTo() => Invoke<Actor?>(GetAttachedToSlot);

    /// <summary>Gets the bone or socket name this Actor is attached to</summary>
    public string GetAttachedToSocket() => Invoke<string>(GetAttachedToSocketSlot)!;

    /// <summary>Gets this Actor's bounds, useful to know its size in the world</summary>
    public IReadOnlyDictionary<string, object?> GetBounds() =>
        Invoke<IReadOnlyDictionary<string, object?>>(GetBoundsSlot)!;

    /// <summary>Gets this Actor's collision type</summary>
    public CollisionType GetCollision() => Invoke<CollisionType>(GetCollisionSlot)!;

    /// <summary>Gets this Actor's dimension</summary>
    public uint GetDimension() => Invoke<uint>(GetDimensionSlot)!;

    /// <summary>Gets the distance of this Actor from the Camera</summary>
    public float GetDistanceFromCamera() => Invoke<float>(GetDistanceFromCameraSlot)!;

    /// <summary>Gets this Actor's force (set by <c>SetForce()</c>)</summary>
    public Vector GetForce() => Invoke<Vector>(GetForceSlot)!;

    /// <summary>Gets the time (in seconds) left before this Actor is destroyed</summary>
    public float GetLifeSpan() => Invoke<float>(GetLifeSpanSlot)!;

    /// <summary>Gets this Actor's location in the game world</summary>
    public Vector GetLocation() => Invoke<Vector>(GetLocationSlot)!;

    /// <summary>Returns the mass of this Actor</summary>
    public float GetMass() => Invoke<float>(GetMassSlot)!;

    /// <summary>Gets this Actor's Network Authority Player</summary>
    public Player? GetNetworkAuthority() => Invoke<Player?>(GetNetworkAuthoritySlot);

    /// <summary>Gets this Actor's Relative Location if it's attached</summary>
    public Vector GetRelativeLocation() => Invoke<Vector>(GetRelativeLocationSlot)!;

    /// <summary>Gets this Actor's Relative Rotation if it's attached</summary>
    public Rotator GetRelativeRotation() => Invoke<Rotator>(GetRelativeRotationSlot)!;

    /// <summary>Gets this Actor's angle in the game world</summary>
    public Rotator GetRotation() => Invoke<Rotator>(GetRotationSlot)!;

    /// <summary>Gets this Actor's scale</summary>
    public Vector GetScale() => Invoke<Vector>(GetScaleSlot)!;

    /// <summary>Gets the percentage of this Actor size in the screen</summary>
    public float GetScreenPercentage() => Invoke<float>(GetScreenPercentageSlot)!;

    /// <summary>Gets a Socket Transform in world space given a bone or socket name</summary>
    public IReadOnlyDictionary<string, object?> GetSocketTransform(string socketName, string? attachableId = null) =>
        Invoke<IReadOnlyDictionary<string, object?>>(GetSocketTransformSlot, ScriptArguments.Trim([socketName, attachableId], 1))!;

    /// <summary>Gets this Actor's current velocity</summary>
    public Vector GetVelocity() => Invoke<Vector>(GetVelocitySlot)!;

    /// <summary>Returns true if the local Player is currently the Network Authority of this Actor</summary>
    public bool HasNetworkAuthority() => Invoke<bool>(HasNetworkAuthoritySlot)!;

    /// <summary>Returns true if gravity is enabled on this Actor</summary>
    public bool IsGravityEnabled() => Invoke<bool>(IsGravityEnabledSlot)!;

    /// <summary>Returns true if this Actor is in water</summary>
    public bool IsInWater() => Invoke<bool>(IsInWaterSlot)!;

    /// <summary>Returns true if this Actor is currently network distributed</summary>
    public bool IsNetworkDistributed() => Invoke<bool>(IsNetworkDistributedSlot)!;

    /// <summary>Returns true if this Actor is visible</summary>
    public bool IsVisible() => Invoke<bool>(IsVisibleSlot)!;

    /// <summary>Removes an Unreal Actor Tag from this Actor</summary>
    public void RemoveActorTag(string tag) => Invoke(RemoveActorTagSlot, tag);

    /// <summary>Smoothly rotates this actor to an angle over a certain time</summary>
    public void RotateTo(Rotator rotation, float time, float? exp = null) =>
        Invoke(RotateToSlot, ScriptArguments.Trim([rotation, time, exp], 2));

    /// <summary>Adds a permanent angular force to this Actor, set to Vector(0, 0, 0) to cancel</summary>
    public void SetAngularForce(Vector angularForce) => Invoke(SetAngularForceSlot, angularForce);

    /// <summary>Sets this Actor's to cast shadows or not</summary>
    public void SetCastShadow(bool castShadow) => Invoke(SetCastShadowSlot, castShadow);

    /// <summary>Sets this Actor's collision type</summary>
    public void SetCollision(CollisionType collisionType) => Invoke(SetCollisionSlot, collisionType);

    /// <summary>Sets this Actor's Dimension. Actors only see and interact with entities in the same Dimension</summary>
    public void SetDimension(uint dimension) => Invoke(SetDimensionSlot, dimension);

    /// <summary>Sets this Actor's distance optimization multiplier</summary>
    public void SetDistanceOptimizationMultiplier(float multiplier) =>
        Invoke(SetDistanceOptimizationMultiplierSlot, multiplier);

    /// <summary>Adds a permanent force to this Actor, set to Vector(0, 0, 0) to cancel</summary>
    public void SetForce(Vector force, bool? isLocal = null) =>
        Invoke(SetForceSlot, ScriptArguments.Trim([force, isLocal], 1));

    /// <summary>Sets whether gravity is enabled on this Actor</summary>
    public void SetGravityEnabled(bool gravityEnabled) => Invoke(SetGravityEnabledSlot, gravityEnabled);

    /// <summary>Sets whether the highlight is enabled on this Actor, and which highlight index to use</summary>
    public void SetHighlightEnabled(bool isEnabled, uint? index = null) =>
        Invoke(SetHighlightEnabledSlot, ScriptArguments.Trim([isEnabled, index], 1));

    /// <summary>Sets the time (in seconds) before this Actor is destroyed. After this time has passed, the actor will be automatically destroyed.</summary>
    public void SetLifeSpan(float seconds) => Invoke(SetLifeSpanSlot, seconds);

    /// <summary>Sets this Actor's location in the game world</summary>
    public void SetLocation(Vector location) => Invoke(SetLocationSlot, location);

    /// <summary>Sets the Player to have network authority over this Actor</summary>
    public void SetNetworkAuthority(Player? player = null, uint? argument2 = null) =>
        Invoke(SetNetworkAuthoritySlot, ScriptArguments.Trim([player, argument2], 0));

    /// <summary>Sets if this Actor will auto distribute the network authority between players</summary>
    public void SetNetworkAuthorityAutoDistributed(bool autoDistribute) =>
        Invoke(SetNetworkAuthorityAutoDistributedSlot, autoDistribute);

    /// <summary>Sets whether the outline is enabled on this Actor, and which outline index to use</summary>
    public void SetOutlineEnabled(bool isEnabled, uint? index = null) =>
        Invoke(SetOutlineEnabledSlot, ScriptArguments.Trim([isEnabled, index], 1));

    /// <summary>Sets this Actor's relative location in local space (only if this actor is attached)</summary>
    public void SetRelativeLocation(Vector relativeLocation) => Invoke(SetRelativeLocationSlot, relativeLocation);

    /// <summary>Sets this Actor's relative rotation in local space (only if this actor is attached)</summary>
    public void SetRelativeRotation(Rotator relativeRotation) => Invoke(SetRelativeRotationSlot, relativeRotation);

    /// <summary>Sets this Actor's render cull distance</summary>
    public void SetRenderCullDistance(uint maxDistance) => Invoke(SetRenderCullDistanceSlot, maxDistance);

    /// <summary>Sets this Actor's rotation in the game world</summary>
    public void SetRotation(Rotator rotation) => Invoke(SetRotationSlot, rotation);

    /// <summary>Sets this Actor's scale</summary>
    public void SetScale(Vector scale) => Invoke(SetScaleSlot, scale);

    /// <summary>Sets whether the actor is visible or not</summary>
    public void SetVisibility(bool isVisible) => Invoke(SetVisibilitySlot, isVisible);

    /// <summary>Smoothly moves this actor to a location over a certain time</summary>
    public void TranslateTo(Vector location, float time, float? exp = null) =>
        Invoke(TranslateToSlot, ScriptArguments.Trim([location, time, exp], 2));

    /// <summary>Gets if this Actor was recently rendered on screen</summary>
    public bool WasRecentlyRendered() => Invoke<bool>(WasRecentlyRenderedSlot)!;
}
