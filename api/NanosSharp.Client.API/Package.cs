namespace NanosSharp.Client.API;

/// <summary>
/// The package is the base class for all package classes.
/// </summary>
public abstract class Package
{
    /// <summary>
    /// Used to interact with the native functions of the nanos world game.
    /// </summary>
    protected internal INatives Natives { get; internal set; } = null!;
    
    /// <summary>
    /// Gets called on package load.
    /// </summary>
    public abstract void OnStart();

    /// <summary>
    /// Prints a message to the nanos world game console.
    /// </summary>
    /// <param name="message">The message to print.</param>
    public void Print(string message)
    {
        Natives.Base_Print(message);
    }
}