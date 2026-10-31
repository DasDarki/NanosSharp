#if !CLIENT
using System.Security.Cryptography;
using NanosSharp.Core;
using NanosSharp.Entities;
using NanosSharp.Logging;
using NanosSharp.Scripting;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Serves sealed client assemblies to connecting players over the NanosSharp event
/// channel, so clients run package C# without carrying a compiler. A client asks for
/// a package by name, sends the hash it already has and an ephemeral public key; the
/// server wraps its per-server key to that public key and replies that there is none,
/// that the cached copy is current, or with the sealed assembly split into base64
/// chunks. The assembly is encrypted at rest and in transit, and no secret is shipped.
/// </summary>
internal static class ClientAssemblyServer
{
    private static ScriptSubscription? _subscription;

    private delegate void RequestHandler(Player player, string packageName, string haveHash, string clientPublicKey);

    public static void Start() =>
        _subscription = Events.SubscribeRemote(Handshake.ClientAssemblyRequestEvent, (RequestHandler)OnRequest);

    public static void Stop()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private static void OnRequest(Player player, string packageName, string haveHash, string clientPublicKey)
    {
        Log.Debug($"[client-asm] request from {player.Name} for {packageName} (have: {(haveHash.Length == 0 ? "none" : haveHash)})");
        var assembly = ClientAssemblyBuilder.Get(packageName);
        if (assembly is null)
        {
            Send(player, packageName, Handshake.ClientAssemblyNone, string.Empty, 0, 0, string.Empty, string.Empty, string.Empty);
            return;
        }

        if (!TryWrapKey(clientPublicKey, out var serverPublicKey, out var wrappedKey))
        {
            Log.Warning($"[client-asm] rejecting the request of {player.Name} for {packageName}, its public key is invalid.");
            Send(player, packageName, Handshake.ClientAssemblyNone, string.Empty, 0, 0, string.Empty, string.Empty, string.Empty);
            return;
        }

        if (assembly.Hash == haveHash)
        {
            Send(player, packageName, Handshake.ClientAssemblyCached, assembly.Hash, 0, 0, serverPublicKey, wrappedKey, string.Empty);
            return;
        }

        var encoded = Convert.ToBase64String(assembly.Sealed);
        var total = (encoded.Length + Handshake.ClientAssemblyChunkSize - 1) / Handshake.ClientAssemblyChunkSize;
        for (var index = 0; index < total; index++)
        {
            var start = index * Handshake.ClientAssemblyChunkSize;
            var length = Math.Min(Handshake.ClientAssemblyChunkSize, encoded.Length - start);
            Send(player, packageName, Handshake.ClientAssemblyChunk, assembly.Hash, total, index, serverPublicKey, wrappedKey,
                encoded.Substring(start, length));
        }
    }

    private static bool TryWrapKey(string clientPublicKey, out string serverPublicKey, out string wrappedKey)
    {
        serverPublicKey = string.Empty;
        wrappedKey = string.Empty;
        if (clientPublicKey.Length == 0)
        {
            return false;
        }

        try
        {
            var recipient = Convert.FromBase64String(clientPublicKey);
            var (publicKey, wrapped) = ClientAssemblyCrypto.WrapKey(ClientAssemblyBuilder.Key, recipient);
            serverPublicKey = Convert.ToBase64String(publicKey);
            wrappedKey = Convert.ToBase64String(wrapped);
            return true;
        }
        catch (Exception exception) when (exception is FormatException or CryptographicException)
        {
            return false;
        }
    }

    private static void Send(Player player, string packageName, int status, string hash, int total, int index,
        string serverPublicKey, string wrappedKey, string data) =>
        Events.CallRemote(Handshake.ClientAssemblyEvent, player, packageName,
            $"{status}|{hash}|{total}|{index}|{serverPublicKey}|{wrappedKey}", data);
}
#endif
