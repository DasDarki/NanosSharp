#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Retrieve Assets Data from Asset Packs.</summary>
public static partial class Assets
{
    /// <summary>Gets a list containing information about all loaded Asset Packs</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> AssetPacks => GetAssetPacks();

    /// <summary>Gets a list containing all Animation Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetAnimations(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetAnimations", assetPackPath)!;

    /// <summary>Gets a list containing information about all loaded Asset Packs</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> GetAssetPacks() =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<string, string>>>("Assets", "GetAssetPacks")!;

    /// <summary>Gets the file path of an asset</summary>
    public static string GetAssetPath(string asset, AssetType assetType) =>
        ScriptApi.Invoke<string>("Assets", "GetAssetPath", asset, assetType)!;

    /// <summary>Gets a list containing all Blueprints Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetBlueprints(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetBlueprints", assetPackPath)!;

    /// <summary>Gets a list containing all Map Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetMaps(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetMaps", assetPackPath)!;

    /// <summary>Gets a list containing all Materials Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetMaterials(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetMaterials", assetPackPath)!;

    /// <summary>Gets a list containing all Other Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetOthers(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetOthers", assetPackPath)!;

    /// <summary>Gets a list containing all Particle Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetParticles(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetParticles", assetPackPath)!;

    /// <summary>Gets a list containing all Skeletal Mesh Asset Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetSkeletalMeshes(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetSkeletalMeshes", assetPackPath)!;

    /// <summary>Gets a list containing all Sound Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetSounds(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetSounds", assetPackPath)!;

    /// <summary>Gets a list containing all Static Mesh Assets Keys from an AssetPack</summary>
    public static IReadOnlyList<IReadOnlyDictionary<object?, object?>> GetStaticMeshes(string assetPackPath) =>
        ScriptApi.Invoke<IReadOnlyList<IReadOnlyDictionary<object?, object?>>>("Assets", "GetStaticMeshes", assetPackPath)!;

    /// <summary>Manually adds an Asset to be loaded during the Player's loading screen</summary>
    public static void Precache(string assetPath, AssetType assetType) =>
        ScriptApi.Invoke("Assets", "Precache", assetPath, assetType);
}
