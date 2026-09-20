#nullable enable

namespace NanosSharp.Enums;

/// <summary>Used when spawning Entities to control when they finish spawning and are sent to clients, so several configs can be set at once without wasting networking and performance on partially spawned Entities</summary>
public enum SpawnMode
{
    /// <summary>Finishes spawning right away when calling the constructor</summary>
    Immediate = 0,
    /// <summary>Keeps the spawn deferred. An Inherited Class finishes it automatically when its Constructor returns, otherwise it behaves like <c>SpawnMode.Manual</c> for non-inherited classes</summary>
    AfterConstructor = 1,
    /// <summary>Keeps the spawn deferred until you call <c>FinishSpawn()</c> yourself</summary>
    Manual = 2,
}
