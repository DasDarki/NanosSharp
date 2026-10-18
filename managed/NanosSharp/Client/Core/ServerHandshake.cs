using NanosSharp.Interop.Reflection;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

internal static class ServerHandshake
{
    private static readonly StaticMethodSlot CallRemoteSlot = new("Events", "CallRemote");
    private static ScriptSubscription? _request;

    public static void Start()
    {
        ScriptApi.Invoke("Client", "SetValue", Handshake.ClientValueKey, Handshake.RuntimeVersion);
        _request = Events.SubscribeRemote(Handshake.RequestEventName, (Action)Reply);
    }

    public static void Stop()
    {
        _request?.Dispose();
        _request = null;
    }

    private static void Reply() =>
        ScriptApi.InvokeVariadic(CallRemoteSlot, [Handshake.EventName, null], [Handshake.ProtocolVersion, Handshake.RuntimeVersion]);
}
