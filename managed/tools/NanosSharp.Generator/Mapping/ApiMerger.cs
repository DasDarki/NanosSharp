using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Mapping;

internal static class ApiMerger
{
    public static PartitionedApi Merge(ApiModel server, ApiModel client)
    {
        var serverTypes = server.Types.ToDictionary(type => type.Name, StringComparer.Ordinal);
        var clientTypes = client.Types.ToDictionary(type => type.Name, StringComparer.Ordinal);
        var parts = serverTypes.Keys.Union(clientTypes.Keys)
            .Order(StringComparer.Ordinal)
            .Select(name => SplitType(serverTypes.GetValueOrDefault(name), clientTypes.GetValueOrDefault(name)))
            .ToArray();
        foreach (var part in parts)
        {
            PropertyBuilder.Build(part.Shared, part.Server, part.Client);
        }

        MoveUnimplementedMixinProperties(parts);
        return new PartitionedApi(
            new ApiModel(parts.Select(part => part.Shared).OfType<TypeModel>().ToArray(), server.Enums),
            new ApiModel(parts.Select(part => part.Server).OfType<TypeModel>().Where(IsNotEmpty).ToArray(), []),
            new ApiModel(parts.Select(part => part.Client).OfType<TypeModel>().Where(IsNotEmpty).ToArray(), []));
    }

    private static TypeParts SplitType(TypeModel? serverType, TypeModel? clientType)
    {
        if (clientType is null)
        {
            return new TypeParts(null, Copy(serverType!, true, serverType!.Interfaces, serverType.Methods, serverType.Events, serverType.Constructor), null);
        }

        if (serverType is null)
        {
            return new TypeParts(null, null, Copy(clientType, true, clientType.Interfaces, clientType.Methods, clientType.Events, clientType.Constructor));
        }

        var (shared, server, client) = Split(serverType, clientType);
        return new TypeParts(shared, server, client);
    }

    private static void MoveUnimplementedMixinProperties(IReadOnlyList<TypeParts> parts)
    {
        foreach (var mixin in parts.Where(part => part.Shared is { Kind: TypeKind.Mixin }))
        {
            var implementers = parts.Select(part => part.Shared).OfType<TypeModel>()
                .Where(type => type.Interfaces.Contains(mixin.Shared!.Name))
                .ToArray();
            foreach (var property in mixin.Shared!.Properties.ToArray())
            {
                var isImplemented = implementers.All(type => type.Properties.Any(candidate =>
                    candidate.Name == property.Name && (property.Setter is null || candidate.Setter is not null)));
                if (isImplemented)
                {
                    continue;
                }

                mixin.Shared.Properties.Remove(property);
                mixin.Server?.Properties.Add(property);
                mixin.Client?.Properties.Add(property);
            }
        }
    }

    private static (TypeModel Shared, TypeModel Server, TypeModel Client) Split(TypeModel serverType, TypeModel clientType)
    {
        var sharedInterfaces = serverType.Interfaces.Intersect(clientType.Interfaces, StringComparer.Ordinal).ToArray();
        var clientMethodKeys = clientType.Methods.Select(MethodKey).ToHashSet(StringComparer.Ordinal);
        var sharedMethods = serverType.Methods.Where(method => clientMethodKeys.Contains(MethodKey(method))).ToArray();
        var sharedMethodKeys = sharedMethods.Select(MethodKey).ToHashSet(StringComparer.Ordinal);
        var clientEventKeys = clientType.Events.Select(EventKey).ToHashSet(StringComparer.Ordinal);
        var sharedEvents = serverType.Events.Where(eventModel => clientEventKeys.Contains(EventKey(eventModel))).ToArray();
        var sharedEventKeys = sharedEvents.Select(EventKey).ToHashSet(StringComparer.Ordinal);
        var sharedConstructor = ConstructorKey(serverType.Constructor) == ConstructorKey(clientType.Constructor) ? serverType.Constructor : null;

        var shared = Copy(serverType, true, sharedInterfaces, sharedMethods, sharedEvents, sharedConstructor);
        var server = Copy(serverType, false, serverType.Interfaces.Except(sharedInterfaces).ToArray(),
            serverType.Methods.Where(method => !sharedMethodKeys.Contains(MethodKey(method))).ToArray(),
            serverType.Events.Where(eventModel => !sharedEventKeys.Contains(EventKey(eventModel))).ToArray(),
            sharedConstructor is null ? serverType.Constructor : null);
        var client = Copy(clientType, false, clientType.Interfaces.Except(sharedInterfaces).ToArray(),
            clientType.Methods.Where(method => !sharedMethodKeys.Contains(MethodKey(method))).ToArray(),
            clientType.Events.Where(eventModel => !sharedEventKeys.Contains(EventKey(eventModel))).ToArray(),
            sharedConstructor is null ? clientType.Constructor : null);
        return (shared, server, client);
    }

    private static TypeModel Copy(TypeModel source, bool isPrimary, IReadOnlyList<string> interfaces,
        IEnumerable<MethodModel> methods, IEnumerable<EventModel> events, ConstructorModel? constructor)
    {
        var copy = new TypeModel
        {
            Name = source.Name,
            Kind = source.Kind,
            Namespace = source.Namespace,
            ScriptClassName = source.ScriptClassName,
            IsPrimary = isPrimary,
            BaseType = source.BaseType,
            Interfaces = interfaces,
            Description = source.Description,
            Constructor = constructor,
        };
        copy.Methods.AddRange(methods);
        copy.Events.AddRange(events);
        return copy;
    }

    private static bool IsNotEmpty(TypeModel type) =>
        type.IsPrimary || type.Methods.Count > 0 || type.Properties.Count > 0 || type.Events.Count > 0 ||
        type.Interfaces.Count > 0 || type.Constructor is not null;

    private static string MethodKey(MethodModel method) =>
        $"{method.Name}({ParameterKey(method.Parameters)}):{method.ReturnType.Name}:{method.IsStatic}";

    private static string EventKey(EventModel eventModel) =>
        $"{eventModel.Name}({ParameterKey(eventModel.Arguments)}):{eventModel.ReturnType.Name}";

    private static string? ConstructorKey(ConstructorModel? constructor) =>
        constructor is null ? null : ParameterKey(constructor.Parameters);

    private static string ParameterKey(IEnumerable<ParameterModel> parameters) =>
        string.Join(",", parameters.Select(parameter => $"{parameter.Type.Name} {parameter.Name}{(parameter.IsOptional ? "?" : string.Empty)}"));

    private sealed record TypeParts(TypeModel? Shared, TypeModel? Server, TypeModel? Client);
}
