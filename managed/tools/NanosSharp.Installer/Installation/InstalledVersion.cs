namespace NanosSharp.Installer.Installation;

internal static class InstalledVersion
{
    public const string FileName = "VERSION";

    public static string? Read(string nanosSharpDirectory)
    {
        var path = Path.Combine(nanosSharpDirectory, FileName);
        return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
    }
}
