#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Retrieve Assets Data from Asset Packs.</summary>
public static partial class Assets
{
    private static readonly StaticMethodSlot GetAnimationsSlot = new("Assets", "GetAnimations");
    private static readonly StaticMethodSlot GetAssetPacksSlot = new("Assets", "GetAssetPacks");
    private static readonly StaticMethodSlot GetAssetPathSlot = new("Assets", "GetAssetPath");
    private static readonly StaticMethodSlot GetBlueprintsSlot = new("Assets", "GetBlueprints");
    private static readonly StaticMethodSlot GetMapsSlot = new("Assets", "GetMaps");
    private static readonly StaticMethodSlot GetMaterialsSlot = new("Assets", "GetMaterials");
    private static readonly StaticMethodSlot GetOthersSlot = new("Assets", "GetOthers");
    private static readonly StaticMethodSlot GetParticlesSlot = new("Assets", "GetParticles");
    private static readonly StaticMethodSlot GetSkeletalMeshesSlot = new("Assets", "GetSkeletalMeshes");
    private static readonly StaticMethodSlot GetSoundsSlot = new("Assets", "GetSounds");
    private static readonly StaticMethodSlot GetStaticMeshesSlot = new("Assets", "GetStaticMeshes");
    private static readonly StaticMethodSlot PrecacheSlot = new("Assets", "Precache");

    /// <summary>Gets a list containing information about all loaded Asset Packs</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> AssetPacks => GetAssetPacks();

    /// <summary>Gets a list containing all Animation Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetAnimations(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetAnimationsSlot, assetPackPath)!;

    /// <summary>Gets a list containing information about all loaded Asset Packs</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> GetAssetPacks() =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<string, string>>>(GetAssetPacksSlot)!;

    /// <summary>Gets the file path of an asset</summary>
    public static string GetAssetPath(string asset, AssetType assetType) =>
        ScriptApi.Invoke<string>(GetAssetPathSlot, asset, assetType)!;

    /// <summary>Gets a list containing all Blueprints Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetBlueprints(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetBlueprintsSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Map Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetMaps(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetMapsSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Materials Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetMaterials(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetMaterialsSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Other Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetOthers(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetOthersSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Particle Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetParticles(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetParticlesSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Skeletal Mesh Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetSkeletalMeshes(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetSkeletalMeshesSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Sound Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetSounds(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetSoundsSlot, assetPackPath)!;

    /// <summary>Gets a list containing all Static Mesh Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetStaticMeshes(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>(GetStaticMeshesSlot, assetPackPath)!;

    /// <summary>Manually adds an Asset to be loaded during the Player's loading screen</summary>
    public static void Precache(string assetPath, AssetType assetType) =>
        ScriptApi.Invoke(PrecacheSlot, assetPath, assetType);
}
