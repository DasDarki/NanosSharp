using NanosSharp.Packages;
using NanosSharp.Threading;

namespace NanosSharp.Eventing;

/// <summary>Registers objects whose <see cref="ServerEventAttribute"/> methods handle typed events.</summary>
public static class EventBus
{
    private static readonly EventDispatcher GlobalDispatcher = new();

    private static EventDispatcher CurrentDispatcher => PackageContext.Current?.Events ?? GlobalDispatcher;

    /// <summary>Registers every event handler method of the listener and returns their count.</summary>
    public static int Register(object listener)
    {
        ArgumentNullException.ThrowIfNull(listener);
        MainThread.Ensure();
        return CurrentDispatcher.Register(listener);
    }

    /// <summary>Removes every event handler of the listener.</summary>
    public static void Unregister(object listener)
    {
        ArgumentNullException.ThrowIfNull(listener);
        MainThread.Ensure();
        CurrentDispatcher.Unregister(listener);
    }

}
