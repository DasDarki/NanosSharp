namespace NanosSharp.Packages;

/// <summary>
/// Publishes a class as a typed package API that other packages resolve through <see cref="PackageApi"/>,
/// the type safe alternative to <see cref="NanosSharp.Core.Exports"/> for package to package calls. Without
/// contracts the class is published under itself and every non framework interface it implements; pass
/// contracts to publish it under specific interfaces only. The instance is the package's dependency injection
/// singleton, so it can inject other services and is injected into them.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PackageApiAttribute(params Type[] contracts) : Attribute
{
    /// <summary>The contract types the class is published under, empty for the default set.</summary>
    public IReadOnlyList<Type> Contracts { get; } = contracts;
}
