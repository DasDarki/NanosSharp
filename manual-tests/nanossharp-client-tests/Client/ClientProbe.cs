public sealed class ClientProbe : Package
{
    private int _interval;
    private int _ticks;

    protected override async void OnStart()
    {
        Log.Info($"client probe started with {SharedProbe.Message}");
        _interval = ScriptTimer.SetInterval(OnInterval, 1000);
        await Scheduler.Delay(500);
        Log.Info("client probe resumed after await");
    }

    protected override void OnStop()
    {
        ScriptTimer.ClearInterval(_interval);
        Log.Info("client probe stopped");
    }

    private void OnInterval()
    {
        _ticks++;
        if (_ticks <= 3)
        {
            Log.Info($"client probe interval {_ticks}");
        }
    }
}
