namespace NanosSharp.Injection;

/// <summary>Base class for services that only needs the lifecycle methods it actually uses.</summary>
public abstract class Service : IService
{
    /// <summary>When true the service is not started, stopping it still runs.</summary>
    public bool IsDisabled { get; set; }

    /// <summary>Runs when the service starts, on the main thread.</summary>
    public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>Runs when the service stops, on the main thread.</summary>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
