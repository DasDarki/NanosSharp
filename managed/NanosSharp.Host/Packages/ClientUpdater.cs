#if CLIENT
using NanosSharp.Logging;
using NanosSharp.Updater;

namespace NanosSharp.Host.Packages;

internal static class ClientUpdater
{
    private const string StagingDirectoryName = "update";
    private const string StagedDirectoryName = "staged";
    private const string MarkerFileName = "pending";
    private const string VersionFileName = "VERSION";
    private const string OptOutEnvironmentVariable = "NANOSSHARP_NO_UPDATE";

    public static void Start(string runtimeDirectory)
    {
        if (!ReleaseSignature.IsConfigured || Environment.GetEnvironmentVariable(OptOutEnvironmentVariable) is "1" or "true")
        {
            return;
        }

        var nanosSharpDirectory = Path.GetFullPath(Path.Combine(runtimeDirectory, ".."));
        _ = Task.Run(() => CheckAsync(nanosSharpDirectory));
    }

    private static async Task CheckAsync(string nanosSharpDirectory)
    {
        try
        {
            var installed = InstalledVersion(nanosSharpDirectory);
            if (installed is null)
            {
                return;
            }

            using var downloader = new ReleaseDownloader();
            var release = await downloader.GetAsync(null);
            if (!ReleaseVersion.IsNewer(release.Version, installed))
            {
                return;
            }

            await Stage(downloader, release, nanosSharpDirectory);
        }
        catch (Exception exception)
        {
            Log.Debug($"The client update check failed: {exception.Message}");
        }
    }

    private static async Task Stage(ReleaseDownloader downloader, Release release, string nanosSharpDirectory)
    {
        var staging = Path.Combine(nanosSharpDirectory, StagingDirectoryName);
        ResetDirectory(staging);
        var download = Path.Combine(staging, "download");
        Directory.CreateDirectory(download);
        var archive = await downloader.DownloadVerifiedAsync(release, Release.ClientAsset(release.Version), download);
        ArchiveExtractor.ExtractZip(archive, Path.Combine(staging, StagedDirectoryName));
        File.WriteAllText(Path.Combine(staging, MarkerFileName), release.Version);
        Log.Info($"NanosSharp client {release.Version} is ready, it is applied the next time the game starts.");
    }

    private static string? InstalledVersion(string nanosSharpDirectory)
    {
        var path = Path.Combine(nanosSharpDirectory, VersionFileName);
        return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
    }

    private static void ResetDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);
    }
}
#endif
