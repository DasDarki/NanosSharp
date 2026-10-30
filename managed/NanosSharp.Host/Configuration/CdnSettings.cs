namespace NanosSharp.Host.Configuration;

internal sealed class CdnSettings
{
    public int ClientFileTransferKib { get; set; }

    public string ServerCdnUrl { get; set; } = string.Empty;
}
