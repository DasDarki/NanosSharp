using System.Diagnostics.CodeAnalysis;

namespace NanosSharp.Packages;

/// <summary>
/// Typed access to the public APIs other C# packages publish, the type safe alternative to
/// <see cref="NanosSharp.Core.Exports"/> for package to package calls. A package publishes an API with
/// <see cref="Provide{TContract}"/> or the <see cref="PackageApiAttribute"/>, consumers resolve it by its
/// contract type. Values returned by <see cref="Get{TContract}"/> and <see cref="Require{TContract}"/> become
/// stale when the providing package reloads; use <see cref="Of{TContract}"/> for a handle that re resolves.
/// </summary>
public static class PackageApi
{
    /// <summary>Publishes an implementation under its contract for the current package, until it unloads.</summary>
    public static void Provide<TContract>(TContract implementation) where TContract : class
    {
        ArgumentNullException.ThrowIfNull(implementation);
        PackageApiRegistry.Provide(typeof(TContract), implementation);
    }

    /// <summary>Returns the implementation of a contract, or throws when no package provides it.</summary>
    public static TContract Require<TContract>() where TContract : class =>
        Get<TContract>() ?? throw new InvalidOperationException(
            $"No package provides an API of type {typeof(TContract).FullName}.");

    /// <summary>Returns the implementation of a contract, or null when no package provides it.</summary>
    public static TContract? Get<TContract>() where TContract : class =>
        PackageApiRegistry.Resolve(typeof(TContract)) as TContract;

    /// <summary>Tries to get the implementation of a contract.</summary>
    public static bool TryGet<TContract>([NotNullWhen(true)] out TContract? api) where TContract : class
    {
        api = Get<TContract>();
        return api is not null;
    }

    /// <summary>Returns a handle that re resolves the contract on every access, so it survives provider reloads.</summary>
    public static PackageApi<TContract> Of<TContract>() where TContract : class => default;
}

/// <summary>
/// A re resolving handle to another package's API. Reading <see cref="Value"/> looks the contract up again every
/// time, so the handle keeps working when the providing package is reloaded. The handle is stateless and safe to
/// store in a field, including via <c>[Inject]</c>; a bare field of this type works without any attribute.
/// </summary>
public readonly struct PackageApi<TContract> where TContract : class
{
    /// <summary>The current implementation, or null when no package provides the contract right now.</summary>
    public TContract? Value => PackageApiRegistry.Resolve(typeof(TContract)) as TContract;

    /// <summary>Whether a package currently provides the contract.</summary>
    public bool IsAvailable => Value is not null;

    /// <summary>The current implementation, or throws when no package provides the contract.</summary>
    public TContract Require() => Value ?? throw new InvalidOperationException(
        $"No package provides an API of type {typeof(TContract).FullName}.");
}
