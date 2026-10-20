using NanosSharp.Installer.Cli;
using NanosSharp.Installer.Client;
using NanosSharp.Updater;
using NanosSharp.Installer.Server;

namespace NanosSharp.Installer;

internal static class InstallerApplication
{
    private const string Usage = """
        Installs, updates and removes NanosSharp.

        Usage:
          nanossharp-install client [options]      the client for the game
          nanossharp-install server [options]      NanosSharp for a nanos world server (Linux)

        Options:
          --path <directory>     the game or server directory, found automatically when omitted
          --version <version>    install a specific version instead of the latest one
          --from <directory>     install from downloaded release files instead of GitHub
          --uninstall            remove NanosSharp
          --update               update to the latest version if it is newer
          --force                reinstall even when the version is already installed
          -y, --yes              answer every question with its default
          -h, --help             show this help
        """;

    public static async Task<int> RunAsync(string[] arguments)
    {
        var exitCode = await RunInternalAsync(arguments);
        if (arguments.Length == 0 && OperatingSystem.IsWindows() && !Console.IsInputRedirected)
        {
            Console.WriteLine();
            Console.Write("Press Enter to close.");
            Console.ReadLine();
        }

        return exitCode;
    }

    private static async Task<int> RunInternalAsync(string[] arguments)
    {
        switch (arguments)
        {
            case ["keygen"]:
                return KeyCommands.Keygen();
            case ["sign", var manifest, var key]:
                return KeyCommands.Sign(manifest, key);
        }

        var temporaryDirectory = Directory.CreateTempSubdirectory("nanossharp-install-").FullName;
        try
        {
            var options = OptionsParser.Parse(arguments);
            if (options.ShowHelp)
            {
                Console.WriteLine(Usage);
                return 0;
            }

            Terminal.AssumeYes = options.AssumeYes;
            Terminal.Quiet = options.Quiet;
            var target = options.Target ?? DefaultTarget() ?? throw new InstallerException("Choose client or server, see --help.");
            await RunAsync(target, options, temporaryDirectory);
            return 0;
        }
        catch (Exception exception) when (exception is InstallerException or UpdateException or ArgumentException or IOException or HttpRequestException or UnauthorizedAccessException)
        {
            Terminal.Error(exception.Message);
            return 1;
        }
        finally
        {
            Directory.Delete(temporaryDirectory, true);
        }
    }

    private static async Task RunAsync(InstallTarget target, Options options, string temporaryDirectory)
    {
        using var client = new ReleaseDownloader(options.Source, Terminal.ProgressReporter);
        switch (target, options.Uninstall)
        {
            case (InstallTarget.Client, true):
                ClientInstaller.Uninstall(options);
                break;
            case (InstallTarget.Client, false):
                await ClientInstaller.InstallAsync(options, client, temporaryDirectory);
                break;
            case (InstallTarget.Server, true):
                ServerInstaller.Uninstall(options);
                break;
            case (InstallTarget.Server, false):
                await ServerInstaller.InstallAsync(options, client, temporaryDirectory);
                break;
        }
    }

    private static InstallTarget? DefaultTarget() => OperatingSystem.IsWindows() ? InstallTarget.Client : Terminal.Choose();
}
