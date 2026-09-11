namespace NanosSharp.Packages;

internal static class PackageContext
{
    private static readonly AsyncLocal<PackageScope?> CurrentScope = new();

    public static PackageScope? Current => CurrentScope.Value;

    public static nint GlobalEnvironment { get; set; }

    public static nint Environment => Current?.Environment is > 0 and var environment ? environment : GlobalEnvironment;

    public static Scope Enter(PackageScope? scope)
    {
        var previous = CurrentScope.Value;
        CurrentScope.Value = scope;
        return new Scope(previous);
    }

    public readonly struct Scope(PackageScope? previous) : IDisposable
    {
        public void Dispose() => CurrentScope.Value = previous;
    }
}
