using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EntityFactoryTests
{
    private const string CubeMesh = "nanos-world::SM_Cube";

    [Test]
    public static void FactoryWrapsNativelySpawnedEntities()
    {
        EntityFactory.Register<TrackedProp>();
        try
        {
            var entity = ScriptApi.Spawn("Prop", Vector.Zero, Rotator.Zero, CubeMesh);
            Assert.True(entity is TrackedProp, $"Spawned wrapper is {entity.GetType().Name}");
            entity.Destroy();
        }
        finally
        {
            EntityFactory.Unregister<TrackedProp>();
        }
    }

    [Test]
    public static void RegistrationRebindsExistingEntities()
    {
        var original = ScriptApi.Spawn("Prop", Vector.Zero, Rotator.Zero, CubeMesh);
        var id = ((Prop)original).ID;
        try
        {
            EntityFactory.Register<TrackedProp>();
            var tracked = ScriptApi.Invoke<Entity>("Server", "GetEntityByID", id);
            Assert.True(tracked is TrackedProp, $"Existing wrapper was not rebound, got {tracked?.GetType().Name}");
            Assert.True(!original.IsValid, "Previous wrapper is still bound");

            EntityFactory.Unregister<TrackedProp>();
            var restored = ScriptApi.Invoke<Entity>("Server", "GetEntityByID", id);
            Assert.True(restored is Prop and not TrackedProp, $"Wrapper was not restored, got {restored?.GetType().Name}");
            Assert.True(!tracked!.IsValid, "Factory wrapper is still bound after unregister");
            restored!.Destroy();
        }
        finally
        {
            EntityFactory.Unregister<TrackedProp>();
        }
    }

    [Test]
    public static void AttributeFactoryIsRegisteredAutomatically()
    {
        var light = ScriptApi.Spawn("Light", Vector.Zero, Rotator.Zero);
        try
        {
            Assert.True(light is TestLight, $"Light wrapper is {light.GetType().Name}");
            Assert.Equal(0, ((TestLight)light).Toggles, "Custom state");
        }
        finally
        {
            light.Destroy();
        }
    }
}

public sealed class TrackedProp : Prop;

[EntityFactory]
public sealed class TestLight : Light
{
    public int Toggles { get; set; }
}
