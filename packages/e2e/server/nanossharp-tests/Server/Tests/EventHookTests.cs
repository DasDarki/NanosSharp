using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EventHookTests
{
    [Test]
    public static void OverriddenHooksReceiveEvents()
    {
        var prop = new HookedProp(Vector.Zero);
        try
        {
            prop.SetValue("nanossharp-hook", 42);
            prop.SetDimension(3);

            Assert.Equal("nanossharp-hook=42", prop.LastValueChange);
            Assert.Equal(3, prop.LastDimension);
        }
        finally
        {
            prop.Destroy();
        }

        Assert.True(prop.WasDestroyed, "The Destroy hook was not called");
    }

    [Test]
    public static void HooksWorkForFactoryCreatedEntities()
    {
        var mesh = Exports.Call<Entity>("LuaInteropSpawnStaticMesh");
        try
        {
            Assert.True(mesh is HookedStaticMesh, $"Expected a HookedStaticMesh, got {mesh?.GetType().Name ?? "null"}");
            var hooked = (HookedStaticMesh)mesh!;
            hooked.SetValue("nanossharp-factory-hook", "set");
            Assert.Equal(1, hooked.ValueChanges);
        }
        finally
        {
            mesh?.Invoke("Destroy");
        }
    }
}

public sealed class HookedProp(Vector location) : Prop(location, Rotator.Zero, "nanos-world::SM_Cube")
{
    public string? LastValueChange { get; private set; }

    public int LastDimension { get; private set; }

    public bool WasDestroyed { get; private set; }

    protected override void HandleValueChange(string? key, object? value) => LastValueChange = $"{key}={value}";

    protected override void HandleDimensionChange(int oldDimension, int newDimension) => LastDimension = newDimension;

    protected override void HandleDestroy() => WasDestroyed = true;
}

[EntityFactory]
public sealed class HookedStaticMesh : StaticMesh
{
    public int ValueChanges { get; private set; }

    protected override void HandleValueChange(string? key, object? value) => ValueChanges++;
}
