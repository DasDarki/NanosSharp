using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ValueTests
{
    [Test]
    public static void PrimitivesRoundTrip()
    {
        var values = Echo(true, 42L, 1.5, "nanos ✓", null);
        Assert.Equal(true, values[0]);
        Assert.Equal(42L, values[1]);
        Assert.Equal(1.5, values[2]);
        Assert.Equal("nanos ✓", values[3]);
        Assert.Equal(null, values[4]);
    }

    [Test]
    public static void StructsRoundTrip()
    {
        var values = Echo(new Vector(1, 2, 3), new Rotator(10, 20, 30), new Color(0.5f, 0.25f, 1f), new Vector2D(4, 5));
        Assert.Equal(new Vector(1, 2, 3), values[0]);
        Assert.Equal(new Rotator(10, 20, 30), values[1]);
        Assert.Equal(new Color(0.5f, 0.25f, 1f), values[2]);
        Assert.Equal(new Vector2D(4, 5), values[3]);
    }

    [Test]
    public static void CollectionsRoundTrip()
    {
        var map = new Dictionary<string, object?> { ["name"] = "test", ["list"] = new object?[] { 1, "two" } };
        var values = Echo(new[] { 1, 2, 3 }, map);

        var array = (object?[])values[0]!;
        Assert.Equal(3, array.Length);
        Assert.Equal(2L, array[1]);

        var result = (Dictionary<object, object?>)values[1]!;
        Assert.Equal("test", result["name"]);
        Assert.Equal("two", ((object?[])result["list"]!)[1]);
    }

    [Test]
    public static void TypedHandlerParametersAreConverted()
    {
        (int Number, float Fraction, string Text, Vector Location)? received = null;
        using var subscription = Events.Subscribe("nanossharp-test-typed",
            (int number, float fraction, string text, Vector location) => received = (number, fraction, text, location));
        Events.Call("nanossharp-test-typed", 7, 0.5, "typed", new Vector(1, 1, 1));

        Assert.True(received.HasValue, "Handler was not invoked");
        Assert.Equal((7, 0.5f, "typed", new Vector(1, 1, 1)), received!.Value);
    }

    [Test]
    public static void DelegatesAreRejectedAsPlainValues()
    {
        Action callback = () => { };
        Assert.Throws<ArgumentException>(() => Events.Call("nanossharp-test-delegate", callback));
    }

    [Test]
    public static void DisposedSubscriptionIsNotInvoked()
    {
        var calls = 0;
        var subscription = Events.Subscribe("nanossharp-test-dispose", () => calls++);
        Events.Call("nanossharp-test-dispose");
        subscription.Dispose();
        Events.Call("nanossharp-test-dispose");

        Assert.Equal(1, calls);
        Assert.True(!subscription.IsActive, "Subscription is still active");
    }

    private static object?[] Echo(params object?[] values)
    {
        object?[]? received = null;
        using var subscription = Events.Subscribe("nanossharp-test-echo", (object?[] arguments) => received = arguments);
        Events.Call("nanossharp-test-echo", values);
        return received ?? throw new AssertionException("Echo handler was not invoked");
    }
}
