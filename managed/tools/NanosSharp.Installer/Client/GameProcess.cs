using System.Diagnostics;

namespace NanosSharp.Installer.Client;

internal static class GameProcess
{
    public static bool IsRunning()
    {
        if (OperatingSystem.IsWindows())
        {
            return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(SteamLibrary.GameExecutable)).Length > 0;
        }

        return LinuxProcesses.AnyExecutable(executable => executable.EndsWith(SteamLibrary.GameExecutable, StringComparison.OrdinalIgnoreCase));
    }
}
