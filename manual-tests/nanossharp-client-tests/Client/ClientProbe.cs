public sealed class ClientProbe : Package
{
    protected override void OnStart() => Log.Info($"client probe started with {SharedProbe.Message}");

    protected override void OnStop() => Log.Info("client probe stopped");
}
