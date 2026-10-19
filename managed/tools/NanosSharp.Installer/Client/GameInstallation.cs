namespace NanosSharp.Installer.Client;

internal sealed record GameInstallation(string BinariesDirectory, string? CompatDataDirectory)
{
    public string? UserRegistry => CompatDataDirectory is null ? null : Path.Combine(CompatDataDirectory, "pfx", "user.reg");
}
