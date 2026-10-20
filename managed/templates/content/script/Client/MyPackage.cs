public sealed class MyPackage : Package
{
    protected override void OnStart() => Log.Info($"{Title} started on the {Shared.Side}.");

    protected override void OnStop() => Log.Info($"{Title} stopped.");
}
