using System.Net.Http.Json;

namespace NanosSharp.Updater;

/// <summary>Reads NanosSharp releases from GitHub or a local directory and downloads their assets verified.</summary>
public sealed class ReleaseDownloader : IDisposable
{
    private const string Repository = "DasDarki/NanosSharp";
    private const int BufferSize = 81920;

    private readonly HttpClient _http = new();
    private readonly string? _source;
    private readonly IProgress<DownloadProgress>? _progress;

    public ReleaseDownloader(string? source = null, IProgress<DownloadProgress>? progress = null)
    {
        _source = source;
        _progress = progress;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("nanossharp-updater");
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
            throw new UpdateException(version is null
                ? $"The latest release could not be read from GitHub ({(int)response.StatusCode})."
                : $"The release {version} does not exist ({(int)response.StatusCode}).");
        }

        var release = await response.Content.ReadFromJsonAsync(ReleaseJsonContext.Default.GitHubRelease)
                      ?? throw new UpdateException("GitHub returned an empty release.");
        return new Release(release.TagName.TrimStart('v'),
            release.Assets.ToDictionary(asset => asset.Name, asset => asset.DownloadUrl, StringComparer.Ordinal));
    }

    public async Task<string> DownloadVerifiedAsync(Release release, string assetName, string directory)
    {
        if (!release.Assets.TryGetValue(assetName, out var url))
        {
            throw new UpdateException($"The release {release.Version} has no file {assetName}.");
        }

        var checksums = await ReadVerifiedChecksumsAsync(release, directory);
        var destination = Path.Combine(directory, assetName);
        await DownloadAsync(url, destination);
        if (checksums is not null && (!checksums.TryGetValue(assetName, out var expected) || !Checksums.Matches(destination, expected)))
        {
            throw new UpdateException($"The checksum of {assetName} does not match, the download is rejected.");
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
            _progress?.Report(new DownloadProgress(received, total, false));
        }

        _progress?.Report(new DownloadProgress(received, total, true));
    }

    public void Dispose() => _http.Dispose();

    private async Task<Dictionary<string, string>?> ReadVerifiedChecksumsAsync(Release release, string directory)
    {
        if (!release.Assets.TryGetValue(Release.ChecksumsAsset, out var checksumsUrl))
        {
            if (_source is null || ReleaseSignature.IsConfigured)
            {
                throw new UpdateException($"The release {release.Version} has no {Release.ChecksumsAsset}.");
            }

            return null;
        }

        var manifestPath = Path.Combine(directory, Release.ChecksumsAsset);
        await DownloadAsync(checksumsUrl, manifestPath);
        var manifest = await File.ReadAllBytesAsync(manifestPath);
        await VerifySignatureAsync(release, directory, manifest);
        return Checksums.Parse(System.Text.Encoding.UTF8.GetString(manifest));
    }

    private async Task VerifySignatureAsync(Release release, string directory, byte[] manifest)
    {
        if (!ReleaseSignature.IsConfigured)
        {
            return;
        }

        if (!release.Assets.TryGetValue(ReleaseSignature.SignatureAsset, out var signatureUrl))
        {
            throw new UpdateException($"The release {release.Version} is not signed, it is rejected.");
        }

        var signaturePath = Path.Combine(directory, ReleaseSignature.SignatureAsset);
        await DownloadAsync(signatureUrl, signaturePath);
        if (!ReleaseSignature.Verify(manifest, await File.ReadAllBytesAsync(signaturePath)))
        {
            throw new UpdateException($"The signature of release {release.Version} is invalid, it is rejected.");
        }
    }
}
