using Tomlyn;
using Tomlyn.Model;

namespace NanosSharp.Host.Configuration;

internal sealed class HostSettings
{
    private const string DefaultContent =
        """
        [hot_reload]
        enabled = true
        delay_milliseconds = 500
        verify_unload = true
        """;

    public bool HotReloadEnabled { get; private init; } = true;

    public TimeSpan HotReloadDelay { get; private init; } = TimeSpan.FromMilliseconds(500);

    public bool VerifyUnload { get; private init; } = true;

    public static HostSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, DefaultContent + Environment.NewLine);
        }

        var model = Toml.ToModel(File.ReadAllText(path), path);
        var hotReload = model.TryGetValue("hot_reload", out var table) ? table as TomlTable : null;
        return new HostSettings
        {
            HotReloadEnabled = Read(hotReload, "enabled", true),
            HotReloadDelay = TimeSpan.FromMilliseconds(Read(hotReload, "delay_milliseconds", 500L)),
            VerifyUnload = Read(hotReload, "verify_unload", true),
        };
    }

    private static T Read<T>(TomlTable? table, string key, T fallback) =>
        table is not null && table.TryGetValue(key, out var value) && value is T typed ? typed : fallback;
}
