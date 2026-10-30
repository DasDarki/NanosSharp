namespace ApiProvider;

public interface ICounterApi
{
    int Value { get; }

    int Increment(int amount);
}

[PackageApi]
public sealed class CounterApi : ICounterApi
{
    private int _value;

    public int Value => _value;

    public int Increment(int amount) => _value += amount;
}

public sealed class ProviderPackage : Package
{
    protected override void OnStart() => Log.Info("API provider started.");
}
