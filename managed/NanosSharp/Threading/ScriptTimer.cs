using NanosSharp.Entities;

namespace NanosSharp.Threading;

/// <summary>Timers running on the main thread, mirroring the Lua Timer class.</summary>
public static class ScriptTimer
{
    /// <summary>Calls the callback repeatedly, returning false from it stops the interval.</summary>
    public static int SetInterval(Func<bool> callback, int milliseconds = 1) => TimerScheduler.Add(callback, milliseconds, true);

    /// <summary>Calls the callback repeatedly.</summary>
    public static int SetInterval(Action callback, int milliseconds = 1) => TimerScheduler.Add(Wrap(callback), milliseconds, true);

    /// <summary>Calls the callback once after the delay.</summary>
    public static int SetTimeout(Action callback, int milliseconds = 1) => TimerScheduler.Add(Wrap(callback), milliseconds, false);

    /// <summary>Stops an interval.</summary>
    public static void ClearInterval(int id) => TimerScheduler.Remove(id);

    /// <summary>Stops a timeout.</summary>
    public static void ClearTimeout(int id) => TimerScheduler.Remove(id);

    /// <summary>Stops the timer automatically when the entity is destroyed.</summary>
    public static void Bind(int id, Entity entity) => TimerScheduler.Bind(id, entity);

    /// <summary>Whether the timer is still scheduled.</summary>
    public static bool IsValid(int id) => TimerScheduler.Find(id) is not null;

    /// <summary>Milliseconds since the timer was started or last fired.</summary>
    public static long GetElapsedTime(int id)
    {
        var timer = TimerScheduler.Get(id);
        return timer.IsPaused ? timer.Interval - timer.RemainingWhilePaused!.Value : TimerScheduler.Now - timer.StartedAt;
    }

    /// <summary>Milliseconds until the timer fires.</summary>
    public static long GetRemainingTime(int id)
    {
        var timer = TimerScheduler.Get(id);
        return timer.RemainingWhilePaused ?? Math.Max(timer.DueAt - TimerScheduler.Now, 0);
    }

    /// <summary>Changes the milliseconds until the timer fires.</summary>
    public static void SetRemainingTime(int id, long milliseconds)
    {
        var timer = TimerScheduler.Get(id);
        if (timer.IsPaused)
        {
            timer.RemainingWhilePaused = milliseconds;
            return;
        }

        timer.DueAt = TimerScheduler.Now + milliseconds;
    }

    /// <summary>Restarts the waiting time of the timer.</summary>
    public static void ResetElapsedTime(int id) => TimerScheduler.Restart(TimerScheduler.Get(id));

    /// <summary>Pauses the timer.</summary>
    public static void Pause(int id)
    {
        var timer = TimerScheduler.Get(id);
        timer.RemainingWhilePaused ??= Math.Max(timer.DueAt - TimerScheduler.Now, 0);
    }

    /// <summary>Resumes a paused timer.</summary>
    public static void Resume(int id)
    {
        var timer = TimerScheduler.Get(id);
        if (timer.RemainingWhilePaused is not { } remaining)
        {
            return;
        }

        timer.DueAt = TimerScheduler.Now + remaining;
        timer.StartedAt = timer.DueAt - timer.Interval;
        timer.RemainingWhilePaused = null;
    }

    private static Func<bool> Wrap(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return () =>
        {
            callback();
            return true;
        };
    }
}
