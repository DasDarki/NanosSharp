using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EntityTests
{
    private const string CubeMesh = "nanos-world::SM_Cube";

    [Test]
    public static void SpawnedPropIsTracked()
    {
        var prop = new Prop(new Vector(0, 0, 100), Rotator.Zero, CubeMesh);
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
        var prop = new Prop(Vector.Zero, Rotator.Zero, CubeMesh);
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
        var prop = new Prop(Vector.Zero, Rotator.Zero, CubeMesh);
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
        var prop = new Prop(Vector.Zero, Rotator.Zero, CubeMesh);
        try
        {
            var resolved = ScriptApi.Invoke<Entity>("Server", "GetEntityByID", prop.ID);
            Assert.True(ReferenceEquals(prop, resolved), "GetEntityByID returned a different wrapper");
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void SubclassIsBoundOnSpawn()
    {
        var prop = new MarkerProp(Vector.Zero);
        try
        {
            Assert.True(prop.IsValid, "Subclass instance is not bound to the spawned entity");
            var resolved = ScriptApi.Invoke<Entity>("Server", "GetEntityByID", prop.ID);
            Assert.True(ReferenceEquals(prop, resolved), "The registry does not return the subclass instance");
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void GeneratedPropertiesDelegateToMethods()
    {
        var prop = new Prop(Vector.Zero, Rotator.Zero, CubeMesh, gravityEnabled: false);
        try
        {
            prop.Scale = new Vector(2, 2, 2);
            Assert.Equal(new Vector(2, 2, 2), prop.GetScale());
            Assert.Equal(CubeMesh, prop.Mesh);
            Assert.True(!prop.IsGravityEnabled(), "Optional constructor argument was not applied");
            Assert.True(!prop.GravityEnabled, "Boolean property does not delegate to IsGravityEnabled");
        }
        finally
        {
            prop.Destroy();
        }
    }

    [Test]
    public static void InvalidArgumentsThrowScriptException()
    {
        var prop = new Prop(Vector.Zero, Rotator.Zero, CubeMesh);
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

public sealed class MarkerProp(Vector location) : Prop(location, Rotator.Zero, "nanos-world::SM_Cube");
