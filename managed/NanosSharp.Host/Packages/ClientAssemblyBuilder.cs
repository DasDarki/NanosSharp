#if !CLIENT
using System.Security.Cryptography;
using System.Text;
using NanosSharp.Host.Compilation;
using NanosSharp.Host.Packages.Building;
using NanosSharp.Logging;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Compiles a package's client-side C# (<c>Client/</c> and <c>Shared/</c>) into an
/// assembly on the server, so the client can load it without carrying Roslyn. The
/// result is cached by a hash of the sources and later served to connecting clients
/// over the NanosSharp event channel.
/// </summary>
internal static class ClientAssemblyBuilder
{
    private const string ClientReferenceFileName = "NanosSharp.ClientReference.dll";
    private const string ClientDirectoryName = "Client";
    private const string SharedDirectoryName = "Shared";

    private static readonly Dictionary<string, ClientAssembly> Cache = new(StringComparer.Ordinal);
    private static string _referencePath = string.Empty;
    private static AssemblyImage? _clientApi;
    private static bool _clientApiLoaded;

    public static void Initialize(string runtimeDirectory) =>
        _referencePath = Path.Combine(runtimeDirectory, ClientReferenceFileName);

    /// <summary>The compiled client assembly for a package, or null when it has no client code.</summary>
    public static ClientAssembly? Get(string packageName) => Cache.GetValueOrDefault(packageName);

    public static void Remove(string packageName) => Cache.Remove(packageName);

    /// <summary>
    /// Compiles the client assembly of a package and caches it, reusing the cached build when
    /// the sources are unchanged. Returns null when the package has no client code.
    /// </summary>
    public static ClientAssembly? Build(string packageName, string packageDirectory, bool optimize)
    {
        var sources = CollectSources(packageDirectory);
        if (sources.Count == 0)
        {
            Cache.Remove(packageName);
            return null;
        }

        var hash = HashSources(sources);
        if (Cache.TryGetValue(packageName, out var cached) && cached.Hash == hash)
        {
            return cached;
        }

        var api = LoadClientApi();
        if (api is null)
        {
            Log.Warning($"Cannot precompile the client code of {packageName}, {ClientReferenceFileName} is missing from the runtime.");
            return null;
        }

        var result = ScriptCompiler.Compile($"{packageName}.Scripts", sources, [], optimize, "CLIENT", api);
        foreach (var diagnostic in result.Diagnostics)
        {
            Log.Warning(diagnostic);
        }

        if (!result.Succeeded)
        {
            throw new PackageBuildException($"Compilation of the client code of {packageName} failed.");
        }

        var assembly = new ClientAssembly(hash, result.Assembly!);
        Cache[packageName] = assembly;
        return assembly;
    }

    private static List<string> CollectSources(string packageDirectory)
    {
        var sources = new List<string>();
        foreach (var directory in new[] { ClientDirectoryName, SharedDirectoryName })
        {
            var path = Path.Combine(packageDirectory, directory);
            if (Directory.Exists(path))
            {
                sources.AddRange(Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories));
            }
        }

        sources.Sort(StringComparer.Ordinal);
        return sources;
    }

    private static string HashSources(IReadOnlyList<string> sources)
    {
        using var sha = SHA256.Create();
        var builder = new StringBuilder();
        foreach (var source in sources)
        {
            builder.Append(Path.GetFileName(source)).Append('\n').Append(File.ReadAllText(source)).Append('\n');
        }

        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static AssemblyImage? LoadClientApi()
    {
        if (_clientApiLoaded)
        {
            return _clientApi;
        }

        _clientApiLoaded = true;
        if (File.Exists(_referencePath))
        {
            _clientApi = new AssemblyImage("NanosSharp", File.ReadAllBytes(_referencePath), null);
        }

        return _clientApi;
    }
}

/// <summary>A compiled client assembly together with the hash of the sources it was built from.</summary>
internal sealed record ClientAssembly(string Hash, byte[] Image);
#endif
