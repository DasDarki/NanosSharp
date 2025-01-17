namespace NanosSharp.Client.API;

/// <summary>
/// An interface for interacting with the native functions of the nanos world game.
/// </summary>
public interface INatives
{
    /// <summary>
    /// Prints a message to the nanos world game console.
    /// </summary>
    void Base_Print(string message);
}