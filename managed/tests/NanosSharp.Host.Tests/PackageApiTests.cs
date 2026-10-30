using NanosSharp.Injection;
using NanosSharp.Packages;

namespace Example.Packages;

public interface IGreeter
{
    string Greet();
}

public interface IFarewell
{
    string Bye();
}

public sealed class Greeter : IGreeter
{
    public string Greet() => "hi";
}

[PackageApi]
public sealed class DeclaredApi : IGreeter, IFarewell
{
    public string Greet() => "declared";

    public string Bye() => "bye";
}

[PackageApi(typeof(IFarewell))]
public sealed class ExplicitApi : IGreeter, IFarewell
{
    public string Greet() => "explicit";

    public string Bye() => "explicit-bye";
}

public sealed class PackageApiTests
{
    [Fact]
    public void ProvidesAndResolvesByContract()
    {
        var greeter = new Greeter();
        using (PackageContext.Enter(new PackageScope("provider", "/tmp")))
        {
            PackageApi.Provide<IGreeter>(greeter);
        }

        Assert.Same(greeter, PackageApi.Require<IGreeter>());
        Assert.Same(greeter, PackageApi.Get<IGreeter>());
        Assert.True(PackageApi.TryGet<IGreeter>(out var resolved));
        Assert.Same(greeter, resolved);
        PackageApiRegistry.ReleaseScope("provider");
    }

    [Fact]
    public void ReturnsNothingWhenAbsent()
    {
        Assert.Null(PackageApi.Get<IGreeter>());
        Assert.False(PackageApi.TryGet<IGreeter>(out _));
        Assert.Throws<InvalidOperationException>(() => PackageApi.Require<IGreeter>());
    }

    [Fact]
    public void ReleaseScopeRemovesTheProvidedApi()
    {
        PackageApiRegistry.Publish("provider", typeof(IGreeter), new Greeter());
        Assert.NotNull(PackageApi.Get<IGreeter>());

        PackageApiRegistry.ReleaseScope("provider");
        Assert.Null(PackageApi.Get<IGreeter>());
    }

    [Fact]
    public void ConflictingProvidersThrow()
    {
        PackageApiRegistry.Publish("first", typeof(IGreeter), new Greeter());
        Assert.Throws<InvalidOperationException>(() =>
            PackageApiRegistry.Publish("second", typeof(IGreeter), new Greeter()));
        PackageApiRegistry.ReleaseScope("first");
    }

    [Fact]
    public void HandleReResolvesAcrossReloads()
    {
        var handle = PackageApi.Of<IGreeter>();
        Assert.False(handle.IsAvailable);

        var first = new Greeter();
        PackageApiRegistry.Publish("provider", typeof(IGreeter), first);
        Assert.Same(first, handle.Value);

        PackageApiRegistry.ReleaseScope("provider");
        Assert.Null(handle.Value);

        var second = new Greeter();
        PackageApiRegistry.Publish("provider", typeof(IGreeter), second);
        Assert.Same(second, handle.Value);
        PackageApiRegistry.ReleaseScope("provider");
    }

    [Fact]
    public void PublishDeclaredRegistersEveryContract()
    {
        var instance = new DeclaredApi();
        PackageApiRegistry.PublishDeclared("provider", [typeof(DeclaredApi)],
            type => type == typeof(DeclaredApi) ? instance : null);

        Assert.Same(instance, PackageApi.Get<IGreeter>());
        Assert.Same(instance, PackageApi.Get<IFarewell>());
        Assert.Same(instance, PackageApi.Get<DeclaredApi>());
        PackageApiRegistry.ReleaseScope("provider");
    }

    [Fact]
    public void PublishDeclaredHonorsExplicitContracts()
    {
        var instance = new ExplicitApi();
        PackageApiRegistry.PublishDeclared("provider", [typeof(ExplicitApi)],
            type => type == typeof(ExplicitApi) ? instance : null);

        Assert.Same(instance, PackageApi.Get<IFarewell>());
        Assert.Same(instance, PackageApi.Get<ExplicitApi>());
        Assert.Null(PackageApi.Get<IGreeter>());
        PackageApiRegistry.ReleaseScope("provider");
    }

    [Fact]
    public void ServiceContainerFallsBackToTheRegistry()
    {
        var container = new ServiceContainer(new PackageScope("consumer", "/tmp"));
        var greeter = new Greeter();
        PackageApiRegistry.Publish("provider", typeof(IGreeter), greeter);

        Assert.Same(greeter, container.Get<IGreeter>());
        Assert.True(container.Any<IGreeter>());

        PackageApiRegistry.ReleaseScope("provider");
        Assert.Null(container.Get<IGreeter>());
    }
}
