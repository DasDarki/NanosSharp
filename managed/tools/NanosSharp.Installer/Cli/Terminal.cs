namespace NanosSharp.Installer.Cli;

internal static class Terminal
{
    public static bool AssumeYes { get; set; }

    public static void Step(string message) => Write(ConsoleColor.Cyan, "==> ", message);

    public static void Info(string message) => Console.WriteLine($"    {message}");

    public static void Success(string message) => Write(ConsoleColor.Green, "OK  ", message);

    public static void Warning(string message) => Write(ConsoleColor.Yellow, "!   ", message);

    public static void Error(string message) => Write(ConsoleColor.Red, "ERR ", message);

    public static bool Confirm(string question, bool defaultAnswer)
    {
        if (AssumeYes || Console.IsInputRedirected)
        {
            return defaultAnswer;
        }

        Console.Write($"?   {question} {(defaultAnswer ? "[Y/n]" : "[y/N]")} ");
        var answer = Console.ReadLine()?.Trim().ToLowerInvariant();
        return string.IsNullOrEmpty(answer) ? defaultAnswer : answer is "y" or "yes";
    }

    public static string? Ask(string question)
    {
        if (AssumeYes || Console.IsInputRedirected)
        {
            return null;
        }

        Console.Write($"?   {question} ");
        var answer = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(answer) ? null : answer;
    }

    public static InstallTarget? Choose()
    {
        if (AssumeYes || Console.IsInputRedirected)
        {
            return null;
        }

        Console.WriteLine("?   What do you want to install?");
        Console.WriteLine("      1) the NanosSharp client for the game");
        Console.WriteLine("      2) NanosSharp for a nanos world server");
        Console.Write("    Choice [1/2]: ");
        return Console.ReadLine()?.Trim() switch
        {
            "1" => InstallTarget.Client,
            "2" => InstallTarget.Server,
            _ => null,
        };
    }

    public static void Progress(long received, long? total)
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        var text = total is > 0
            ? $"    {received / 1048576.0:0.0} / {total.Value / 1048576.0:0.0} MB"
            : $"    {received / 1048576.0:0.0} MB";
        Console.Write($"\r{text}   ");
    }

    public static void EndProgress()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.WriteLine();
        }
    }

    private static void Write(ConsoleColor color, string prefix, string message)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(prefix);
        Console.ForegroundColor = previous;
        Console.WriteLine(message);
    }
}
