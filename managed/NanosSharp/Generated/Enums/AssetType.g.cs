#nullable enable

namespace NanosSharp.Enums;

/// <summary>Types of assets, used to filter assets from Asset Packs</summary>
public enum AssetType
{
    Map = 2,
    StaticMesh = 4,
    SkeletalMesh = 8,
    Sound = 16,
    Animation = 32,
    Particle = 64,
    Material = 128,
    Blueprint = 256,
    Other = 32768,
}
