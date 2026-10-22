namespace OrbRush;

/// <summary>A single collectible orb: a glowing light paired with an invisible overlap trigger.</summary>
internal sealed class Orb
{
    /// <summary>The light that the player sees and runs towards.</summary>
    public Light Light { get; }

    /// <summary>The trigger that detects a character walking into the orb.</summary>
    public Trigger Trigger { get; }

    public Orb(Light light, Trigger trigger)
    {
        Light = light;
        Trigger = trigger;
    }

    /// <summary>Moves both the light and the trigger to a new location.</summary>
    public void MoveTo(Vector location)
    {
        Light.SetLocation(location);
        Trigger.SetLocation(location);
    }

    /// <summary>Destroys the underlying entities.</summary>
    public void Destroy()
    {
        if (Light.IsValid)
        {
            Light.Destroy();
        }

        if (Trigger.IsValid)
        {
            Trigger.Destroy();
        }
    }
}
