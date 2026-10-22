using NanosSharp.Injection;
using NanosSharp.Logging;
using NanosSharp.Packages;

namespace WasmSmokePackage;

public sealed class Greeter : Service
{
    public string Message => "hello from an injected service";

    public override Task StartAsync(CancellationToken token) => Task.CompletedTask;
}

public sealed class Entry : Package
{
    [Inject]
    private Greeter _greeter = null!;

    protected override void OnStart()
    {
        Log.Info("hello from a runtime-loaded package (via Log.Info)");
        Log.Info($"injected service says: {_greeter.Message}");
    }

    protected override void OnStop()
    {
        Log.Info("goodbye from the runtime-loaded package");
    }
}
