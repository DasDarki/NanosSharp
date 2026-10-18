#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

public static partial class Chat
{
    /// <summary>Called when a new Chat Message is received, this is also triggered when new messages are sent programatically</summary>
    public static event ChatEntryHandler? OnChatEntry
    {
        add => ChatEntryEvent.Add(value);
        remove => ChatEntryEvent.Remove(value);
    }

    /// <summary>When player closes the Chat</summary>
    public static event CloseHandler? OnClose
    {
        add => CloseEvent.Add(value);
        remove => CloseEvent.Remove(value);
    }

    /// <summary>When player opens the Chat</summary>
    public static event OpenHandler? OnOpen
    {
        add => OpenEvent.Add(value);
        remove => OpenEvent.Remove(value);
    }

    private static readonly ClassEvent<ChatEntryHandler> ChatEntryEvent = new("Chat", "ChatEntry");
    private static readonly ClassEvent<CloseHandler> CloseEvent = new("Chat", "Close");
    private static readonly ClassEvent<OpenHandler> OpenEvent = new("Chat", "Open");
    private static readonly StaticMethodSlot AddMessageSlot = new("Chat", "AddMessage");
    private static readonly StaticMethodSlot ClearSlot = new("Chat", "Clear");
    private static readonly StaticMethodSlot SetConfigurationSlot = new("Chat", "SetConfiguration");
    private static readonly StaticMethodSlot SetVisibilitySlot = new("Chat", "SetVisibility");

    /// <summary>Adds a chat message which will display local only</summary>
    public static void AddMessage(string message) => ScriptApi.Call(AddMessageSlot, 1, message);

    /// <summary>Clears all messages</summary>
    public static void Clear() => ScriptApi.Call(ClearSlot, 0);

    /// <summary>Configures the Chat visuals and position</summary>
    public static void SetConfiguration(
        Vector2D? screenLocation = null,
        Vector2D? size = null,
        Vector2D? anchorsMin = null,
        Vector2D? anchorsMax = null,
        Vector2D? alignment = null,
        bool? justify = null,
        bool? showScrollbar = null) =>
        ScriptApi.Call(SetConfigurationSlot, 0, screenLocation, size, anchorsMin, anchorsMax, alignment, justify, showScrollbar);

    /// <summary>Sets if the Chat is visible or not</summary>
    public static void SetVisibility(bool isVisible) => ScriptApi.Call(SetVisibilitySlot, 1, isVisible);

    /// <summary>Called when a new Chat Message is received, this is also triggered when new messages are sent programatically</summary>
    public delegate void ChatEntryHandler(string? message, Player? player);

    /// <summary>When player closes the Chat</summary>
    public delegate void CloseHandler();

    /// <summary>When player opens the Chat</summary>
    public delegate void OpenHandler();
}
