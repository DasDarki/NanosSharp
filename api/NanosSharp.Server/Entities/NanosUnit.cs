using NanosSharp.API;

namespace NanosSharp.Server;

/// <summary>
/// This is a unit in the nanos world ecosystem. It does not add any functionality rather being a base class for all entities and actors.
/// It contains the <see cref="LuaRef"/> which is the reference to the lua object.
/// </summary>
public abstract class NanosUnit(LuaRef handle) : ILuaUnit
{
    /// <summary>
    /// The handle to the lua object.
    /// </summary>
    public LuaRef Handle { get; } = handle;
    
    /// <summary>
    /// Casts a <see cref="NanosUnit"/> to a <see cref="LuaRef"/> implicitly.
    /// </summary>
    public static implicit operator LuaRef(NanosUnit unit) => unit.Handle;
}