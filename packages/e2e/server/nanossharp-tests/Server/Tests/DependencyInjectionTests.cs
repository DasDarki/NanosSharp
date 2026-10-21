using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class DependencyInjectionTests
{
    [Test]
    public static void ServicesAreDiscoveredInjectedAndStarted()
    {
        Assert.True(CounterService.WasStarted, "IService.StartAsync did not run");
        Assert.True(InjectionProbe.Instance is not null, "The IAutoRegister singleton was not created");
        Assert.True(InjectionProbe.Instance!.Initialized, "[OnInit] did not run");
        Assert.True(InjectionProbe.Instance.CounterInjected, "[Inject] did not resolve the service");
        Assert.True(InjectionProbe.Instance.ResolvedThroughContainer, "ServiceContainer.Get did not resolve the service");
    }

    [Test]
    public static async Task AutoRegisterServiceHandlesEvents()
    {
        var before = InjectionProbe.Instance!.Ticks;
        await Scheduler.Delay(TimeSpan.FromMilliseconds(200));
        Assert.True(InjectionProbe.Instance.Ticks > before, "The IAutoRegister event handler did not fire");
    }

    [Test]
    public static async Task ScheduledServiceRunsOnTheTick()
    {
        var before = TickCounterService.Ticks;
        await Scheduler.Delay(TimeSpan.FromMilliseconds(200));
        Assert.True(TickCounterService.Ticks > before, "The scheduled service did not tick");
    }
}

public sealed class CounterService : Service
{
    public static bool WasStarted { get; private set; }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        WasStarted = true;
        return Task.CompletedTask;
    }
}

public sealed class InjectionProbe : IAutoRegister
{
    public static InjectionProbe? Instance { get; private set; }

    [Inject] private CounterService _counter = null!;
    [Inject] private ServiceContainer _container = null!;

    public bool Initialized { get; private set; }

    public bool CounterInjected { get; private set; }

    public bool ResolvedThroughContainer { get; private set; }

    public int Ticks { get; private set; }

    public InjectionProbe() => Instance = this;

    [OnInit]
    private void Initialize()
    {
        Initialized = true;
        CounterInjected = _counter is not null;
        ResolvedThroughContainer = _container.Get<CounterService>() is not null;
    }

    [ServerEvent]
    private void OnTick(ServerTickEvent tickEvent) => Ticks++;
}

public sealed class TickCounterService : ScheduledService
{
    public static int Ticks { get; private set; }

    public TickCounterService() : base(TimeSpan.Zero)
    {
    }

    protected override void OnTick(float deltaTime) => Ticks++;
}
