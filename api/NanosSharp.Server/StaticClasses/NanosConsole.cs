using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The nanos console is the equivalent to the static Console class in C#.
/// </summary>
public static class NanosConsole
{
    /// <summary>
    /// The command handler delegate.
    /// </summary>
    public delegate void CommandHandler(string[] args);
    
    /// <summary>
    /// Logs a message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to pass to the message.</param>
    public static void Log(string message, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BConsole.Log(ServerModule.VM, message, args);
    }

    /// <summary>
    /// Logs a warning message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to pass to the message.</param>
    public static void Warn(string message, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BConsole.Warn(ServerModule.VM, message, args);
    }
    
    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">The arguments to pass to the message.</param>
    public static void Error(string message, params object[] args)
    {
        ServerModule.EnsureMainThread();
        
        BConsole.Error(ServerModule.VM, message, args);
    }

    /// <summary>
    /// Runs a command in the console.
    /// </summary>
    /// <param name="command">The command to run.</param>
    public static void RunCommand(string command)
    {
        ServerModule.EnsureMainThread();
        
        BConsole.RunCommand(ServerModule.VM, command);
    }

    /// <summary>
    /// Registers a command for the console.
    /// </summary>
    /// <param name="command">The name of the command.</param>
    /// <param name="handler">The handler for the command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="parameters">The parameter names of the command.</param>
    public static void RegisterCommand(string command, CommandHandler handler, string description = "",
        params string[] parameters)
    {
        ServerModule.EnsureMainThread();
        
        BConsole.RegisterCommand(ServerModule.VM, command, state =>
        {
            var args = new string[state.GetTop()];
            for (var i = 1; i <= args.Length; i++)
            {
                args[i - 1] = state.ToString(i);
            }
            
            handler(args);
            
            return 0;
        }, description, parameters);
    }
}