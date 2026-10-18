#nullable enable

namespace NanosSharp.Enums;

/// <summary>Collision channels, used to filter what traces and collisions hit</summary>
[Flags]
public enum CollisionChannel : long
{
    /// <summary>WorldStatic Object Types</summary>
    WorldStatic = 1,
    /// <summary>WorldDynamic Object Types</summary>
    WorldDynamic = 2,
    /// <summary>Capsules (usually from Characters)</summary>
    Pawn = 4,
    /// <summary>Pickables and Props Meshes</summary>
    PhysicsBody = 32,
    /// <summary>Primitive components used for traces</summary>
    TracePrimitive = 65536,
    /// <summary>Character Mesh</summary>
    Mesh = 131072,
    /// <summary>Water Body</summary>
    Water = 524288,
    /// <summary>Foliage Meshes</summary>
    Foliage = 1048576,
    /// <summary>Vehicles Meshes</summary>
    Vehicle = 4194304,
    /// <summary>All Objects</summary>
    All = 4294967295,
}
