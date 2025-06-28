using System.Collections.Concurrent;
using NanosSharp.API;
using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Events class in C#.
/// </summary>
public static class Events
{
    /// <summary>
    /// A dictionary mapping actions to lua c functions for event subscriptions.
    /// </summary>
    internal static readonly ConcurrentDictionary<Action<object[]>, ILuaVM.CFunction> Subscriptions = new();
    
    /// <summary>
    /// Calls an Event on Server which will be triggered in all Client's Packages of all Players.
    /// </summary>
    /// <param name="eventName">The Event Name to trigger the event.</param>
    /// <param name="args">Arguments to pass to the event.</param>
    public static void BroadcastRemote(string eventName, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.BroadcastRemote(ServerModule.VM, eventName, args);
    }
    
    /// <summary>
    /// Calls an Event on Server which will be triggered in all Client's Packages of all Players in that dimension.
    /// </summary>
    /// <param name="dimension">The Dimension to send this event.</param>
    /// <param name="eventName">The Event Name to trigger the event.</param>
    /// <param name="args">Arguments to pass to the event.</param>
    public static void BroadcastRemoteDimension(int dimension, string eventName, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.BroadcastRemoteDimension(ServerModule.VM, dimension, eventName, args);
    }
    
    /// <summary>
    /// Calls an Event which will be triggered in all Local Packages.
    /// </summary>
    /// <param name="eventName">The Event Name to trigger the event.</param>
    /// <param name="args">	Arguments to pass to the event.</param>
    public static void Call(string eventName, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.Call(ServerModule.VM, eventName, args);
    }
    
    /// <summary>
    /// Calls an Event if on Server which will be triggered in all Client's Packages of a specific Player.
    /// </summary>
    /// <param name="eventName">The Event Name to trigger the event.</param>
    /// <param name="player">The remote player to send this event.</param>
    /// <param name="args">Arguments to pass to the event.</param>
    public static void CallRemote(string eventName, Player player, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.CallRemote(ServerModule.VM, eventName, player, args);
    }
    
    /// <summary>
    /// Subscribes for an user-created event which will be triggered for only local called events.
    /// </summary>
    /// <param name="eventName">The Event Name to subscribe.</param>
    /// <param name="callback">The callback function to execute.</param>
    public static void Subscribe(string eventName, Action<object?[]> callback)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.Subscribe(ServerModule.VM, eventName, GetSubscription(callback));
    }
    
    /// <summary>
    /// Subscribes for an user-created event which will be triggered for only remote called events.
    /// </summary>
    /// <param name="eventName">The Event Name to subscribe.</param>
    /// <param name="callback">The callback function to execute.</param>
    public static void SubscribeRemote(string eventName, Action<object?[]> callback)
    {
        ServerModule.EnsureMainThread();
        
        BEvents.SubscribeRemote(ServerModule.VM, eventName, GetSubscription(callback));
    }
    
    /// <summary>
    /// Unsubscribes from all subscribed events in this Package with that event name, optionally passing the function to unsubscribe only that callback
    /// </summary>
    /// <param name="eventName">The Event Name to unsubscribe.</param>
    /// <param name="callback">The callback function to unsubscribe.</param>
    /// <exception cref="ArgumentException">The callback is not subscribed to the event.</exception>
    public static void Unsuscribe(string eventName, Action<object?[]>? callback = null)
    {
        ServerModule.EnsureMainThread();
        
        if (callback == null)
        {
            BEvents.Unsubscribe(ServerModule.VM, eventName);
            return;
        }
        
        var cfunc = RemoveSubscription(callback);
        if (cfunc != null)
        {
            BEvents.Unsubscribe(ServerModule.VM, eventName, cfunc);
        }
        else
        {
            throw new ArgumentException("The callback is not subscribed to the event.");
        }
    }
    
    /// <summary>
    /// Unsubscribes from all subscribed remote events in this Package with that event name, optionally passing the function to unsubscribe only that callback.
    /// </summary>
    /// <param name="eventName">The Event Name to unsubscribe.</param>
    /// <param name="callback">The callback function to unsubscribe.</param>
    /// <exception cref="ArgumentException">The callback is not subscribed to the event.</exception>
    public static void UnsubscribeRemote(string eventName, Action<object?[]>? callback = null)
    {
        ServerModule.EnsureMainThread();
        
        if (callback == null)
        {
            BEvents.UnsubscribeRemote(ServerModule.VM, eventName);
            return;
        }
        
        var cfunc = RemoveSubscription(callback);
        if (cfunc != null)
        {
            BEvents.UnsubscribeRemote(ServerModule.VM, eventName, cfunc);
        }
        else
        {
            throw new ArgumentException("The callback is not subscribed to the event.");
        }
    }
    
    private static ILuaVM.CFunction GetSubscription(Action<object?[]> callback)
    {
        return Subscriptions.GetOrAdd(callback, _ =>
        {
            return state =>
            {
                var args = new object?[state.GetTop()];
                for (var i = 1; i <= args.Length; i++)
                {
                    args[i - 1] = state.ToObject(i);
                }
                
                callback(args);
                return 0;
            };
        });
    }
    
    private static ILuaVM.CFunction? RemoveSubscription(Action<object?[]> callback)
    {
        Subscriptions.TryRemove(callback, out var cfunc);
        return cfunc;
    }
}