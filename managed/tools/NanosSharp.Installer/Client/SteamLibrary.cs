using Microsoft.Win32;

namespace NanosSharp.Installer.Client;

internal static class SteamLibrary
{
    public const string AppId = "1841660";
    public const string GameExecutable = "NanosWorld-Win64-Shipping.exe";

    private const string SteamRegistryKey = @"HKEY_CURRENT_USER\Software\Valve\Steam";
    private static readonly string[] BinariesPath = ["NanosWorld", "Binaries", "Win64"];

    public static GameInstallation? Find()
    {
        foreach (var library in SteamRoots().SelectMany(Libraries).Distinct(StringComparer.Ordinal))
        {
            var steamApps = Path.Combine(library, "steamapps");
            var manifest = Path.Combine(steamApps, $"appmanifest_{AppId}.acf");
            if (!File.Exists(manifest))
            {
                continue;
            }

            var installDirectory = KeyValues.Parse(File.ReadAllText(manifest)).Child("AppState")?.Value("installdir");
            if (installDirectory is null)
            {
                continue;
            }

            var binaries = Path.Combine([steamApps, "common", installDirectory, .. BinariesPath]);
            if (File.Exists(Path.Combine(binaries, GameExecutable)))
            {
                return new GameInstallation(binaries, CompatData(steamApps));
            }
        }

        return null;
    }

    public static GameInstallation? FromPath(string path)
    {
        var full = Path.GetFullPath(path);
        foreach (var candidate in new[] { full, Path.Combine([full, .. BinariesPath]) })
        {
            if (File.Exists(Path.Combine(candidate, GameExecutable)))
            {
                var steamApps = Directory.GetParent(candidate)?.Parent?.Parent?.Parent?.Parent?.FullName;
                return new GameInstallation(candidate, steamApps is null ? null : CompatData(steamApps));
            }
        }

        return null;
    }

    private static string? CompatData(string steamApps) =>
        OperatingSystem.IsWindows() ? null : Path.Combine(steamApps, "compatdata", AppId);

    private static IEnumerable<string> SteamRoots()
    {
        if (OperatingSystem.IsWindows())
        {
            if (Registry.GetValue(SteamRegistryKey, "SteamPath", null) is string registryPath)
            {
                yield return Path.GetFullPath(registryPath);
            }

            yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
            yield break;
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] candidates =
        [
            Path.Combine(home, ".local", "share", "Steam"),
            Path.Combine(home, ".steam", "steam"),
            Path.Combine(home, ".steam", "root"),
            Path.Combine(home, ".var", "app", "com.valvesoftware.Steam", ".local", "share", "Steam"),
            Path.Combine(home, "snap", "steam", "common", ".local", "share", "Steam"),
        ];
        foreach (var candidate in candidates.Where(Directory.Exists))
        {
            yield return ResolveLinks(candidate);
        }
    }

    private static IEnumerable<string> Libraries(string steamRoot)
    {
        yield return steamRoot;
        var libraryFolders = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(libraryFolders))
        {
            yield break;
        }

        var folders = KeyValues.Parse(File.ReadAllText(libraryFolders)).Child("libraryfolders");
        foreach (var folder in folders?.Children.Values ?? [])
        {
            if (folder.Value("path") is { Length: > 0 } path && Directory.Exists(path))
            {
                yield return ResolveLinks(path);
            }
        }
    }

    private static string ResolveLinks(string path)
    {
        var info = new DirectoryInfo(path);
        return info.ResolveLinkTarget(true)?.FullName ?? info.FullName;
    }
}
