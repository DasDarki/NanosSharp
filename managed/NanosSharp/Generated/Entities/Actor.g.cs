#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>An Actor is an object which can be spawned and destroyed through Lua.Actors support 3D transformations such as translation (location), rotation, and scale.An Actor is the base for several entities, and all those entities share the same Methods and Events described in this page.</summary>
public abstract partial class Actor : Entity
{
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
    public void AddActorTag(string tag) => Invoke("AddActorTag", tag);

    /// <summary>Add an angular impulse to this Actor. Good for one time instant burst</summary>
    public void AddAngularImpulse(Vector angularImpulse, bool? velocityChange = null) =>
        Invoke("AddAngularImpulse", ScriptArguments.Trim([angularImpulse, velocityChange], 1));

    /// <summary>Add an impulse to this Actor. Good for one time instant burst</summary>
    public void AddImpulse(Vector impulse, bool? velocityChange = null) =>
        Invoke("AddImpulse", ScriptArguments.Trim([impulse, velocityChange], 1));

    /// <summary>Attaches this Actor to any other Actor, optionally at a specific bone</summary>
    public bool AttachTo(
        Entity other,
        AttachmentRule? attachmentRule = null,
        string? boneName = null,
        float? lifespanWhenDetached = null,
        bool? useAbsoluteRotation = null) =>
        Invoke<bool>("AttachTo", ScriptArguments.Trim([other, attachmentRule, boneName, lifespanWhenDetached, useAbsoluteRotation], 1))!;

    /// <summary>Detaches this Actor from AttachedTo Actor</summary>
    public void Detach() => Invoke("Detach");

    /// <summary>Gets all Unreal Actor Tags on this Actor</summary>
    public IReadOnlyList<string> GetActorTags() => Invoke<IReadOnlyList<string>>("GetActorTags")!;

    /// <summary>Gets this Actor's angular force (set by <c>SetAngularForce()</c>)</summary>
    public Vector GetAngularForce() => Invoke<Vector>("GetAngularForce")!;

    /// <summary>Returns the angular velocity of this Actor</summary>
    public Vector GetAngularVelocity() => Invoke<Vector>("GetAngularVelocity")!;

    /// <summary>Gets all Actors attached to this Actor</summary>
    public IReadOnlyList<object?> GetAttachedEntities(bool? recursively = null) =>
        Invoke<IReadOnlyList<object?>>("GetAttachedEntities", ScriptArguments.Trim([recursively], 0))!;

    /// <summary>Gets the Actor this Actor is attached to</summary>
    public Actor? GetAttachedTo() => Invoke<Actor?>("GetAttachedTo");

    /// <summary>Gets the bone or socket name this Actor is attached to</summary>
    public string GetAttachedToSocket() => Invoke<string>("GetAttachedToSocket")!;

    /// <summary>Gets this Actor's bounds, useful to know its size in the world</summary>
    public IReadOnlyDictionary<string, object?> GetBounds() =>
        Invoke<IReadOnlyDictionary<string, object?>>("GetBounds")!;

    /// <summary>Gets this Actor's collision type</summary>
    public CollisionType GetCollision() => Invoke<CollisionType>("GetCollision")!;

    /// <summary>Gets this Actor's dimension</summary>
    public uint GetDimension() => Invoke<uint>("GetDimension")!;

    /// <summary>Gets the distance of this Actor from the Camera</summary>
    public float GetDistanceFromCamera() => Invoke<float>("GetDistanceFromCamera")!;

    /// <summary>Gets this Actor's force (set by <c>SetForce()</c>)</summary>
    public Vector GetForce() => Invoke<Vector>("GetForce")!;

    /// <summary>Gets the time (in seconds) left before this Actor is destroyed</summary>
    public float GetLifeSpan() => Invoke<float>("GetLifeSpan")!;

    /// <summary>Gets this Actor's location in the game world</summary>
    public Vector GetLocation() => Invoke<Vector>("GetLocation")!;

    /// <summary>Returns the mass of this Actor</summary>
    public float GetMass() => Invoke<float>("GetMass")!;

    /// <summary>Gets this Actor's Network Authority Player</summary>
    public Player? GetNetworkAuthority() => Invoke<Player?>("GetNetworkAuthority");

    /// <summary>Gets this Actor's Relative Location if it's attached</summary>
    public Vector GetRelativeLocation() => Invoke<Vector>("GetRelativeLocation")!;

    /// <summary>Gets this Actor's Relative Rotation if it's attached</summary>
    public Rotator GetRelativeRotation() => Invoke<Rotator>("GetRelativeRotation")!;

    /// <summary>Gets this Actor's angle in the game world</summary>
    public Rotator GetRotation() => Invoke<Rotator>("GetRotation")!;

    /// <summary>Gets this Actor's scale</summary>
    public Vector GetScale() => Invoke<Vector>("GetScale")!;

    /// <summary>Gets the percentage of this Actor size in the screen</summary>
    public float GetScreenPercentage() => Invoke<float>("GetScreenPercentage")!;

    /// <summary>Gets a Socket Transform in world space given a bone or socket name</summary>
    public IReadOnlyDictionary<string, object?> GetSocketTransform(string socketName, string? attachableId = null) =>
        Invoke<IReadOnlyDictionary<string, object?>>("GetSocketTransform", ScriptArguments.Trim([socketName, attachableId], 1))!;

    /// <summary>Gets this Actor's current velocity</summary>
    public Vector GetVelocity() => Invoke<Vector>("GetVelocity")!;

    /// <summary>Returns true if the local Player is currently the Network Authority of this Actor</summary>
    public bool HasNetworkAuthority() => Invoke<bool>("HasNetworkAuthority")!;

    /// <summary>Returns true if gravity is enabled on this Actor</summary>
    public bool IsGravityEnabled() => Invoke<bool>("IsGravityEnabled")!;

    /// <summary>Returns true if this Actor is in water</summary>
    public bool IsInWater() => Invoke<bool>("IsInWater")!;

    /// <summary>Returns true if this Actor is currently network distributed</summary>
    public bool IsNetworkDistributed() => Invoke<bool>("IsNetworkDistributed")!;

    /// <summary>Returns true if this Actor is visible</summary>
    public bool IsVisible() => Invoke<bool>("IsVisible")!;

    /// <summary>Removes an Unreal Actor Tag from this Actor</summary>
    public void RemoveActorTag(string tag) => Invoke("RemoveActorTag", tag);

    /// <summary>Smoothly rotates this actor to an angle over a certain time</summary>
    public void RotateTo(Rotator rotation, float time, float? exp = null) =>
        Invoke("RotateTo", ScriptArguments.Trim([rotation, time, exp], 2));

    /// <summary>Adds a permanent angular force to this Actor, set to Vector(0, 0, 0) to cancel</summary>
    public void SetAngularForce(Vector angularForce) => Invoke("SetAngularForce", angularForce);

    /// <summary>Sets this Actor's to cast shadows or not</summary>
    public void SetCastShadow(bool castShadow) => Invoke("SetCastShadow", castShadow);

    /// <summary>Sets this Actor's collision type</summary>
    public void SetCollision(CollisionType collisionType) => Invoke("SetCollision", collisionType);

    /// <summary>Sets this Actor's Dimension. Actors only see and interact with entities in the same Dimension</summary>
    public void SetDimension(uint dimension) => Invoke("SetDimension", dimension);

    /// <summary>Sets this Actor's distance optimization multiplier</summary>
    public void SetDistanceOptimizationMultiplier(float multiplier) =>
        Invoke("SetDistanceOptimizationMultiplier", multiplier);

    /// <summary>Adds a permanent force to this Actor, set to Vector(0, 0, 0) to cancel</summary>
    public void SetForce(Vector force, bool? isLocal = null) =>
        Invoke("SetForce", ScriptArguments.Trim([force, isLocal], 1));

    /// <summary>Sets whether gravity is enabled on this Actor</summary>
    public void SetGravityEnabled(bool gravityEnabled) => Invoke("SetGravityEnabled", gravityEnabled);

    /// <summary>Sets whether the highlight is enabled on this Actor, and which highlight index to use</summary>
    public void SetHighlightEnabled(bool isEnabled, uint? index = null) =>
        Invoke("SetHighlightEnabled", ScriptArguments.Trim([isEnabled, index], 1));

    /// <summary>Sets the time (in seconds) before this Actor is destroyed. After this time has passed, the actor will be automatically destroyed.</summary>
    public void SetLifeSpan(float seconds) => Invoke("SetLifeSpan", seconds);

    /// <summary>Sets this Actor's location in the game world</summary>
    public void SetLocation(Vector location) => Invoke("SetLocation", location);

    /// <summary>Sets the Player to have network authority over this Actor</summary>
    public void SetNetworkAuthority(Player? player = null, uint? argument2 = null) =>
        Invoke("SetNetworkAuthority", ScriptArguments.Trim([player, argument2], 0));

    /// <summary>Sets if this Actor will auto distribute the network authority between players</summary>
    public void SetNetworkAuthorityAutoDistributed(bool autoDistribute) =>
        Invoke("SetNetworkAuthorityAutoDistributed", autoDistribute);

    /// <summary>Sets whether the outline is enabled on this Actor, and which outline index to use</summary>
    public void SetOutlineEnabled(bool isEnabled, uint? index = null) =>
        Invoke("SetOutlineEnabled", ScriptArguments.Trim([isEnabled, index], 1));

    /// <summary>Sets this Actor's relative location in local space (only if this actor is attached)</summary>
    public void SetRelativeLocation(Vector relativeLocation) => Invoke("SetRelativeLocation", relativeLocation);

    /// <summary>Sets this Actor's relative rotation in local space (only if this actor is attached)</summary>
    public void SetRelativeRotation(Rotator relativeRotation) => Invoke("SetRelativeRotation", relativeRotation);

    /// <summary>Sets this Actor's render cull distance</summary>
    public void SetRenderCullDistance(uint maxDistance) => Invoke("SetRenderCullDistance", maxDistance);

    /// <summary>Sets this Actor's rotation in the game world</summary>
    public void SetRotation(Rotator rotation) => Invoke("SetRotation", rotation);

    /// <summary>Sets this Actor's scale</summary>
    public void SetScale(Vector scale) => Invoke("SetScale", scale);

    /// <summary>Sets whether the actor is visible or not</summary>
    public void SetVisibility(bool isVisible) => Invoke("SetVisibility", isVisible);

    /// <summary>Smoothly moves this actor to a location over a certain time</summary>
    public void TranslateTo(Vector location, float time, float? exp = null) =>
        Invoke("TranslateTo", ScriptArguments.Trim([location, time, exp], 2));

    /// <summary>Gets if this Actor was recently rendered on screen</summary>
    public bool WasRecentlyRendered() => Invoke<bool>("WasRecentlyRendered")!;
}
