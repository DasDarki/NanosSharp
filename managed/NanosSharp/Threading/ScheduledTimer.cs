using NanosSharp.Entities;
using NanosSharp.Packages;

namespace NanosSharp.Threading;

internal sealed class ScheduledTimer(int id, Func<bool> callback, long interval, bool isRepeating, PackageScope? scope)
{
    public int Id { get; } = id;

    public Func<bool> Callback { get; } = callback;

    public long Interval { get; } = interval;

    public bool IsRepeating { get; } = isRepeating;

    public PackageScope? Scope { get; } = scope;

    public long StartedAt { get; set; }

    public long DueAt { get; set; }

    public long? RemainingWhilePaused { get; set; }

    public Entity? BoundEntity { get; set; }

    public bool IsPaused => RemainingWhilePaused.HasValue;
}
