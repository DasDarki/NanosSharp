#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial class Player
{
    private static readonly MethodSlot GetCameraArmLengthSlot = new("GetCameraArmLength");
    private static readonly MethodSlot GetCameraLocationSlot = new("GetCameraLocation");
    private static readonly MethodSlot GetCameraRotationSlot = new("GetCameraRotation");
    private static readonly MethodSlot IsLocalPlayerSlot = new("IsLocalPlayer");

    /// <summary>Gets this Player's dimension</summary>
    public uint Dimension => GetDimension();

    /// <summary>Gets the Player's name</summary>
    public string Name => GetName();

    /// <summary>Gets the voice channel of the Player</summary>
    public uint VOIPChannel => GetVOIPChannel();

    /// <summary>Gets the location of the Player's camera</summary>
    public Vector CameraLocation
    {
        get => GetCameraLocation();
        set => SetCameraLocation(value);
    }

    /// <summary>Gets the rotation of the Player's camera</summary>
    public Rotator CameraRotation
    {
        get => GetCameraRotation();
        set => SetCameraRotation(value);
    }

    /// <summary>Gets if this Player is the local Player (the one running this client)</summary>
    public bool LocalPlayer => IsLocalPlayer();

    /// <summary>Gets the Player's camera arm (spring arm) length</summary>
    public float GetCameraArmLength(bool? returnBase = null) =>
        CallResult<float, bool?>(GetCameraArmLengthSlot, 0, returnBase)!;

    /// <summary>Gets the location of the Player's camera</summary>
    public Vector GetCameraLocation() => CallResult<Vector>(GetCameraLocationSlot, 0)!;

    /// <summary>Gets the rotation of the Player's camera</summary>
    public Rotator GetCameraRotation() => CallResult<Rotator>(GetCameraRotationSlot, 0)!;

    /// <summary>Gets if this Player is the local Player (the one running this client)</summary>
    public bool IsLocalPlayer() => CallResult<bool>(IsLocalPlayerSlot, 0)!;
}
