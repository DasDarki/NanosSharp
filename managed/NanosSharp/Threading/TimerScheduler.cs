using System.Diagnostics;
using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Packages;

namespace NanosSharp.Threading;

internal static class TimerScheduler
{
    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static readonly Dictionary<int, ScheduledTimer> Timers = [];
    private static readonly List<(TaskCompletionSource Completion, long DueAt, PackageScope? Scope)> Delays = [];
    private static int _nextId;

    public static long Now => Clock.ElapsedMilliseconds;

    public static int Add(Func<bool> callback, long interval, bool isRepeating)
    {
        MainThread.Ensure();
        ArgumentNullException.ThrowIfNull(callback);
        var timer = new ScheduledTimer(++_nextId, callback, Math.Max(interval, 0), isRepeating, PackageContext.Current);
        Restart(timer);
        Timers[timer.Id] = timer;
        return timer.Id;
    }

    public static ScheduledTimer? Find(int id) => Timers.GetValueOrDefault(id);

    public static ScheduledTimer Get(int id) =>
        Timers.TryGetValue(id, out var timer) ? timer : throw new ArgumentException($"Timer {id} does not exist.", nameof(id));

    public static void Remove(int id)
    {
        if (!Timers.Remove(id, out var timer) || timer.BoundEntity is null)
        {
            return;
        }

        timer.BoundEntity.Destroyed -= OnBoundEntityDestroyed;
    }

    public static void Bind(int id, Entity entity)
    {
        var timer = Get(id);
        timer.BoundEntity = entity;
        entity.Destroyed += OnBoundEntityDestroyed;
    }

    public static void Restart(ScheduledTimer timer)
    {
        timer.StartedAt = Now;
        timer.DueAt = timer.StartedAt + timer.Interval;
    }

    public static Task Delay(long milliseconds, CancellationToken cancellationToken)
    {
        MainThread.Ensure();
        var completion = new TaskCompletionSource();
        cancellationToken.Register(() => MainThread.Post(() => completion.TrySetCanceled(cancellationToken)));
        Delays.Add((completion, Now + Math.Max(milliseconds, 0), PackageContext.Current));
        return completion.Task;
    }

    public static void Tick()
    {
        var now = Now;
        CompleteDelays(now);
        var due = Timers.Values.Where(timer => !timer.IsPaused && timer.DueAt <= now).OrderBy(timer => timer.DueAt).ThenBy(timer => timer.Id).ToArray();
        foreach (var timer in due)
        {
            Run(timer);
        }
    }

    public static void ReleaseScope(PackageScope scope)
    {
        foreach (var timer in Timers.Values.Where(timer => timer.Scope == scope).ToArray())
        {
            Remove(timer.Id);
        }

        foreach (var delay in Delays.Where(delay => delay.Scope == scope).ToArray())
        {
            Delays.Remove(delay);
            delay.Completion.TrySetCanceled();
        }
    }

    private static void CompleteDelays(long now)
    {
        var completed = Delays.Where(delay => delay.DueAt <= now).ToArray();
        foreach (var delay in completed)
        {
            Delays.Remove(delay);
            delay.Completion.TrySetResult();
        }
    }

    private static void Run(ScheduledTimer timer)
    {
        if (!Timers.ContainsKey(timer.Id))
        {
            return;
        }

        var keepRunning = Invoke(timer);
        if (!timer.IsRepeating || !keepRunning)
        {
            Remove(timer.Id);
            return;
        }

        timer.StartedAt = Now;
        timer.DueAt = Math.Max(timer.DueAt + timer.Interval, timer.StartedAt);
    }

    private static bool Invoke(ScheduledTimer timer)
    {
        using var _ = PackageContext.Enter(timer.Scope);
        try
        {
            return timer.Callback();
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"Unhandled exception in timer {timer.Id}");
            return true;
        }
    }

    private static void OnBoundEntityDestroyed(Entity entity)
    {
        foreach (var timer in Timers.Values.Where(timer => ReferenceEquals(timer.BoundEntity, entity)).ToArray())
        {
            Timers.Remove(timer.Id);
        }
    }
}
