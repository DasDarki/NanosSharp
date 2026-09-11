using NanosSharp.Mathematics;

namespace NanosSharp.Entities;

/// <summary>An entity placed in the world.</summary>
public abstract class Actor : Entity
{
    /// <summary>World location.</summary>
    public Vector Location
    {
        get => Invoke<Vector>("GetLocation");
        set => Invoke("SetLocation", value);
    }

    /// <summary>World rotation.</summary>
    public Rotator Rotation
    {
        get => Invoke<Rotator>("GetRotation");
        set => Invoke("SetRotation", value);
    }

    /// <summary>World scale.</summary>
    public Vector Scale
    {
        get => Invoke<Vector>("GetScale");
        set => Invoke("SetScale", value);
    }

    /// <summary>Current velocity.</summary>
    public Vector Velocity => Invoke<Vector>("GetVelocity");

    /// <summary>Whether the actor is visible.</summary>
    public bool IsVisible
    {
        get => Invoke<bool>("IsVisible");
        set => Invoke("SetVisibility", value);
    }

    /// <summary>Whether gravity affects the actor.</summary>
    public bool IsGravityEnabled
    {
        get => Invoke<bool>("IsGravityEnabled");
        set => Invoke("SetGravityEnabled", value);
    }

    /// <summary>Adds an impulse to the actor.</summary>
    public void AddImpulse(Vector force, bool velocityChange = false) => Invoke("AddImpulse", force, velocityChange);
}
