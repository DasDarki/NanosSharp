using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Mathematics;
using NanosSharp.Packages;

namespace MyPackage;

public sealed class MyPackage : Package
{
    protected override void OnStart()
    {
        Log.Info($"{Title} started.");
        Player.OnReady += OnPlayerReady;
    }

    protected override void OnStop() => Log.Info($"{Title} stopped.");

    private static void OnPlayerReady(Player player)
    {
        var character = new Character(new Vector(0, 0, 200), Rotator.Zero, "nanos-world::SK_Mannequin");
        player.Possess(character);
        player.SendMessage($"Welcome {player.Name}!");
    }
}
