namespace NanosSharp.Installer;

internal static class InstallConfig
{
    private const string SettingsFileName = "NanosSharp.toml";

    public static bool AutoUpdateEnabled(string directory)
    {
        var path = Path.Combine(directory, SettingsFileName);
        if (!File.Exists(path))
        {
            return false;
        }

        var inUpdateSection = false;
        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Split('#', 2)[0].Trim();
            if (line.StartsWith('['))
            {
                inUpdateSection = line is "[update]";
                continue;
            }

            if (inUpdateSection && line.Replace(" ", string.Empty) is "enabled=true")
            {
                return true;
            }
        }

        return false;
    }
}
