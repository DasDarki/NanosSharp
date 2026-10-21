namespace NanosSharp.Injection;

/// <summary>A singleton with an asynchronous start and stop, created and managed by the package container.</summary>
public interface IService
{
    /// <summary>Runs when the service starts, on the main thread.</summary>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>Runs when the service stops, on the main thread.</summary>
    Task StopAsync(CancellationToken cancellationToken);
}
