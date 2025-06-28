using NanosSharp.API;

namespace NanosSharp.Server;

public class Player(LuaRef handle) : NanosUnit(handle), IBaseEntity
{
    /// <summary>
    /// Calls an Event if on Server which will be triggered in all Client's Packages of this Player.
    /// </summary>
    /// <param name="eventName">The Event Name to trigger the event.</param>
    /// <param name="args">Arguments to pass to the event.</param>
    public void CallRemote(string eventName, params object[] args)
    {
        Events.CallRemote(eventName, this, args);
    }
}