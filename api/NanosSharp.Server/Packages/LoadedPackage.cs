using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.Server.Pool;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The internal representation of a loaded package in the NanosSharp server. This holds internal state and metadata about the package
/// as well as the package's main class.
/// </summary>
internal sealed class LoadedPackage
{
    public AssemblyLoadContext Context { get; }
    
    public Assembly Assembly { get; }
    
    public Package MainClass { get; }
    
    public EntityPool<Blueprint> Blueprints { get; }
    
    public EntityPool<Cable> Cables { get; }
    
    public EntityPool<Character> Characters { get; }
    
    public EntityPool<CharacterSimple> CharactersSimple { get; }
    
    public EntityPool<Database> Databases { get; }
    
    public EntityPool<File> Files { get; }
    
    public EntityPool<Grenade> Grenades { get; }
    
    public EntityPool<Light> Lights { get; }
    
    public EntityPool<Melee> Melees { get; }
    
    public EntityPool<Particle> Particles { get; }
    
    public EntityPool<Player> Players { get; }
    
    public EntityPool<Prop> Props { get; }
    
    public EntityPool<StaticMesh> StaticMeshes { get; }
    
    public EntityPool<TextRenderer> TextRenderers { get; }
    
    public EntityPool<Trigger> Triggers { get; }
    
    public EntityPool<VehicleWater> VehiclesWater { get; }
    
    public EntityPool<VehicleWheeled> VehicleWheeled { get; }
    
    public EntityPool<Weapon> Weapons { get; }

    internal LoadedPackage(AssemblyLoadContext context, Assembly assembly, Package package)
    {
        Context = context;
        Assembly = assembly;
        MainClass = package;
        Blueprints = new EntityPool<Blueprint>(package.GetBlueprintFactory());
        Cables = new EntityPool<Cable>(package.GetCableFactory());
        Characters = new EntityPool<Character>(package.GetCharacterFactory());
        CharactersSimple = new EntityPool<CharacterSimple>(package.GetCharacterSimpleFactory());
        Databases = new EntityPool<Database>(package.GetDatabaseFactory());
        Files = new EntityPool<File>(package.GetFileFactory());
        Grenades = new EntityPool<Grenade>(package.GetGrenadeFactory());
        Lights = new EntityPool<Light>(package.GetLightFactory());
        Melees = new EntityPool<Melee>(package.GetMeleeFactory());
        Particles = new EntityPool<Particle>(package.GetParticleFactory());
        Players = new EntityPool<Player>(package.GetPlayerFactory());
        Props = new EntityPool<Prop>(package.GetPropFactory());
        StaticMeshes = new EntityPool<StaticMesh>(package.GetStaticMeshFactory());
        TextRenderers = new EntityPool<TextRenderer>(package.GetTextRendererFactory());
        Triggers = new EntityPool<Trigger>(package.GetTriggerFactory());
        VehiclesWater = new EntityPool<VehicleWater>(package.GetVehicleWaterFactory());
        VehicleWheeled = new EntityPool<VehicleWheeled>(package.GetVehicleWheeledFactory());
        Weapons = new EntityPool<Weapon>(package.GetWeaponFactory());
    }
}