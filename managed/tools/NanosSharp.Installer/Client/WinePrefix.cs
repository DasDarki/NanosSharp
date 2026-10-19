namespace NanosSharp.Installer.Client;

internal static class WinePrefix
{
    public const string NativeThenBuiltin = "native,builtin";

    private const string OverridesSection = @"[Software\\Wine\\DllOverrides]";
    private const string BackupSuffix = ".nanossharp-backup";

    public static void SetDllOverride(string userRegistry, string library, string? mode)
    {
        var backup = userRegistry + BackupSuffix;
        if (!File.Exists(backup))
        {
            File.Copy(userRegistry, backup);
        }

        var lines = File.ReadAllLines(userRegistry).ToList();
        ApplyOverride(lines, library, mode);
        File.WriteAllText(userRegistry, string.Join('\n', lines) + '\n');
    }

    public static void ApplyOverride(List<string> lines, string library, string? mode)
    {
        var entry = $"\"{library}\"=";
        var header = lines.FindIndex(line => line.StartsWith(OverridesSection, StringComparison.Ordinal));
        if (header < 0)
        {
            if (mode is not null)
            {
                lines.AddRange(["", $"{OverridesSection} {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", $"{entry}\"{mode}\""]);
            }

            return;
        }

        var end = header + 1;
        while (end < lines.Count && lines[end].Length > 0 && !lines[end].StartsWith('['))
        {
            end++;
        }

        var existing = lines.FindIndex(header + 1, end - header - 1, line => line.StartsWith(entry, StringComparison.Ordinal));
        if (existing >= 0)
        {
            lines.RemoveAt(existing);
        }

        if (mode is null)
        {
            return;
        }

        var insert = header + 1;
        while (insert < lines.Count && lines[insert].StartsWith('#'))
        {
            insert++;
        }

        lines.Insert(insert, $"{entry}\"{mode}\"");
    }

    public static bool IsInUse(string compatDataDirectory)
    {
        var prefix = Path.Combine(compatDataDirectory, "pfx");
        string[] markers = [$"STEAM_COMPAT_DATA_PATH={compatDataDirectory}", $"WINEPREFIX={prefix}"];
        return LinuxProcesses.AnyEnvironment(environment => markers.Any(environment.Contains));
    }
}
