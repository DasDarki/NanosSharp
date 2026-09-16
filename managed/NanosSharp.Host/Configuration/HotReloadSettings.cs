namespace NanosSharp.Host.Configuration;

internal sealed class HotReloadSettings
{
    public bool Enabled { get; set; } = true;

    public int DelayMilliseconds { get; set; } = 500;

    public bool VerifyUnload { get; set; } = true;
}
