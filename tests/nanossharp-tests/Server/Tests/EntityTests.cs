using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EntityTests
{
    [Test]
    public static void SpawnedPropIsTracked()
    {
        var prop = Prop.Spawn(new Vector(0, 0, 100), Rotator.Zero);
        try
        {
            Assert.True(prop.IsValid, "Prop is not valid after spawn");
            Assert.Equal("Prop", prop.ClassName);
            Assert.Equal(new Vector(0, 0, 100), prop.Location);
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void PropertiesWriteThrough()
    {
        var prop = Prop.Spawn(Vector.Zero, Rotator.Zero);
        try
        {
            prop.Location = new Vector(10, 20, 30);
            prop.SetValue("nanossharp-test", 123);

            Assert.Equal(new Vector(10, 20, 30), prop.Location);
            Assert.Equal(123, prop.GetValue<int>("nanossharp-test"));
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void DestroyInvalidatesEntity()
    {
        var prop = Prop.Spawn(Vector.Zero, Rotator.Zero);
        var destroyed = false;
        prop.Destroyed += _ => destroyed = true;
        prop.Destroy();

        Assert.True(destroyed, "Destroyed event was not raised");
        Assert.True(!prop.IsValid, "Prop is still valid");
        Assert.Throws<ObjectDisposedException>(() => _ = prop.Location);
    }

    [Test]
    public static void EntityIsResolvedById()
    {
        var prop = Prop.Spawn(Vector.Zero, Rotator.Zero);
        try
        {
            var resolved = ScriptApi.Invoke<Entity>("Server", "GetEntityByID", prop.Id);
            Assert.True(ReferenceEquals(prop, resolved), "GetEntityByID returned a different wrapper");
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void InvalidArgumentsThrowScriptException()
    {
        var prop = Prop.Spawn(Vector.Zero, Rotator.Zero);
        try
        {
            Assert.Throws<ScriptException>(() => prop.Invoke("SetLocation", "not a vector"));
            Assert.Throws<MissingMethodException>(() => prop.Invoke("DoesNotExist"));
        }
        finally
        {
            prop.Destroy();
        }
    }
}
