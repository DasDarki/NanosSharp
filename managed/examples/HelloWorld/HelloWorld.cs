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
        Player.OnReady += OnPlayerReady;
    }

    protected override void OnStop() => Log.Info("Hello World stopped.");

    private static void OnPlayerReady(Player player)
    {
        var character = new Character(new Vector(0, 0, 200), Rotator.Zero, "nanos-world::SK_Mannequin");
        player.Possess(character);
        player.SendMessage($"Welcome {player.Name}, you are running on NanosSharp!");
    }
}
