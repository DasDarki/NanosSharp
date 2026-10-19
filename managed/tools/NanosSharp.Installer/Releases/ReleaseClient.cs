using System.Net.Http.Json;
using NanosSharp.Installer.Cli;

namespace NanosSharp.Installer.Releases;

internal sealed class ReleaseClient : IDisposable
{
    private const string Repository = "DasDarki/NanosSharp";
    private const int BufferSize = 81920;

    private readonly HttpClient _http = new();
    private readonly string? _source;

    public ReleaseClient(string? source)
    {
        _source = source;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("nanossharp-install");
    }

    public async Task<Release> GetAsync(string? version)
    {
        if (_source is not null)
        {
            return Release.FromDirectory(_source);
        }

        var url = version is null
            ? $"https://api.github.com/repos/{Repository}/releases/latest"
            : $"https://api.github.com/repos/{Repository}/releases/tags/v{version}";
        using var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new InstallerException(version is null
                ? $"The latest release could not be read from GitHub ({(int)response.StatusCode})."
                : $"The release {version} does not exist ({(int)response.StatusCode}).");
        }

        var release = await response.Content.ReadFromJsonAsync(ReleaseJsonContext.Default.GitHubRelease)
                      ?? throw new InstallerException("GitHub returned an empty release.");
        return new Release(release.TagName.TrimStart('v'),
            release.Assets.ToDictionary(asset => asset.Name, asset => asset.DownloadUrl, StringComparer.Ordinal));
    }

    public async Task<string> DownloadVerifiedAsync(Release release, string assetName, string directory)
    {
        if (!release.Assets.TryGetValue(assetName, out var url))
        {
            throw new InstallerException($"The release {release.Version} has no file {assetName}.");
        }

        var destination = Path.Combine(directory, assetName);
        if (!release.Assets.TryGetValue(Release.ChecksumsAsset, out var checksumsUrl))
        {
            if (_source is null)
            {
                throw new InstallerException($"The release {release.Version} has no {Release.ChecksumsAsset}.");
            }

            Terminal.Warning($"{_source} contains no {Release.ChecksumsAsset}, {assetName} is not verified.");
            await DownloadAsync(url, destination);
            return destination;
        }

        var checksums = Checksums.Parse(await ReadTextAsync(checksumsUrl));
        if (!checksums.TryGetValue(assetName, out var expected))
        {
            throw new InstallerException($"{Release.ChecksumsAsset} of the release {release.Version} does not list {assetName}.");
        }

        await DownloadAsync(url, destination);
        if (!Checksums.Matches(destination, expected))
        {
            throw new InstallerException($"The checksum of {assetName} does not match, the download is damaged.");
        }

        return destination;
    }

    public async Task DownloadAsync(string url, string destination)
    {
        if (File.Exists(url))
        {
            File.Copy(url, destination, true);
            return;
        }

        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength;
        await using var source = await response.Content.ReadAsStreamAsync();
        await using var target = File.Create(destination);
        var buffer = new byte[BufferSize];
        long received = 0;
        int read;
        while ((read = await source.ReadAsync(buffer)) > 0)
        {
            await target.WriteAsync(buffer.AsMemory(0, read));
            received += read;
            Terminal.Progress(received, total);
        }

        Terminal.EndProgress();
    }

    public void Dispose() => _http.Dispose();

    private async Task<string> ReadTextAsync(string location) =>
        File.Exists(location) ? await File.ReadAllTextAsync(location) : await _http.GetStringAsync(location);
}
