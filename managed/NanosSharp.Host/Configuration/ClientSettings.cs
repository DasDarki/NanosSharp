namespace NanosSharp.Host.Configuration;

internal sealed class ClientSettings
{
    public bool Required { get; set; }

    public int HandshakeTimeoutSeconds { get; set; } = 30;

    public string KickMessage { get; set; } = "This server requires NanosSharp {version}. Download it at https://nanossharp.dasdarki.de";
}
