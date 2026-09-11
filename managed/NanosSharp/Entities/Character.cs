using NanosSharp.Entities.Binding;
using NanosSharp.Mathematics;

namespace NanosSharp.Entities;

/// <summary>A humanoid character that can be possessed by players.</summary>
[ScriptClass("Character")]
public sealed class Character : Actor
{
    /// <summary>Default mannequin mesh shipped with nanos world.</summary>
    public const string DefaultMesh = "nanos-world::SK_Mannequin";

    /// <summary>Current health.</summary>
    public int Health
    {
        get => Invoke<int>("GetHealth");
        set => Invoke("SetHealth", value);
    }

    /// <summary>Maximum health.</summary>
    public int MaxHealth
    {
        get => Invoke<int>("GetMaxHealth");
        set => Invoke("SetMaxHealth", value);
    }

    /// <summary>Whether the character is dead.</summary>
    public bool IsDead => Invoke<bool>("IsDead");

    /// <summary>Player controlling the character.</summary>
    public Player? Player => Invoke<Player>("GetPlayer");

    /// <summary>Team of the character.</summary>
    public int Team
    {
        get => Invoke<int>("GetTeam");
        set => Invoke("SetTeam", value);
    }

    private Character()
    {
    }

    /// <summary>Spawns a new character.</summary>
    public static Character Spawn(Vector location, Rotator rotation, string mesh = DefaultMesh) =>
        Spawn<Character>(location, rotation, mesh);

    /// <summary>Applies damage to the character and returns the damage taken.</summary>
    public int ApplyDamage(int damage) => Invoke<int>("ApplyDamage", damage);

    /// <summary>Respawns the character.</summary>
    public void Respawn() => Invoke("Respawn");

    /// <summary>Makes the character jump.</summary>
    public void Jump() => Invoke("Jump");
}
