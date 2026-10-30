using NanosSharp.Installer.Cli;
using NanosSharp.Installer.Installation;
using NanosSharp.Updater;

namespace NanosSharp.Installer.Server;

internal static class ServerInstaller
{
    private const string ServerScript = "NanosWorldServer.sh";
    private const string ServerExecutable = "NanosWorldServer.exe";
    private const string Launcher = "NanosSharp.sh";
    private const string ProxyLibrary = "version.dll";
    private const string NanosSharpDirectory = "NanosSharp";
    private const string InstallerName = "nanossharp-install";
    private static readonly string[] OwnedEntries = ["runtime", "LICENSE", InstallerName, InstalledVersion.FileName];
    private static readonly string[] WindowsStaleDirectories = ["runtime", "dotnet"];
    private static readonly string[] WindowsOwnedEntries = ["runtime", "dotnet", "LICENSE", InstalledVersion.FileName];

    private static string ServerMarker => OperatingSystem.IsWindows() ? ServerExecutable : ServerScript;

    public static async Task InstallAsync(Options options, ReleaseDownloader client, string temporaryDirectory)
    {
        if (OperatingSystem.IsWindows())
        {
            await InstallWindowsAsync(options, client, temporaryDirectory);
            return;
        }

        EnsureLinux();
        var serverDirectory = LocateServer(options.Path);
        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        var installed = InstalledVersion.Read(nanosSharpDirectory);

        if (options.IfEnabled && !InstallConfig.AutoUpdateEnabled(serverDirectory))
        {
            return;
        }

        if (options.Update && installed is null)
        {
            throw new InstallerException($"NanosSharp is not installed in {serverDirectory}, install it before updating.");
        }

        Terminal.Step($"Server found in {serverDirectory}");
        var release = await client.GetAsync(options.Version);
        if (installed == release.Version && !options.Force)
        {
            Terminal.Success($"NanosSharp {release.Version} is already installed, use --force to reinstall it.");
            return;
        }

        if (options.Update && installed is not null && !ReleaseVersion.IsNewer(release.Version, installed))
        {
            Terminal.Success($"NanosSharp {installed} is up to date.");
            return;
        }

        await EnsureDotnetAsync(client, temporaryDirectory);

        Terminal.Step($"Downloading NanosSharp {release.Version}");
        var archive = await client.DownloadVerifiedAsync(release, Release.ServerAsset(release.Version), temporaryDirectory);
        ApplyWithBackup(serverDirectory, temporaryDirectory, () => ArchiveExtractor.ExtractTarGz(archive, serverDirectory));
        Terminal.Success($"NanosSharp {release.Version} is installed.");
        Terminal.Info($"Start the server with ./{Launcher} instead of ./{ServerScript}.");
    }

    private static async Task InstallWindowsAsync(Options options, ReleaseDownloader client, string temporaryDirectory)
    {
        var serverDirectory = LocateServer(options.Path);
        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        var installed = InstalledVersion.Read(nanosSharpDirectory);

        if (options.IfEnabled && !InstallConfig.AutoUpdateEnabled(serverDirectory))
        {
            return;
        }

        if (options.Update && installed is null)
        {
            throw new InstallerException($"NanosSharp is not installed in {serverDirectory}, install it before updating.");
        }

        Terminal.Step($"Server found in {serverDirectory}");
        var release = await client.GetAsync(options.Version);
        if (installed == release.Version && !options.Force)
        {
            Terminal.Success($"NanosSharp {release.Version} is already installed, use --force to reinstall it.");
            return;
        }

        if (options.Update && installed is not null && !ReleaseVersion.IsNewer(release.Version, installed))
        {
            Terminal.Success($"NanosSharp {installed} is up to date.");
            return;
        }

        Terminal.Step($"Downloading NanosSharp {release.Version}");
        var archive = await client.DownloadVerifiedAsync(release, Release.ServerWindowsAsset(release.Version), temporaryDirectory);
        RemoveStaleWindowsDirectories(nanosSharpDirectory);
        ArchiveExtractor.ExtractZip(archive, serverDirectory);
        Terminal.Success($"NanosSharp {release.Version} is installed.");
        Terminal.Info("Start NanosWorldServer.exe as usual, NanosSharp loads automatically.");
    }

    private static void RemoveStaleWindowsDirectories(string nanosSharpDirectory)
    {
        foreach (var directory in WindowsStaleDirectories.Select(name => Path.Combine(nanosSharpDirectory, name)))
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    private static void ApplyWithBackup(string serverDirectory, string temporaryDirectory, Action apply)
    {
        var backup = Path.Combine(temporaryDirectory, "backup");
        Directory.CreateDirectory(backup);
        MoveOwnedFiles(serverDirectory, backup);
        try
        {
            apply();
        }
        catch
        {
            MoveOwnedFiles(serverDirectory, null);
            MoveOwnedFiles(backup, serverDirectory);
            throw;
        }
    }

    private static void MoveOwnedFiles(string from, string? to)
    {
        foreach (var relative in OwnedPaths())
        {
            Move(Path.Combine(from, relative), to is null ? null : Path.Combine(to, relative));
        }
    }

    private static IEnumerable<string> OwnedPaths()
    {
        yield return Launcher;
        foreach (var entry in OwnedEntries)
        {
            yield return Path.Combine(NanosSharpDirectory, entry);
        }
    }

    private static void Move(string source, string? destination)
    {
        if (Directory.Exists(source))
        {
            if (destination is null)
            {
                Directory.Delete(source, true);
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            Directory.Move(source, destination);
        }
        else if (File.Exists(source))
        {
            if (destination is null)
            {
                File.Delete(source);
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Move(source, destination, true);
        }
    }

    public static void Uninstall(Options options)
    {
        if (OperatingSystem.IsWindows())
        {
            UninstallWindows(LocateServer(options.Path));
            return;
        }

        EnsureLinux();
        var serverDirectory = LocateServer(options.Path);
        RemoveOwnedFiles(serverDirectory);
        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        if (Directory.Exists(nanosSharpDirectory) && !Directory.EnumerateFileSystemEntries(nanosSharpDirectory).Any())
        {
            Directory.Delete(nanosSharpDirectory);
        }

        Terminal.Success($"NanosSharp was removed from {serverDirectory}.");
        Terminal.Info("NanosSharp.toml and the NuGet cache in NanosSharp/cache were kept.");
    }

    private static void UninstallWindows(string serverDirectory)
    {
        var proxy = Path.Combine(serverDirectory, ProxyLibrary);
        if (File.Exists(proxy))
        {
            File.Delete(proxy);
        }

        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        foreach (var entry in WindowsOwnedEntries.Select(name => Path.Combine(nanosSharpDirectory, name)))
        {
            if (Directory.Exists(entry))
            {
                Directory.Delete(entry, true);
            }
            else if (File.Exists(entry))
            {
                File.Delete(entry);
            }
        }

        if (Directory.Exists(nanosSharpDirectory) && !Directory.EnumerateFileSystemEntries(nanosSharpDirectory).Any())
        {
            Directory.Delete(nanosSharpDirectory);
        }

        Terminal.Success($"NanosSharp was removed from {serverDirectory}.");
        Terminal.Info("NanosSharp.toml and the NuGet cache in NanosSharp/cache were kept.");
    }

    private static async Task EnsureDotnetAsync(ReleaseDownloader client, string temporaryDirectory)
    {
        if (DotnetRuntime.FindRoot() is { } root)
        {
            Terminal.Step($".NET 10 runtime found in {root}");
            if (!DotnetRuntime.IsDiscoverable(root))
            {
                Terminal.Warning($"Set DOTNET_ROOT={root} before starting the server, NanosSharp does not find it otherwise.");
            }

            return;
        }

        Terminal.Warning("The .NET 10 runtime is not installed.");
        if (!Terminal.Confirm($"Install it for the current user into {DotnetRuntime.UserRoot}?", true))
        {
            throw new InstallerException("NanosSharp needs the .NET 10 runtime, install it and run the installer again.");
        }

        Terminal.Step("Installing the .NET 10 runtime");
        await DotnetRuntime.InstallAsync(client, temporaryDirectory);
        Terminal.Success($".NET 10 runtime installed into {DotnetRuntime.UserRoot}.");
    }

    private static string LocateServer(string? path)
    {
        var directory = Path.GetFullPath(path ?? Directory.GetCurrentDirectory());
        while (!File.Exists(Path.Combine(directory, ServerMarker)))
        {
            var answer = Terminal.Ask($"{ServerMarker} was not found in {directory}. Enter the server directory:")
                         ?? throw new InstallerException($"{ServerMarker} was not found in {directory}, pass the server directory with --path.");
            directory = Path.GetFullPath(answer);
        }

        return directory;
    }

    private static void RemoveOwnedFiles(string serverDirectory)
    {
        var launcher = Path.Combine(serverDirectory, Launcher);
        if (File.Exists(launcher))
        {
            File.Delete(launcher);
        }

        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        foreach (var entry in OwnedEntries.Select(entry => Path.Combine(nanosSharpDirectory, entry)))
        {
            if (Directory.Exists(entry))
            {
                Directory.Delete(entry, true);
            }
            else if (File.Exists(entry))
            {
                File.Delete(entry);
            }
        }
    }

    private static void EnsureLinux()
    {
        if (!OperatingSystem.IsLinux())
        {
            throw new InstallerException("NanosSharp servers run on Linux and Windows only.");
        }
    }
}
