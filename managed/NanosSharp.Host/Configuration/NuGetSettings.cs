namespace NanosSharp.Host.Configuration;

internal sealed class NuGetSettings
{
    public const string DefaultSource = "https://api.nuget.org/v3/index.json";

    public List<string> Sources { get; set; } = [DefaultSource];
}
