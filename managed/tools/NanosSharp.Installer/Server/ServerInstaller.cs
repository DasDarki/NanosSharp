using NanosSharp.Installer.Cli;
using NanosSharp.Installer.Installation;
using NanosSharp.Installer.Releases;

namespace NanosSharp.Installer.Server;

internal static class ServerInstaller
{
    private const string ServerScript = "NanosWorldServer.sh";
    private const string Launcher = "NanosSharp.sh";
    private const string NanosSharpDirectory = "NanosSharp";
    private static readonly string[] OwnedEntries = ["runtime", "LICENSE", InstalledVersion.FileName];

    public static async Task InstallAsync(Options options, ReleaseClient client, string temporaryDirectory)
    {
        EnsureLinux();
        var serverDirectory = LocateServer(options.Path);
        Terminal.Step($"Server found in {serverDirectory}");

        var release = await client.GetAsync(options.Version);
        var nanosSharpDirectory = Path.Combine(serverDirectory, NanosSharpDirectory);
        if (InstalledVersion.Read(nanosSharpDirectory) == release.Version && !options.Force)
        {
            Terminal.Success($"NanosSharp {release.Version} is already installed, use --force to reinstall it.");
            return;
        }

        await EnsureDotnetAsync(client, temporaryDirectory);

        Terminal.Step($"Downloading NanosSharp {release.Version}");
        var archive = await client.DownloadVerifiedAsync(release, Release.ServerAsset(release.Version), temporaryDirectory);
        RemoveOwnedFiles(serverDirectory);
        ArchiveExtractor.ExtractTarGz(archive, serverDirectory);
        Terminal.Success($"NanosSharp {release.Version} is installed.");
        Terminal.Info($"Start the server with ./{Launcher} instead of ./{ServerScript}.");
    }

    public static void Uninstall(Options options)
    {
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

    private static async Task EnsureDotnetAsync(ReleaseClient client, string temporaryDirectory)
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
        while (!File.Exists(Path.Combine(directory, ServerScript)))
        {
            var answer = Terminal.Ask($"{ServerScript} was not found in {directory}. Enter the server directory:")
                         ?? throw new InstallerException($"{ServerScript} was not found in {directory}, pass the server directory with --path.");
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
            throw new InstallerException("NanosSharp servers run on Linux only, Windows servers are not supported yet.");
        }
    }
}
