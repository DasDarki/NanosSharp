using NanosSharp.Entities.Binding;
using NanosSharp.Mathematics;

namespace NanosSharp.Entities;

/// <summary>A physics driven static mesh.</summary>
[ScriptClass("Prop")]
public sealed class Prop : Actor
{
    /// <summary>Default cube mesh shipped with nanos world.</summary>
    public const string DefaultMesh = "nanos-world::SM_Cube";

    private Prop()
    {
    }

    /// <summary>Spawns a new prop.</summary>
    public static Prop Spawn(Vector location, Rotator rotation, string mesh = DefaultMesh) =>
        Spawn<Prop>(location, rotation, mesh);
}
