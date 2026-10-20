namespace NanosSharp.Installer.Cli;

internal static class OptionsParser
{
    public static Options Parse(IReadOnlyList<string> arguments)
    {
        InstallTarget? target = null;
        var uninstall = false;
        var update = false;
        string? path = null;
        string? version = null;
        string? source = null;
        var assumeYes = false;
        var force = false;
        var quiet = false;
        var ifEnabled = false;
        var showHelp = false;

        for (var index = 0; index < arguments.Count; index++)
        {
            switch (arguments[index])
            {
                case "client":
                    target = InstallTarget.Client;
                    break;
                case "server":
                    target = InstallTarget.Server;
                    break;
                case "uninstall" or "--uninstall":
                    uninstall = true;
                    break;
                case "update" or "--update":
                    update = true;
                    break;
                case "--quiet":
                    quiet = true;
                    break;
                case "--if-enabled":
                    ifEnabled = true;
                    break;
                case "--path":
                    path = ReadValue(arguments, ref index);
                    break;
                case "--version":
                    version = ReadValue(arguments, ref index).TrimStart('v');
                    break;
                case "--from":
                    source = Path.GetFullPath(ReadValue(arguments, ref index));
                    break;
                case "-y" or "--yes":
                    assumeYes = true;
                    break;
                case "--force":
                    force = true;
                    break;
                case "-h" or "--help":
                    showHelp = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{arguments[index]}'.");
            }
        }

        return new Options(target, uninstall, update, path, version, source, assumeYes, force, quiet, ifEnabled, showHelp);
    }

    private static string ReadValue(IReadOnlyList<string> arguments, ref int index)
    {
        if (index + 1 >= arguments.Count)
        {
            throw new ArgumentException($"{arguments[index]} needs a value.");
        }

        index++;
        return arguments[index];
    }
}
