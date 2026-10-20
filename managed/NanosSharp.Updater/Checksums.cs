using System.Security.Cryptography;

namespace NanosSharp.Updater;

public static class Checksums
{
    public static Dictionary<string, string> Parse(string content)
    {
        var checksums = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separator = line.IndexOf(' ');
            if (separator <= 0)
            {
                continue;
            }

            var name = line[(separator + 1)..].TrimStart(' ', '*');
            checksums[name] = line[..separator].ToLowerInvariant();
        }

        return checksums;
    }

    public static bool Matches(string file, string expected)
    {
        using var stream = File.OpenRead(file);
        return Convert.ToHexStringLower(SHA256.HashData(stream)) == expected;
    }
}
