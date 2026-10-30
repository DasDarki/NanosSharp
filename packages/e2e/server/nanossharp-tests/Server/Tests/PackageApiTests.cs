using ApiProvider;
using NanosSharp.Packages;
using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class PackageApiTests
{
    [Test]
    public static void RequireResolvesTheProvidedApi()
    {
        var counter = PackageApi.Require<ICounterApi>();
        Assert.True(counter is not null, "ICounterApi was not provided by the provider package");
    }

    [Test]
    public static void TypedCallCrossesThePackageBoundary()
    {
        var counter = PackageApi.Require<ICounterApi>();
        var before = counter.Value;
        var result = counter.Increment(5);
        Assert.Equal(before + 5, result);
        Assert.Equal(result, counter.Value);
    }

    [Test]
    public static void HandleResolvesTheSameInstance()
    {
        var handle = PackageApi.Of<ICounterApi>();
        Assert.True(handle.IsAvailable, "the PackageApi handle did not resolve the contract");
        Assert.True(ReferenceEquals(handle.Value, PackageApi.Require<ICounterApi>()), "handle and Require returned different instances");
    }

    [Test]
    public static void ConcreteTypeIsResolvable()
    {
        Assert.True(PackageApi.TryGet<CounterApi>(out var counter), "the concrete CounterApi type was not resolvable");
        Assert.True(counter is not null, "the resolved concrete instance is null");
    }
}
