using NanosSharp.Client.API;

namespace NanosSharp.Client;

internal sealed class Natives : INatives
{
    public void Base_Print(string message)
    {
        BridgeServer.Instance.CallNanos("print", message);
    }
}