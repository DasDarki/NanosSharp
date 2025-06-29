using NanosSharp.Server.Configuration;
using NanosSharp.Server.Pool;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Package class in C#. In the C# context this class is also an abstract
/// class for all C# packages to inherit from. Each main class must inherit from this class to be recognized as one
/// and there can only be one main class per package.
/// <br/><br/>
/// <b>Important note</b>: Due to the way how NanosSharp handles package class initialization, API functions and objects
/// in the main class are not initialized until the constructor call is finished. This means that you cannot use these
/// API functions or objects in the constructor of the main class.
/// </summary>
public abstract partial class Package
{
    /// <summary>
    /// The logger for this package. It is used to log messages to the console or to a file.
    /// </summary>
    public Logger Logger { get; internal set; } = null!;
    
    /// <summary>
    /// The configuration for this package. It contains the settings defined in the package's configuration file.
    /// </summary>
    public PackageConfig Config { get; internal set; } = null!;
    
    /// <summary>
    /// The path to the directory where this package is located.
    /// </summary>
    public string Directory { get; internal set; } = string.Empty;
    
    /// <summary>
    /// Gets called by NanosSharp runtime when the package is loaded.
    /// </summary>
    public abstract void OnStart();
    
    /// <summary>
    /// Gets called by NanosSharp runtime when the package is unloaded.
    /// </summary>
    public virtual void OnStop() {}
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Blueprint"/> type. This factory is used to create instances of blueprints.
    /// This can be overridden to provide a custom factory for blueprints if needed.
    /// </summary>
    public virtual IEntityFactory<Blueprint> GetBlueprintFactory() => new DefaultEntityFactory<Blueprint>(r => new Blueprint(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Cable"/> type. This factory is used to create instances of cables.
    /// This can be overridden to provide a custom factory for cables if needed.
    /// </summary>
    public virtual IEntityFactory<Cable> GetCableFactory() => new DefaultEntityFactory<Cable>(r => new Cable(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Character"/> type. This factory is used to create instances of characters.
    /// This can be overridden to provide a custom factory for characters if needed.
    /// </summary>
    public virtual IEntityFactory<Character> GetCharacterFactory() => new DefaultEntityFactory<Character>(r => new Character(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="CharacterSimple"/> type. This factory is used to create instances of simple characters.
    /// This can be overridden to provide a custom factory for simple characters if needed.
    /// </summary>
    public virtual IEntityFactory<CharacterSimple> GetCharacterSimpleFactory() => new DefaultEntityFactory<CharacterSimple>(r => new CharacterSimple(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Database"/> type. This factory is used to create instances of databases.
    /// This can be overridden to provide a custom factory for databases if needed.
    /// </summary>
    public virtual IEntityFactory<Database> GetDatabaseFactory() => new DefaultEntityFactory<Database>(r => new Database(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="File"/> type. This factory is used to create instances of files.
    /// This can be overridden to provide a custom factory for files if needed.
    /// </summary>
    public virtual IEntityFactory<File> GetFileFactory() => new DefaultEntityFactory<File>(r => new File(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Grenade"/> type. This factory is used to create instances of grenades.
    /// This can be overridden to provide a custom factory for grenades if needed.
    /// </summary>
    public virtual IEntityFactory<Grenade> GetGrenadeFactory() => new DefaultEntityFactory<Grenade>(r => new Grenade(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Light"/> type. This factory is used to create instances of lights.
    /// This can be overridden to provide a custom factory for lights if needed.
    /// </summary>
    public virtual IEntityFactory<Light> GetLightFactory() => new DefaultEntityFactory<Light>(r => new Light(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Melee"/> type. This factory is used to create instances of melee weapons.
    /// This can be overridden to provide a custom factory for melee weapons if needed.
    /// </summary>
    public virtual IEntityFactory<Melee> GetMeleeFactory() => new DefaultEntityFactory<Melee>(r => new Melee(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Particle"/> type. This factory is used to create instances of particles.
    /// This can be overridden to provide a custom factory for particles if needed.
    /// </summary>
    public virtual IEntityFactory<Particle> GetParticleFactory() => new DefaultEntityFactory<Particle>(r => new Particle(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Player"/> type. This factory is used to create instances of players.
    /// This can be overridden to provide a custom factory for players if needed.
    /// </summary>
    public virtual IEntityFactory<Player> GetPlayerFactory() => new DefaultEntityFactory<Player>(r => new Player(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Prop"/> type. This factory is used to create instances of props.
    /// This can be overridden to provide a custom factory for props if needed.
    /// </summary>
    public virtual IEntityFactory<Prop> GetPropFactory() => new DefaultEntityFactory<Prop>(r => new Prop(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="StaticMesh"/> type. This factory is used to create instances of static meshes.
    /// /// This can be overridden to provide a custom factory for static meshes if needed.
    /// </summary>
    public virtual IEntityFactory<StaticMesh> GetStaticMeshFactory() => new DefaultEntityFactory<StaticMesh>(r => new StaticMesh(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="TextRenderer"/> type. This factory is used to create instances of text renderers.
    /// This can be overridden to provide a custom factory for text renderers if needed.
    /// </summary>
    public virtual IEntityFactory<TextRenderer> GetTextRendererFactory() => new DefaultEntityFactory<TextRenderer>(r => new TextRenderer(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Trigger"/> type. This factory is used to create instances of triggers.
    /// This can be overridden to provide a custom factory for triggers if needed.
    /// </summary>
    public virtual IEntityFactory<Trigger> GetTriggerFactory() => new DefaultEntityFactory<Trigger>(r => new Trigger(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="VehicleWater"/> type. This factory is used to create instances of water vehicles.
    /// This can be overridden to provide a custom factory for water vehicles if needed.
    /// </summary>
    public virtual IEntityFactory<VehicleWater> GetVehicleWaterFactory() => new DefaultEntityFactory<VehicleWater>(r => new VehicleWater(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="VehicleWheeled"/> type. This factory is used to create instances of wheeled vehicles.
    /// This can be overridden to provide a custom factory for wheeled vehicles if needed.
    /// </summary>
    public virtual IEntityFactory<VehicleWheeled> GetVehicleWheeledFactory() => new DefaultEntityFactory<VehicleWheeled>(r => new VehicleWheeled(r));
    
    /// <summary>
    /// Returns the entity factory for the <see cref="Weapon"/> type. This factory is used to create instances of weapons.
    /// This can be overridden to provide a custom factory for weapons if needed.
    /// </summary>
    public virtual IEntityFactory<Weapon> GetWeaponFactory() => new DefaultEntityFactory<Weapon>(r => new Weapon(r));
}