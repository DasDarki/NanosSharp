using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Assets class in C#.
/// </summary>
public static class Assets
{
    /// <summary>
    /// Gets a list containing all Animation Assets Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetAnimations(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var animations = BAssets.GetAnimations(ServerModule.VM, assetPackPath);
        if (animations == null)
        {
            yield break;
        }

        foreach (var animation in animations)
        {
            var key = animation["key"] as string;
            animation.Remove("key");

            yield return new AssetMetaData(key!, animation);
        }
    }

    /// <summary>
    /// Gets a list containing information about all loaded Asset Packs
    /// </summary>
    public static IEnumerable<AssetPack> GetAssetPacks()
    {
        ServerModule.EnsureMainThread();
        
        var assetPacks = BAssets.GetAssetPacks(ServerModule.VM);
        if (assetPacks == null)
        {
            yield break;
        }
        
        foreach (var assetPack in assetPacks)
        {
            yield return new AssetPack(
                (assetPack["name"] as string)!,
                (assetPack["path"] as string)!,
                (assetPack["author"] as string)!,
                (assetPack["version"] as string)!
            );
        }
    }

    /// <summary>
    /// Gets the file path of an asset
    /// </summary>
    /// <param name="asset">The asset reference in the format asset-pack::AssetKey.</param>
    /// <param name="type">The asset type.</param>
    public static string GetAssetPath(string asset, AssetType type)
    {
        ServerModule.EnsureMainThread();
        
        return BAssets.GetAssetPath(ServerModule.VM, asset, type);
    }
    
    /// <summary>
    /// Gets a list containing all Blueprints Assets Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetBlueprints(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var blueprints = BAssets.GetBlueprints(ServerModule.VM, assetPackPath);
        if (blueprints == null)
        {
            yield break;
        }

        foreach (var blueprint in blueprints)
        {
            var key = blueprint["key"] as string;
            blueprint.Remove("key");

            yield return new AssetMetaData(key!, blueprint);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Map Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetMaps(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var maps = BAssets.GetMaps(ServerModule.VM, assetPackPath);
        if (maps == null)
        {
            yield break;
        }

        foreach (var map in maps)
        {
            var key = map["key"] as string;
            map.Remove("key");

            yield return new AssetMetaData(key!, map);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Material Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetMaterials(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var materials = BAssets.GetMaterials(ServerModule.VM, assetPackPath);
        if (materials == null)
        {
            yield break;
        }

        foreach (var material in materials)
        {
            var key = material["key"] as string;
            material.Remove("key");

            yield return new AssetMetaData(key!, material);
        }
    }
    
    /// <summary>
    /// Gets a list containing all other Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetOthers(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var others = BAssets.GetOthers(ServerModule.VM, assetPackPath);
        if (others == null)
        {
            yield break;
        }

        foreach (var other in others)
        {
            var key = other["key"] as string;
            other.Remove("key");

            yield return new AssetMetaData(key!, other);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Particle Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetParticles(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var particles = BAssets.GetParticles(ServerModule.VM, assetPackPath);
        if (particles == null)
        {
            yield break;
        }

        foreach (var particle in particles)
        {
            var key = particle["key"] as string;
            particle.Remove("key");

            yield return new AssetMetaData(key!, particle);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Skeletal Mesh Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetSkeletalMeshes(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var skeletalMeshes = BAssets.GetSkeletalMeshes(ServerModule.VM, assetPackPath);
        if (skeletalMeshes == null)
        {
            yield break;
        }

        foreach (var skeletalMesh in skeletalMeshes)
        {
            var key = skeletalMesh["key"] as string;
            skeletalMesh.Remove("key");

            yield return new AssetMetaData(key!, skeletalMesh);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Sound Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetSounds(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var sounds = BAssets.GetSounds(ServerModule.VM, assetPackPath);
        if (sounds == null)
        {
            yield break;
        }

        foreach (var sound in sounds)
        {
            var key = sound["key"] as string;
            sound.Remove("key");

            yield return new AssetMetaData(key!, sound);
        }
    }
    
    /// <summary>
    /// Gets a list containing all Static Mesh Asset Keys from an AssetPack.
    /// </summary>
    /// <param name="assetPackPath">The path to the asset pack.</param>
    public static IEnumerable<AssetMetaData> GetStaticMeshes(string assetPackPath)
    {
        ServerModule.EnsureMainThread();
        
        var staticMeshes = BAssets.GetStaticMeshes(ServerModule.VM, assetPackPath);
        if (staticMeshes == null)
        {
            yield break;
        }

        foreach (var staticMesh in staticMeshes)
        {
            var key = staticMesh["key"] as string;
            staticMesh.Remove("key");

            yield return new AssetMetaData(key!, staticMesh);
        }
    }

    /// <summary>
    /// Manually adds an Asset to be loaded during the Player's loading screen.
    /// </summary>
    public static void Precache(string assetPath, AssetType type)
    {
        ServerModule.EnsureMainThread();
        
        BAssets.Precache(ServerModule.VM, assetPath, type);
    }
}