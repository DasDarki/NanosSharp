using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EventTests
{
    private const string Mannequin = "nanos-world::SK_Mannequin";
    private static readonly TimeSpan SpawnProtection = TimeSpan.FromMilliseconds(600);

    [Test]
    public static async Task ClassEventReceivesTypedArguments()
    {
        var character = new Character(Vector.Zero, Rotator.Zero, Mannequin);
        await Task.Delay(SpawnProtection);
        Character? damaged = null;
        var damage = 0;
        Character.TakeDamageHandler handler = (self, amount, _, _, _, _, _) =>
        {
            damaged = self;
            damage = amount;
        };

        Character.OnTakeDamage += handler;
        try
        {
            character.ApplyDamage(10);
            Assert.True(ReferenceEquals(character, damaged), "Event self argument is not the damaged character");
            Assert.Equal(10, damage, "Damage argument");
        }
        finally
        {
            Character.OnTakeDamage -= handler;
            DestroyIfValid(character);
        }
    }

    [Test]
    public static void BroadcastRemoteSupportsBothReliabilities()
    {
        Events.BroadcastRemote("nanossharp-test-remote", "payload", 1);
        Events.BroadcastRemoteUnreliable("nanossharp-test-remote", "payload", 2);
        Events.BroadcastRemoteUnreliable("nanossharp-test-remote", ["payload", 3]);
    }

    [Test]
    public static void HandlerReturningFalseBlocks()
    {
        using var blocking = Events.Subscribe("nanossharp-test-cancel", () => false);
        Assert.Equal(false, ScriptApi.Invoke<bool>("Events", "Call", "nanossharp-test-cancel"), "Call result");
    }

    [Test]
    public static async Task InstanceSubscriptionOnlyTargetsEntity()
    {
        var first = new Character(Vector.Zero, Rotator.Zero, Mannequin);
        var second = new Character(new Vector(200, 0, 0), Rotator.Zero, Mannequin);
        await Task.Delay(SpawnProtection);
        var deaths = new List<Character>();
        using var subscription = first.SubscribeDeath((self, _, _, _, _, _, _) => deaths.Add(self));
        try
        {
            second.ApplyDamage(10000);
            first.ApplyDamage(10000);
            Assert.Equal(1, deaths.Count, "Death count");
            Assert.True(ReferenceEquals(first, deaths[0]), "Wrong character died");
        }
        finally
        {
            DestroyIfValid(first);
            DestroyIfValid(second);
        }
    }

    [Test]
    public static async Task TickEventFires()
    {
        var ticks = 0;
        Server.TickHandler handler = _ => ticks++;
        Server.OnTick += handler;
        try
        {
            await Task.Delay(200);
            Assert.True(ticks > 0, "Tick event did not fire");
        }
        finally
        {
            Server.OnTick -= handler;
        }
    }

    private static void DestroyIfValid(Entity entity)
    {
        if (entity.IsValid)
        {
            entity.Destroy();
        }
    }
}
