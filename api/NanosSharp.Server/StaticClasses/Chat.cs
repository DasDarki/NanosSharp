using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Chat class in C#.
/// </summary>
public static class Chat
{
    /// <summary>
    /// Sends a chat message to all Players
    /// </summary>
    public static void BroadcastMessage(string message)
    {
        ServerModule.EnsureMainThread();
        
        BChat.BroadcastMessage(ServerModule.VM, message);
    }

    /// <summary>
    /// Sends a chat message to a specific Player
    /// </summary>
    public static void SendMessage(Player player, string message)
    {
        ServerModule.EnsureMainThread();
        
        BChat.SendMessage(ServerModule.VM, player, message);
    }
}