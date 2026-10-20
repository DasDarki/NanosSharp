namespace MyPackage;

public sealed class MyPackage : Package
{
    protected override void OnStart() => Log.Info($"{Title} started, protocol version {Protocol.Version}.");

    protected override void OnStop() => Log.Info($"{Title} stopped.");
}
