using NanosSharp.Packages;

namespace NanosSharp.Entities.Binding;

internal sealed record FactoryRegistration(Type EntityType, Func<Entity> Create, PackageScope? Scope);
