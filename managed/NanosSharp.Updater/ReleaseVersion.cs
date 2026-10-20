namespace NanosSharp.Updater;

public static class ReleaseVersion
{
    public static bool IsNewer(string candidate, string current) => Compare(candidate, current) > 0;

    public static int Compare(string left, string right)
    {
        var (leftCore, leftPre) = Split(left);
        var (rightCore, rightPre) = Split(right);
        for (var index = 0; index < 3; index++)
        {
            var result = leftCore[index].CompareTo(rightCore[index]);
            if (result != 0)
            {
                return result;
            }
        }

        if (leftPre == rightPre)
        {
            return 0;
        }

        if (leftPre.Length == 0)
        {
            return 1;
        }

        if (rightPre.Length == 0)
        {
            return -1;
        }

        return string.CompareOrdinal(leftPre, rightPre);
    }

    private static (int[] Core, string PreRelease) Split(string version)
    {
        version = version.TrimStart('v');
        var dash = version.IndexOf('-');
        var pre = dash < 0 ? string.Empty : version[(dash + 1)..];
        var core = (dash < 0 ? version : version[..dash]).Split('.');
        var numbers = new int[3];
        for (var index = 0; index < 3 && index < core.Length; index++)
        {
            int.TryParse(core[index], out numbers[index]);
        }

        return (numbers, pre);
    }
}
