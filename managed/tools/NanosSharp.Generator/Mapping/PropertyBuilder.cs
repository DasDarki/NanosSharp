using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Mapping;

internal static class PropertyBuilder
{
    private static readonly HashSet<string> ReservedPropertyNames = ["ClassName", "IsValid", "NativeClass"];

    public static void Build(TypeModel? shared, TypeModel? server, TypeModel? client)
    {
        var parts = new[] { shared, server, client }.OfType<TypeModel>().ToArray();
        var memberNames = parts.SelectMany(part => part.Methods).Select(method => method.Name).ToHashSet(StringComparer.Ordinal);
        var typeName = parts[0].Name;
        foreach (var part in parts)
        {
            foreach (var getter in part.Methods.Where(IsGetter))
            {
                var name = getter.Name[(getter.Name.StartsWith("Get", StringComparison.Ordinal) ? 3 : 2)..];
                if (name.Length == 0 || !char.IsLetter(name[0]) || name == typeName || memberNames.Contains(name) || ReservedPropertyNames.Contains(name))
                {
                    continue;
                }

                if (part != shared)
                {
                    part.Properties.Add(Create(name, getter, FindSetter(name, getter, part, shared)));
                    continue;
                }

                var sharedSetter = FindSetter(name, getter, shared);
                var serverSetter = FindSetter(name, getter, server);
                var clientSetter = FindSetter(name, getter, client);
                if (sharedSetter is not null || (serverSetter is null && clientSetter is null))
                {
                    shared.Properties.Add(Create(name, getter, sharedSetter));
                    continue;
                }

                server?.Properties.Add(Create(name, getter, serverSetter));
                client?.Properties.Add(Create(name, getter, clientSetter));
            }
        }
    }

    private static PropertyModel Create(string name, MethodModel getter, MethodModel? setter) =>
        new(name, getter.ReturnType, getter, setter);

    private static MethodModel? FindSetter(string name, MethodModel getter, params TypeModel?[] parts) =>
        parts.OfType<TypeModel>()
            .SelectMany(part => part.Methods)
            .FirstOrDefault(method => method.Name == "Set" + name && IsMatchingSetter(method, getter.ReturnType));

    private static bool IsGetter(MethodModel method) =>
        method.Parameters.Count == 0 &&
        method.ReturnType.Kind != CSharpTypeKind.Void &&
        (method.Name.StartsWith("Get", StringComparison.Ordinal) || method.Name.StartsWith("Is", StringComparison.Ordinal));

    private static bool IsMatchingSetter(MethodModel setter, CSharpType type) =>
        setter.ReturnType.Kind == CSharpTypeKind.Void &&
        setter.Parameters.Count >= 1 &&
        setter.Parameters[0].Type == type &&
        setter.Parameters.Skip(1).All(parameter => parameter.IsOptional);
}
