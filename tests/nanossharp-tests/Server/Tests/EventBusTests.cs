using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class EventBusTests
{
    private static readonly TimeSpan SpawnProtection = TimeSpan.FromMilliseconds(600);

    [Test]
    public static async Task AttributedHandlersRunByPriority()
    {
        var listener = new DamageListener();
        Assert.Equal(2, EventBus.Register(listener), "Registered handler count");
        var character = new Character(Vector.Zero, Rotator.Zero, "nanos-world::SK_Mannequin");
        try
        {
            await Task.Delay(SpawnProtection);
            character.ApplyDamage(5);

            Assert.Equal("early:5,late:5", string.Join(",", listener.Calls), "Handler order");
            Assert.True(ReferenceEquals(character, listener.LastCharacter), "Typed event property");
        }
        finally
        {
            EventBus.Unregister(listener);
            character.Destroy();
        }
    }

    [Test]
    public static async Task UnregisteredListenerIsNotCalled()
    {
        var listener = new DamageListener();
        EventBus.Register(listener);
        EventBus.Unregister(listener);
        var character = new Character(Vector.Zero, Rotator.Zero, "nanos-world::SK_Mannequin");
        try
        {
            await Task.Delay(SpawnProtection);
            character.ApplyDamage(5);
            Assert.Equal(0, listener.Calls.Count, "Calls after unregister");
        }
        finally
        {
            character.Destroy();
        }
    }

    [Test]
    public static async Task StaticEventClassesReceiveArguments()
    {
        var listener = new TickListener();
        EventBus.Register(listener);
        try
        {
            await Task.Delay(200);
            Assert.True(listener.Ticks > 0, "Tick handler was not called");
            Assert.True(listener.LastDelta > 0, "Delta time was not converted");
        }
        finally
        {
            EventBus.Unregister(listener);
        }
    }

    private sealed class DamageListener
    {
        public List<string> Calls { get; } = [];

        public Character? LastCharacter { get; private set; }

        [ServerEvent(Priority = 10)]
        private void Late(CharacterTakeDamageEvent damageEvent) => Calls.Add($"late:{damageEvent.Damage}");

        [ServerEvent(Priority = -10)]
        private void Early(CharacterTakeDamageEvent damageEvent)
        {
            LastCharacter = damageEvent.Character;
            Calls.Add($"early:{damageEvent.Damage}");
        }
    }

    private sealed class TickListener
    {
        public int Ticks { get; private set; }

        public float LastDelta { get; private set; }

        [ServerEvent]
        private void OnTick(ServerTickEvent tickEvent)
        {
            Ticks++;
            LastDelta = tickEvent.DeltaTime;
        }
    }
}
