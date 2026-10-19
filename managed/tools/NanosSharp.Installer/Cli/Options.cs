namespace NanosSharp.Installer.Cli;

internal sealed record Options(InstallTarget? Target, bool Uninstall, string? Path, string? Version, string? Source, bool AssumeYes, bool Force, bool ShowHelp);
