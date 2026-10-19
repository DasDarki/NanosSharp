namespace NanosSharp.Installer.Client;

internal static class LinuxProcesses
{
    public static bool AnyExecutable(Func<string, bool> predicate) =>
        Any("cmdline", commandLine => predicate(commandLine.Split('\n')[0]));

    public static bool AnyEnvironment(Func<string, bool> predicate) => Any("environ", predicate);

    private static bool Any(string file, Func<string, bool> predicate)
    {
        foreach (var directory in Directory.EnumerateDirectories("/proc"))
        {
            if (!int.TryParse(Path.GetFileName(directory), out var processId) || processId == Environment.ProcessId)
            {
                continue;
            }

            if (Read(Path.Combine(directory, file)) is { } content && predicate(content))
            {
                return true;
            }
        }

        return false;
    }

    private static string? Read(string path)
    {
        try
        {
            return File.ReadAllText(path).Replace('\0', '\n');
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
