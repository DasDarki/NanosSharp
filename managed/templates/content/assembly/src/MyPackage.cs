using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Packages;
//#if (side != "Client")
using NanosSharp.Mathematics;
//#endif

namespace MyPackage;

public sealed class MyPackage : Package
{
    protected override void OnStart()
    {
//#if (side == "Client")
        Log.Info($"{Title} started on the client.");
        Client.OnSpawnLocalPlayer += OnSpawnLocalPlayer;
//#else
        Log.Info($"{Title} started.");
        Player.OnReady += OnPlayerReady;
//#endif
    }

    protected override void OnStop()
    {
        Log.Info($"{Title} stopped.");
    }

//#if (side == "Client")
    private static void OnSpawnLocalPlayer(Player? localPlayer)
    {
        Log.Info($"The local player spawned: {localPlayer?.Name ?? "unknown"}.");
    }
//#else
    private static void OnPlayerReady(Player player)
    {
        var character = new Character(new Vector(0, 0, 200), Rotator.Zero, "nanos-world::SK_Mannequin");
        player.Possess(character);
        player.SendMessage($"Welcome {player.Name}!");
    }
//#endif
}
