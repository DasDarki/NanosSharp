namespace NanosSharp.Host.Packages.Dependencies;

internal static class LaunchConfiguration
{
    private const string ProcessName = "NanosWorldServer";

    public static void Write(string serverDirectory)
    {
        var directory = Path.Combine(serverDirectory, ".vscode");
        var path = Path.Combine(directory, "launch.json");
        if (File.Exists(path))
        {
            return;
        }

        Directory.CreateDirectory(directory);
        File.WriteAllText(path, $$"""
            {
              "version": "0.2.0",
              "configurations": [
                {
                  "name": "Attach to the NanosSharp server",
                  "type": "coreclr",
                  "request": "attach",
                  "processName": "{{ProcessName}}"
                }
              ]
            }
            """);
    }
}
