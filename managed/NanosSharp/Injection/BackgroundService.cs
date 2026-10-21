namespace NanosSharp.Injection;

/// <summary>A service that runs work off the main thread, it must not access nanos world directly.</summary>
public abstract class BackgroundService : IService
{
    /// <summary>Runs the background work until the token is cancelled.</summary>
    public abstract Task StartAsync(CancellationToken cancellationToken);

    /// <summary>Runs when the service stops, after its token was cancelled.</summary>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
