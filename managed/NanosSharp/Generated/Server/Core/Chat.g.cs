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
    private static readonly StaticMethodSlot BroadcastMessageSlot = new("Chat", "BroadcastMessage");
    private static readonly StaticMethodSlot SendMessageSlot = new("Chat", "SendMessage");

    /// <summary>Sends a chat message to all Players</summary>
    public static void BroadcastMessage(string message) => ScriptApi.Call(BroadcastMessageSlot, 1, message);

    /// <summary>Sends a chat message to a Player only</summary>
    public static void SendMessage(Player player, string message) =>
        ScriptApi.Call(SendMessageSlot, 2, player, message);
}
