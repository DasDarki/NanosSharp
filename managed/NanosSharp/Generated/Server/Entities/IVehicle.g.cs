#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

public partial interface IVehicle
{

    /// <summary>Checks if the engine is started</summary>
    bool EngineStarted { get; }

    /// <summary>Adds a Door to the Vehicle</summary>
    void SetDoor(byte seatIndex, Vector offsetLocation, Vector seatLocation, Rotator seatRotation, ushort triggerRadius, ushort leaveLateralOffset);
}
