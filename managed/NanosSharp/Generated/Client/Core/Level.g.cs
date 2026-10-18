#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Work with Unreal level in runtime.</summary>
public static partial class Level
{
    /// <summary>Called when streaming pause begins</summary>
    public static event StreamLevelBeginPauseHandler? OnStreamLevelBeginPause
    {
        add => StreamLevelBeginPauseEvent.Add(value);
        remove => StreamLevelBeginPauseEvent.Remove(value);
    }

    /// <summary>Called when streaming pause ends</summary>
    public static event StreamLevelEndPauseHandler? OnStreamLevelEndPause
    {
        add => StreamLevelEndPauseEvent.Add(value);
        remove => StreamLevelEndPauseEvent.Remove(value);
    }

    /// <summary>Called when a Stream Level is hidden</summary>
    public static event StreamLevelHideHandler? OnStreamLevelHide
    {
        add => StreamLevelHideEvent.Add(value);
        remove => StreamLevelHideEvent.Remove(value);
    }

    /// <summary>Called when a Stream Level is loaded</summary>
    public static event StreamLevelLoadHandler? OnStreamLevelLoad
    {
        add => StreamLevelLoadEvent.Add(value);
        remove => StreamLevelLoadEvent.Remove(value);
    }

    /// <summary>Called when a Stream Level is shown</summary>
    public static event StreamLevelShowHandler? OnStreamLevelShow
    {
        add => StreamLevelShowEvent.Add(value);
        remove => StreamLevelShowEvent.Remove(value);
    }

    /// <summary>Called when a Stream Level is unloaded</summary>
    public static event StreamLevelUnloadHandler? OnStreamLevelUnload
    {
        add => StreamLevelUnloadEvent.Add(value);
        remove => StreamLevelUnloadEvent.Remove(value);
    }

    private static readonly ClassEvent<StreamLevelBeginPauseHandler> StreamLevelBeginPauseEvent = new("Level", "StreamLevelBeginPause");
    private static readonly ClassEvent<StreamLevelEndPauseHandler> StreamLevelEndPauseEvent = new("Level", "StreamLevelEndPause");
    private static readonly ClassEvent<StreamLevelHideHandler> StreamLevelHideEvent = new("Level", "StreamLevelHide");
    private static readonly ClassEvent<StreamLevelLoadHandler> StreamLevelLoadEvent = new("Level", "StreamLevelLoad");
    private static readonly ClassEvent<StreamLevelShowHandler> StreamLevelShowEvent = new("Level", "StreamLevelShow");
    private static readonly ClassEvent<StreamLevelUnloadHandler> StreamLevelUnloadEvent = new("Level", "StreamLevelUnload");
    private static readonly StaticMethodSlot CallLevelBlueprintEventSlot = new("Level", "CallLevelBlueprintEvent");
    private static readonly StaticMethodSlot GetStreamLevelsSlot = new("Level", "GetStreamLevels");
    private static readonly StaticMethodSlot LoadStreamLevelSlot = new("Level", "LoadStreamLevel");
    private static readonly StaticMethodSlot SetStreamLevelVisibilitySlot = new("Level", "SetStreamLevelVisibility");
    private static readonly StaticMethodSlot UnloadStreamLevelSlot = new("Level", "UnloadStreamLevel");

    /// <summary>Gets a list of all Stream Levels</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> StreamLevels => GetStreamLevels();

    /// <summary>Calls a Level Blueprint custom event (which can be added when creating levels through Unreal Engine)</summary>
    public static object?[] CallLevelBlueprintEvent(string eventName, params object?[] arguments) =>
        ScriptApi.InvokeVariadic<object?[]>(CallLevelBlueprintEventSlot, [eventName], arguments)!;

    /// <summary>Gets a list of all Stream Levels</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, object?>> GetStreamLevels() =>
        ScriptApi.CallResult<IReadOnlyList<IReadOnlyDictionary<string, object?>>>(GetStreamLevelsSlot, 0)!;

    /// <summary>Loads a Level in runtime</summary>
    public static void LoadStreamLevel(
        string levelName,
        bool? shouldBlockOnLoad = null,
        bool? makeVisibleAfterLoad = null) =>
        ScriptApi.Call(LoadStreamLevelSlot, 1, levelName, shouldBlockOnLoad, makeVisibleAfterLoad);

    /// <summary>Sets a Stream Level visibility</summary>
    public static void SetStreamLevelVisibility(string levelName, bool visibility) =>
        ScriptApi.Call(SetStreamLevelVisibilitySlot, 2, levelName, visibility);

    /// <summary>Unloads a Level in runtime</summary>
    public static void UnloadStreamLevel(string levelName, bool? shouldBlockOnUnload = null) =>
        ScriptApi.Call(UnloadStreamLevelSlot, 1, levelName, shouldBlockOnUnload);

    /// <summary>Called when streaming pause begins</summary>
    public delegate void StreamLevelBeginPauseHandler();

    /// <summary>Called when streaming pause ends</summary>
    public delegate void StreamLevelEndPauseHandler();

    /// <summary>Called when a Stream Level is hidden</summary>
    public delegate void StreamLevelHideHandler(string? levelName);

    /// <summary>Called when a Stream Level is loaded</summary>
    public delegate void StreamLevelLoadHandler(string? levelName);

    /// <summary>Called when a Stream Level is shown</summary>
    public delegate void StreamLevelShowHandler(string? levelName);

    /// <summary>Called when a Stream Level is unloaded</summary>
    public delegate void StreamLevelUnloadHandler(string? levelName);
}
