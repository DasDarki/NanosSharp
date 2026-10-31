#if CLIENT
using NanosSharp.Core;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Logging;
using NanosSharp.Scripting;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Requests a package's precompiled client assembly from the server over the event
/// channel, reassembles the chunks, caches the assembly by its hash and hands it to
/// the sandbox. The client needs no compiler; a cached assembly is reused when the
/// server reports it is still current.
/// </summary>
internal static class ClientAssemblyTransfer
{
    private const string AssemblySuffix = ".Scripts";
    private const string CacheDirectoryName = "client-assemblies";

    private static string _cacheDirectory = string.Empty;
    private static Action<string, PackageBuild>? _stage;
    private static ScriptSubscription? _subscription;
    private static readonly Dictionary<string, Pending> Pendings = new(StringComparer.Ordinal);

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
    }

    public static void Request(string packageName) => Request(packageName, ReadCachedHash(packageName));

    private static void Request(string packageName, string haveHash) =>
        Events.CallRemote(Handshake.ClientAssemblyRequestEvent, packageName, haveHash);

    private static void OnResponse(string packageName, string header, string data)
    {
        var parts = header.Split('|');
        if (parts.Length != 4 || !int.TryParse(parts[0], out var status))
        {
            return;
        }

        var hash = parts[1];
        switch (status)
        {
            case Handshake.ClientAssemblyNone:
                Pendings.Remove(packageName);
                return;
            case Handshake.ClientAssemblyCached:
                StageCached(packageName, hash);
                return;
            case Handshake.ClientAssemblyChunk:
                if (int.TryParse(parts[2], out var total) && int.TryParse(parts[3], out var index))
                {
                    Accumulate(packageName, hash, total, index, data);
                }

                return;
        }
    }

    private static void Accumulate(string packageName, string hash, int total, int index, string data)
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
            var image = Convert.FromBase64String(string.Concat(pending.Chunks));
            WriteCache(packageName, hash, image);
            Stage(packageName, image);
        }
        catch (FormatException exception)
        {
            Log.Error($"The client assembly of {packageName} could not be decoded: {exception.Message}");
        }
    }

    private static void StageCached(string packageName, string hash)
    {
        var path = CachePath(packageName, hash);
        if (File.Exists(path))
        {
            Stage(packageName, File.ReadAllBytes(path));
            return;
        }

        Request(packageName, string.Empty);
    }

    private static void Stage(string packageName, byte[] image)
    {
        var name = packageName + AssemblySuffix;
        var build = new PackageBuild(
            new Dictionary<string, AssemblyImage>(StringComparer.Ordinal) { [name] = new AssemblyImage(name, image, null) },
            new Dictionary<string, string>(StringComparer.Ordinal), []);
        _stage?.Invoke(packageName, build);
    }

    private static string CachePath(string packageName, string hash) =>
        Path.Combine(_cacheDirectory, packageName, hash + ".dll");

    private static string ReadCachedHash(string packageName)
    {
        var directory = Path.Combine(_cacheDirectory, packageName);
        if (!Directory.Exists(directory))
        {
            return string.Empty;
        }

        var cached = Directory.EnumerateFiles(directory, "*.dll").FirstOrDefault();
        return cached is null ? string.Empty : Path.GetFileNameWithoutExtension(cached);
    }

    private static void WriteCache(string packageName, string hash, byte[] image)
    {
        var directory = Path.Combine(_cacheDirectory, packageName);
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);
        File.WriteAllBytes(CachePath(packageName, hash), image);
    }
}
#endif
