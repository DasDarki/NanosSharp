#if !CLIENT
using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Scripting;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Serves precompiled client assemblies to connecting players over the NanosSharp
/// event channel, so clients run package C# without carrying a compiler. A client
/// asks for a package by name and sends the hash it already has; the server replies
/// that there is none, that the cached copy is current, or with the assembly split
/// into base64 chunks.
/// </summary>
internal static class ClientAssemblyServer
{
    private static ScriptSubscription? _subscription;

    private delegate void RequestHandler(Player player, string packageName, string haveHash);

    public static void Start() =>
        _subscription = Events.SubscribeRemote(Handshake.ClientAssemblyRequestEvent, (RequestHandler)OnRequest);

    public static void Stop()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private static void OnRequest(Player player, string packageName, string haveHash)
    {
        var assembly = ClientAssemblyBuilder.Get(packageName);
        if (assembly is null)
        {
            Send(player, packageName, Handshake.ClientAssemblyNone, string.Empty, 0, 0, string.Empty);
            return;
        }

        if (assembly.Hash == haveHash)
        {
            Send(player, packageName, Handshake.ClientAssemblyCached, assembly.Hash, 0, 0, string.Empty);
            return;
        }

        var encoded = Convert.ToBase64String(assembly.Image);
        var total = (encoded.Length + Handshake.ClientAssemblyChunkSize - 1) / Handshake.ClientAssemblyChunkSize;
        for (var index = 0; index < total; index++)
        {
            var start = index * Handshake.ClientAssemblyChunkSize;
            var length = Math.Min(Handshake.ClientAssemblyChunkSize, encoded.Length - start);
            Send(player, packageName, Handshake.ClientAssemblyChunk, assembly.Hash, total, index, encoded.Substring(start, length));
        }
    }

    private static void Send(Player player, string packageName, int status, string hash, int total, int index, string data) =>
        Events.CallRemote(Handshake.ClientAssemblyEvent, player, packageName, $"{status}|{hash}|{total}|{index}", data);
}
#endif
