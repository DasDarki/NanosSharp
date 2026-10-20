using NanosSharp.Installer.Cli;
using NanosSharp.Installer.Installation;
using NanosSharp.Updater;

namespace NanosSharp.Installer.Client;

internal static class ClientInstaller
{
    private const string ProxyLibrary = "version.dll";
    private const string ProxyName = "version";
    private const string NanosSharpDirectory = "NanosSharp";
    private const string LaunchOptions = "WINEDLLOVERRIDES=\"version=n,b\" %command%";
    private static readonly string[] OwnedDirectories = ["runtime", "dotnet"];

    public static async Task InstallAsync(Options options, ReleaseDownloader client, string temporaryDirectory)
    {
        var game = Locate(options.Path);
        Terminal.Step($"nanos world found in {game.BinariesDirectory}");
        EnsureGameClosed(game);

        var release = await client.GetAsync(options.Version);
        var nanosSharpDirectory = Path.Combine(game.BinariesDirectory, NanosSharpDirectory);
        var isCurrent = InstalledVersion.Read(nanosSharpDirectory) == release.Version;
        if (isCurrent && !options.Force)
        {
            Terminal.Success($"The NanosSharp client {release.Version} is already installed, use --force to reinstall it.");
            ConfigureProton(game);
            return;
        }

        Terminal.Step($"Downloading the NanosSharp client {release.Version}");
        var archive = await client.DownloadVerifiedAsync(release, Release.ClientAsset(release.Version), temporaryDirectory);
        RemoveOwnedFiles(game.BinariesDirectory);
        ArchiveExtractor.ExtractZip(archive, game.BinariesDirectory);
        Terminal.Success($"The NanosSharp client {release.Version} is installed.");
        ConfigureProton(game);
    }

    public static void Uninstall(Options options)
    {
        var game = Locate(options.Path);
        EnsureGameClosed(game);
        var library = Path.Combine(game.BinariesDirectory, ProxyLibrary);
        if (File.Exists(library))
        {
            File.Delete(library);
        }

        var nanosSharpDirectory = Path.Combine(game.BinariesDirectory, NanosSharpDirectory);
        if (Directory.Exists(nanosSharpDirectory))
        {
            Directory.Delete(nanosSharpDirectory, true);
        }

        if (game.UserRegistry is { } userRegistry && File.Exists(userRegistry))
        {
            WinePrefix.SetDllOverride(userRegistry, ProxyName, null);
        }

        Terminal.Success("The NanosSharp client was removed.");
    }

    private static GameInstallation Locate(string? path)
    {
        var game = path is null ? SteamLibrary.Find() : SteamLibrary.FromPath(path);
        while (game is null)
        {
            var answer = Terminal.Ask("nanos world was not found. Enter the folder of nanos world:")
                         ?? throw new InstallerException("nanos world was not found, pass its folder with --path.");
            game = SteamLibrary.FromPath(answer);
        }

        return game;
    }

    private static void EnsureGameClosed(GameInstallation game)
    {
        var isPrefixInUse = game.CompatDataDirectory is { } compatData && WinePrefix.IsInUse(compatData);
        if (GameProcess.IsRunning() || isPrefixInUse)
        {
            throw new InstallerException("nanos world is still running, close it and run the installer again.");
        }
    }

    private static void ConfigureProton(GameInstallation game)
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        if (game.UserRegistry is not { } userRegistry || !File.Exists(userRegistry))
        {
            Terminal.Warning("nanos world was not started with Proton yet, so its Wine prefix does not exist.");
            Terminal.Info("Start nanos world once and run the installer again, or add this to its launch options in Steam:");
            Terminal.Info(LaunchOptions);
            return;
        }

        WinePrefix.SetDllOverride(userRegistry, ProxyName, WinePrefix.NativeThenBuiltin);
        Terminal.Success("Proton loads NanosSharp now, no launch options are needed.");
    }

    private static void RemoveOwnedFiles(string binariesDirectory)
    {
        foreach (var directory in OwnedDirectories.Select(name => Path.Combine(binariesDirectory, NanosSharpDirectory, name)))
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
