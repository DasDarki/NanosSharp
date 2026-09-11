using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Mathematics;
using NanosSharp.Packages;

namespace HelloWorld;

public sealed class HelloWorld : Package
{
    protected override void OnStart()
    {
        Log.Info($"Hello World from a compiled assembly in package '{Name}'.");
        Player.Ready += OnPlayerReady;
        Chat.PlayerSubmit += OnChatMessage;
    }

    protected override void OnStop() => Log.Info("Hello World stopped.");

    private static void OnPlayerReady(Player player)
    {
        var character = Character.Spawn(new Vector(0, 0, 200), Rotator.Zero);
        player.Possess(character);
        player.SendMessage($"Welcome {player.Name}, you are running on NanosSharp!");
    }

    private static bool OnChatMessage(string message, Player player)
    {
        if (message != "/packages")
        {
            return true;
        }

        var packages = Server.GetPackages().Select(package => package["name"]);
        player.SendMessage($"Loaded packages: {string.Join(", ", packages)}");
        return false;
    }
}
