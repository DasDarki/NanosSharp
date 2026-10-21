namespace NanosSharp.Injection;

/// <summary>A service whose <see cref="OnTick"/> runs on the main thread once its interval elapsed, driven by the game tick.</summary>
public abstract class ScheduledService : IService
{
    /// <summary>When true the service is not ticked.</summary>
    public bool IsDisabled { get; set; }

    private readonly double _interval;
    private double _elapsed;

    /// <summary>Creates a scheduled service, <see cref="TimeSpan.Zero"/> runs on every tick.</summary>
    protected ScheduledService(TimeSpan interval) => _interval = Math.Max(interval.TotalSeconds, 0);

    Task IService.StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    Task IService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>Runs on the main thread when the interval elapsed, with the seconds since the previous run.</summary>
    protected abstract void OnTick(float deltaTime);

    internal void Advance(float deltaTime)
    {
        if (IsDisabled)
        {
            return;
        }

        _elapsed += deltaTime;
        if (_elapsed < _interval)
        {
            return;
        }

        var elapsed = (float)_elapsed;
        _elapsed = 0;
        OnTick(elapsed);
    }
}
