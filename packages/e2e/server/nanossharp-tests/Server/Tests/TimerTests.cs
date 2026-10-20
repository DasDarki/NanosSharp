using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class TimerTests
{
    [Test]
    public static async Task TimeoutFiresOnceOnMainThread()
    {
        var calls = 0;
        var onMainThread = false;
        var id = ScriptTimer.SetTimeout(() =>
        {
            calls++;
            onMainThread = MainThread.IsCurrent;
        }, 50);

        await Scheduler.Delay(250);
        Assert.Equal(1, calls, "Timeout calls");
        Assert.True(onMainThread, "Timeout did not run on the main thread");
        Assert.True(!ScriptTimer.IsValid(id), "Timeout is still scheduled");
    }

    [Test]
    public static async Task IntervalStopsWhenCallbackReturnsFalse()
    {
        var calls = 0;
        var id = ScriptTimer.SetInterval(() => ++calls < 3, 20);

        await Scheduler.Delay(400);
        Assert.Equal(3, calls, "Interval calls");
        Assert.True(!ScriptTimer.IsValid(id), "Interval is still scheduled");
    }

    [Test]
    public static async Task PausedTimerDoesNotFire()
    {
        var calls = 0;
        var id = ScriptTimer.SetTimeout(() => calls++, 100);
        ScriptTimer.Pause(id);

        await Scheduler.Delay(250);
        Assert.Equal(0, calls, "Calls while paused");

        ScriptTimer.Resume(id);
        await Scheduler.Delay(250);
        Assert.Equal(1, calls, "Calls after resume");
    }

    [Test]
    public static async Task BoundTimerStopsWithEntity()
    {
        var prop = new Prop(Vector.Zero, Rotator.Zero, "nanos-world::SM_Cube");
        var calls = 0;
        var id = ScriptTimer.SetInterval(() => calls++, 20);
        ScriptTimer.Bind(id, prop);
        prop.Destroy();

        await Scheduler.Delay(150);
        Assert.Equal(0, calls, "Calls after the bound entity was destroyed");
        Assert.True(!ScriptTimer.IsValid(id), "Bound timer is still scheduled");
    }

    [Test]
    public static async Task NextTickContinuesOnMainThread()
    {
        await Scheduler.NextTick();
        Assert.True(MainThread.IsCurrent, "Continuation is not on the main thread");
    }
}
