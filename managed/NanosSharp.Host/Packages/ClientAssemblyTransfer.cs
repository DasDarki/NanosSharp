#if CLIENT
using System.Security.Cryptography;
using NanosSharp.Core;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Logging;
using NanosSharp.Scripting;
using NanosSharp.Threading;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Requests a package's sealed client assembly from the server over the event channel,
/// reassembles the chunks, unseals them with the per-server key delivered over an
/// ephemeral key exchange, caches the sealed bytes by their hash and hands the plaintext
/// to the sandbox. The client needs no compiler, the assembly is never stored or sent in
/// the clear, and a cached assembly is reused when the server reports it is still current.
/// </summary>
internal static class ClientAssemblyTransfer
{
    private const string AssemblySuffix = ".Scripts";
    private const string CacheDirectoryName = "client-assemblies";
    private const string CacheFileExtension = ".sealed";
    private const int MaxRequestAttempts = 8;
    private const int RequestRetryMilliseconds = 2000;

    private static string _cacheDirectory = string.Empty;
    private static Action<string, PackageBuild>? _stage;
    private static ScriptSubscription? _subscription;
    private static readonly Dictionary<string, Pending> Pendings = new(StringComparer.Ordinal);
    private static readonly HashSet<string> Awaiting = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, ECDiffieHellman> KeyPairs = new(StringComparer.Ordinal);

    private delegate void ResponseHandler(string packageName, string header, string data);

    private sealed class Pending
    {
        public required string Hash { get; init; }

        public required string?[] Chunks { get; init; }

        public int Received { get; set; }
    }

    public static void Initialize(string runtimeDirectory, Action<string, PackageBuild> stage)
    {
        _cacheDirectory = Path.Combine(runtimeDirectory, "cache", CacheDirectoryName);
        _stage = stage;
    }

    public static void Start() =>
        _subscription = Events.SubscribeRemote(Handshake.ClientAssemblyEvent, (ResponseHandler)OnResponse);

    public static void Stop()
    {
        _subscription?.Dispose();
        _subscription = null;
        Pendings.Clear();
        Awaiting.Clear();
        foreach (var keyPair in KeyPairs.Values)
        {
            keyPair.Dispose();
        }

        KeyPairs.Clear();
    }

    public static void Request(string packageName)
    {
        Discard(packageName);
        Awaiting.Add(packageName);
        KeyPairs[packageName] = ClientAssemblyCrypto.CreateEphemeral();
        SendRequest(packageName, 0, forceFull: false);
    }

    private static void SendRequest(string packageName, int attempt, bool forceFull)
    {
        if (!Awaiting.Contains(packageName) || !KeyPairs.TryGetValue(packageName, out var keyPair))
        {
            return;
        }

        var haveHash = forceFull ? string.Empty : ReadCachedHash(packageName);
        var publicKey = Convert.ToBase64String(ClientAssemblyCrypto.ExportPublicKey(keyPair));
        Log.Debug($"[client-asm] requesting {packageName} (attempt {attempt + 1}, cached: {(haveHash.Length == 0 ? "none" : haveHash)})");
        Events.CallRemote(Handshake.ClientAssemblyRequestEvent, packageName, haveHash, publicKey);
        if (attempt + 1 < MaxRequestAttempts)
        {
            ScriptTimer.SetTimeout(() => SendRequest(packageName, attempt + 1, forceFull), RequestRetryMilliseconds);
        }
    }

    private static void OnResponse(string packageName, string header, string data)
    {
        Awaiting.Remove(packageName);
        Log.Debug($"[client-asm] response for {packageName}: {header.Split('|')[0]} ({data.Length} chars)");
        var parts = header.Split('|');
        if (parts.Length != 6 || !int.TryParse(parts[0], out var status))
        {
            Discard(packageName);
            return;
        }

        var hash = parts[1];
        if (status == Handshake.ClientAssemblyNone)
        {
            Discard(packageName);
            return;
        }

        if (!TryUnwrapKey(packageName, parts[4], parts[5], out var key))
        {
            Log.Error($"The server key for the client assembly of {packageName} could not be unwrapped.");
            Discard(packageName);
            return;
        }

        switch (status)
        {
            case Handshake.ClientAssemblyCached:
                StageCached(packageName, hash, key);
                return;
            case Handshake.ClientAssemblyChunk:
                if (int.TryParse(parts[2], out var total) && int.TryParse(parts[3], out var index))
                {
                    Accumulate(packageName, hash, total, index, data, key);
                }

                return;
        }
    }

    private static bool TryUnwrapKey(string packageName, string serverPublicKey, string wrappedKey, out byte[] key)
    {
        key = [];
        if (!KeyPairs.TryGetValue(packageName, out var keyPair) || serverPublicKey.Length == 0 || wrappedKey.Length == 0)
        {
            return false;
        }

        try
        {
            key = ClientAssemblyCrypto.UnwrapKey(Convert.FromBase64String(wrappedKey), Convert.FromBase64String(serverPublicKey), keyPair);
            return true;
        }
        catch (Exception exception) when (exception is FormatException or CryptographicException)
        {
            return false;
        }
    }

    private static void Accumulate(string packageName, string hash, int total, int index, string data, byte[] key)
    {
        if (total <= 0 || index < 0 || index >= total)
        {
            return;
        }

        if (!Pendings.TryGetValue(packageName, out var pending) || pending.Hash != hash || pending.Chunks.Length != total)
        {
            pending = new Pending { Hash = hash, Chunks = new string?[total] };
            Pendings[packageName] = pending;
        }

        if (pending.Chunks[index] is not null)
        {
            return;
        }

        pending.Chunks[index] = data;
        pending.Received++;
        if (pending.Received < total)
        {
            return;
        }

        Pendings.Remove(packageName);
        try
        {
            var sealedImage = Convert.FromBase64String(string.Concat(pending.Chunks));
            var image = ClientAssemblyCrypto.Unseal(key, sealedImage);
            WriteCache(packageName, hash, sealedImage);
            Discard(packageName);
            Stage(packageName, image);
        }
        catch (Exception exception) when (exception is FormatException or CryptographicException)
        {
            Discard(packageName);
            Log.Error($"The client assembly of {packageName} could not be decoded: {exception.Message}");
        }
    }

    private static void StageCached(string packageName, string hash, byte[] key)
    {
        var path = CachePath(packageName, hash);
        if (File.Exists(path))
        {
            try
            {
                var image = ClientAssemblyCrypto.Unseal(key, File.ReadAllBytes(path));
                Discard(packageName);
                Stage(packageName, image);
                return;
            }
            catch (CryptographicException)
            {
                Log.Warning($"The cached client assembly of {packageName} could not be unsealed, requesting a fresh copy.");
            }
        }

        Awaiting.Add(packageName);
        SendRequest(packageName, 0, forceFull: true);
    }

    private static void Stage(string packageName, byte[] image)
    {
        var name = packageName + AssemblySuffix;
        var build = new PackageBuild(
            new Dictionary<string, AssemblyImage>(StringComparer.Ordinal) { [name] = new AssemblyImage(name, image, null) },
            new Dictionary<string, string>(StringComparer.Ordinal), []);
        _stage?.Invoke(packageName, build);
    }

    private static void Discard(string packageName)
    {
        Pendings.Remove(packageName);
        if (KeyPairs.Remove(packageName, out var keyPair))
        {
            keyPair.Dispose();
        }
    }

    private static string CachePath(string packageName, string hash) =>
        Path.Combine(_cacheDirectory, packageName, hash + CacheFileExtension);

    private static string ReadCachedHash(string packageName)
    {
        var directory = Path.Combine(_cacheDirectory, packageName);
        if (!Directory.Exists(directory))
        {
            return string.Empty;
        }

        var cached = Directory.EnumerateFiles(directory, "*" + CacheFileExtension).FirstOrDefault();
        return cached is null ? string.Empty : Path.GetFileNameWithoutExtension(cached);
    }

    private static void WriteCache(string packageName, string hash, byte[] sealedImage)
    {
        var directory = Path.Combine(_cacheDirectory, packageName);
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);
        File.WriteAllBytes(CachePath(packageName, hash), sealedImage);
    }
}
#endif
