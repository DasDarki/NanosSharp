namespace NanosSharp.Packages;

internal static class PackageContext
{
    private static readonly AsyncLocal<PackageScope?> CurrentScope = new();

    public static PackageScope? Current => CurrentScope.Value;

    public static nint GlobalEnvironment { get; set; }

    public static nint Environment => Current switch
    {
        { IsActive: false } scope => throw new ObjectDisposedException(scope.Name,
            $"The package {scope.Name} was stopped, its code can no longer access nanos world."),
        { Environment: > 0 } scope => scope.Environment,
        _ => GlobalEnvironment,
    };

    public static bool CanAccessNative => Current switch
    {
        { IsActive: false } => false,
        { Environment: > 0 } => true,
        _ => GlobalEnvironment != 0,
    };

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
