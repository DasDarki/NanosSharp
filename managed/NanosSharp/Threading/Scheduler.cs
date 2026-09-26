namespace NanosSharp.Threading;

/// <summary>Awaitable delays aligned to the server tick, continuations run on the main thread.</summary>
public static class Scheduler
{
    /// <summary>Completes after the delay on the main thread, it is cancelled when the package stops.</summary>
    public static Task Delay(TimeSpan delay, CancellationToken cancellationToken = default) =>
        TimerScheduler.Delay((long)delay.TotalMilliseconds, cancellationToken);

    /// <summary>Completes after the given milliseconds on the main thread, it is cancelled when the package stops.</summary>
    public static Task Delay(int milliseconds, CancellationToken cancellationToken = default) =>
        TimerScheduler.Delay(milliseconds, cancellationToken);

    /// <summary>Completes during the next server tick.</summary>
    public static Task NextTick() => TimerScheduler.Delay(0, CancellationToken.None);
}
