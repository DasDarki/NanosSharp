public sealed class MyPackage : Package
{
    protected override void OnStart()
    {
        Log.Info($"{Title} started on the {Shared.Side}.");
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
