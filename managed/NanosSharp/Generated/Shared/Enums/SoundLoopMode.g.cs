#nullable enable

namespace NanosSharp.Enums;

/// <summary>Whether a Sound loops</summary>
public enum SoundLoopMode
{
    /// <summary>Default value defined in the asset</summary>
    Default = 0,
    /// <summary>Overrides it to make it loop forever</summary>
    Forever = 1,
    /// <summary>Overrides it to make it play once</summary>
    Never = 2,
}
