using System.Diagnostics;
using NanosSharp.Installer.Cli;
using NanosSharp.Updater;

namespace NanosSharp.Installer.Server;

internal static class DotnetRuntime
{
    private const string RuntimeMajor = "10.";
    private const string Channel = "10.0";
    private const string InstallScriptUrl = "https://dot.net/v1/dotnet-install.sh";
    private const string InstallLocationFile = "/etc/dotnet/install_location";
    private static readonly string[] DefaultRoots = ["/usr/share/dotnet", "/usr/lib/dotnet", "/usr/lib64/dotnet"];

    public static string UserRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet");

    public static string? FindRoot() => CandidateRoots().FirstOrDefault(HasRuntime);

    public static bool IsDiscoverable(string root) =>
        DefaultRoots.Contains(root) || root == UserRoot || root == ReadInstallLocation() ||
        root == Environment.GetEnvironmentVariable("DOTNET_ROOT");

    public static async Task InstallAsync(ReleaseDownloader client, string temporaryDirectory)
    {
        var script = Path.Combine(temporaryDirectory, "dotnet-install.sh");
        await client.DownloadAsync(InstallScriptUrl, script);
        using var process = Process.Start(new ProcessStartInfo("bash")
        {
            ArgumentList = { script, "--runtime", "dotnet", "--channel", Channel, "--install-dir", UserRoot, "--no-path" },
        }) ?? throw new InstallerException("bash could not be started to install .NET.");
        await process.WaitForExitAsync();
        if (process.ExitCode != 0 || !HasRuntime(UserRoot))
        {
            throw new InstallerException($"The .NET {Channel} runtime could not be installed into {UserRoot}.");
        }
    }

    private static IEnumerable<string> CandidateRoots()
    {
        if (Environment.GetEnvironmentVariable("DOTNET_ROOT") is { Length: > 0 } environmentRoot)
        {
            yield return environmentRoot;
        }

        yield return UserRoot;
        if (ReadInstallLocation() is { } installLocation)
        {
            yield return installLocation;
        }

        foreach (var root in DefaultRoots)
        {
            yield return root;
        }
    }

    private static bool HasRuntime(string root)
    {
        var runtimes = Path.Combine(root, "shared", "Microsoft.NETCore.App");
        return Directory.Exists(runtimes) &&
               Directory.EnumerateDirectories(runtimes).Any(directory => Path.GetFileName(directory).StartsWith(RuntimeMajor, StringComparison.Ordinal));
    }

    private static string? ReadInstallLocation() =>
        File.Exists(InstallLocationFile) ? File.ReadAllText(InstallLocationFile).Trim() : null;
}
