using NanosSharp.API;

namespace NanosSharp.Server;

/// <summary>
/// The server module loads the managed C# packages for the server side and provides the server side API.
/// </summary>
internal sealed class ServerModule : IModule
{
    /// <summary>
    /// The current runtime configuration for the server module.
    /// </summary>
    internal static RuntimeConfig Config { get; private set; } = null!;
    
    /// <summary>
    /// The logger for the server module.
    /// </summary>
    internal static Logger Logger { get; private set; } = null!;

    /// <summary>
    /// The current Lua virtual machine bound to the server module.
    /// </summary>
    internal static ILuaVM VM { get; private set; } = null!;
    
    /// <summary>
    /// The main thread ID of the server module.
    /// </summary>
    internal static int MainThreadID { get; private set; }
    
    /// <summary>
    /// Checks if the current thread is the main thread.
    /// </summary>
    internal static bool IsMainThread => Environment.CurrentManagedThreadId == MainThreadID;
    
    /// <summary>
    /// A list of all actions which are enqueued to run on the main thread.
    /// </summary>
    private static readonly List<Action> EnqueuedActions = [];
    
    /// <summary>
    /// Gets called by NanosSharp runtime when the module is loaded.
    /// </summary>
    public void Initialize(ILuaVM vm)
    {
        VM = vm;
        MainThreadID = Environment.CurrentManagedThreadId;
        
        Config = RuntimeConfig.Load();
        Logger = new Logger("NanosSharp", Config.Debug);
        
        RegisterOnTick();
    }

    /// <summary>
    /// Registers the <see cref="OnTick"/> function to the server tick event.
    /// </summary>
    private void RegisterOnTick()
    {
        VM.PushGlobalTable();
        VM.GetField(-1, "Server");
        VM.GetField(-1, "Subscribe");
        VM.PushString("Tick");
        VM.PushManagedFunction(OnTick);
        VM.MCall(2, 0);
        VM.ClearStack();
    }

    /// <summary>
    /// Gets called every tick of the server. It used to run all enqueued actions on the main thread.
    /// </summary>
    private int OnTick(ILuaVM vm)
    {
        lock (EnqueuedActions)
        {
            foreach (var action in EnqueuedActions)
            {
                action();
            }
            
            EnqueuedActions.Clear();
        }
        
        return 0;
    }
    
    /// <summary>
    /// Checks if the current thread is the main thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the current thread is not the main thread.</exception>
    internal static void EnsureMainThread()
    {
        if (Environment.CurrentManagedThreadId != MainThreadID)
        {
            throw new InvalidOperationException("This operation must be performed on the main thread.");
        }
    }
    
    /// <summary>
    /// Enqueues an action to run on the main thread.
    /// </summary>
    internal static void RunOnMainThread(Action action)
    {
        lock (EnqueuedActions)
        {
            EnqueuedActions.Add(action);
        }
    }
}