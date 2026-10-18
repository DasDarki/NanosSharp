using NanosSharp.Generator.Definitions;
using NanosSharp.Generator.Model;
using NanosSharp.Generator.Native;

namespace NanosSharp.Generator.Mapping;

internal sealed class ApiModelBuilder
{
    private const string EntitiesNamespace = "NanosSharp.Entities";
    private const string CoreNamespace = "NanosSharp.Core";
    private const string EntityBaseName = "Entity";
    private const string ActorBaseName = "Actor";
    private const string AsyncSuffix = "Async";

    private static readonly HashSet<string> SkippedClasses = ["Events", "Package", "Timer"];

    private static readonly Dictionary<string, string> RenamedClasses = new(StringComparer.Ordinal)
    {
        ["Console"] = "ScriptConsole",
        ["File"] = "ScriptFile",
    };

    private static readonly HashSet<string> ExcludedFunctions =
    [
        "Create", "Equals", "GetHashCode", "GetType", "IsValid", "Subscribe", "SubscribeRemote", "ToString", "Unsubscribe",
        "UnsubscribeRemote",
    ];


    private static readonly HashSet<string> ValueTypeNames =
    [
        "bool", "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "Vector", "Rotator", "Color",
        "Vector2D",
    ];

    private readonly NativeDump _native;
    private readonly ApiSide _side;
    private readonly DocsCatalog _docs;
    private readonly GeneratorReport _report;
    private readonly NativeTypeMapper _nativeTypes;
    private readonly DocsTypeMapper _docsTypes;
    private readonly Dictionary<string, TypeModel> _types = new(StringComparer.Ordinal);
    private readonly HashSet<string> _enumNames;

    public ApiModelBuilder(NativeDump native, ApiSide side, DocsCatalog docs, GeneratorReport report)
    {
        _native = native;
        _side = side;
        _docs = docs;
        _report = report;

        var entityClasses = native.Classes
            .Select(nativeClass => NameConverter.StripScriptingSuffix(nativeClass.Name))
            .Where(name => !SkippedClasses.Contains(name) && docs.Classes.ContainsKey(name) && !docs.IsStaticClass(name))
            .ToHashSet(StringComparer.Ordinal);
        var enumNames = docs.Enums.Keys.ToHashSet(StringComparer.Ordinal);
        _enumNames = enumNames;

        _nativeTypes = new NativeTypeMapper(entityClasses.ToDictionary(name => $"S{name}", CSharpName), enumNames, native.IsMsvc);
        _docsTypes = new DocsTypeMapper(entityClasses.Append(ActorBaseName).ToHashSet(StringComparer.Ordinal), enumNames, RenamedClasses);
    }

    public ApiModel Build()
    {
        foreach (var nativeClass in _native.Classes.OrderBy(nativeClass => nativeClass.Name, StringComparer.Ordinal))
        {
            BuildClass(nativeClass);
        }

        foreach (var type in _types.Values.Where(type => type.Kind == TypeKind.Entity).ToArray())
        {
            RemoveIncompleteMixins(type);
        }

        var types = _types.Values.OrderBy(type => type.Name, StringComparer.Ordinal).ToArray();
        foreach (var type in types)
        {
            type.Methods.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
        }

        return new ApiModel(types, BuildEnums());
    }

    private void Report(string section, string entry) => _report.Add($"{_side}: {section}", entry);

    private void RemoveIncompleteMixins(TypeModel type)
    {
        var methods = type.Methods.Select(method => method.Name).ToHashSet(StringComparer.Ordinal);
        var complete = type.Interfaces
            .Where(name => !_types.TryGetValue(name, out var mixin) || mixin.Methods.All(method => methods.Contains(method.Name)))
            .ToArray();
        foreach (var removed in type.Interfaces.Except(complete))
        {
            Report("Mixins that are not implemented completely", $"{type.Name}: {removed}");
        }

        type.Interfaces = complete;
    }

    private void BuildClass(NativeClassDump nativeClass)
    {
        var name = NameConverter.StripScriptingSuffix(nativeClass.Name);
        if (SkippedClasses.Contains(name))
        {
            Report("Skipped classes (implemented by hand)", nativeClass.Name);
            return;
        }

        var docs = _docs.FindClass(name);
        if (docs is null)
        {
            Report("Native classes without API definitions", nativeClass.Name);
            return;
        }

        if (_docs.IsStaticClass(name))
        {
            BuildStaticClass(nativeClass, name, docs);
            return;
        }

        BuildEntityClass(nativeClass, name, docs);
    }

    private void BuildStaticClass(NativeClassDump nativeClass, string name, DocsClass docs)
    {
        var type = new TypeModel
        {
            Name = CSharpName(name),
            Kind = TypeKind.StaticClass,
            Namespace = CoreNamespace,
            ScriptClassName = nativeClass.Name,
            Description = docs.Description,
        };
        _types[type.Name] = type;

        var functions = docs.StaticFunctions.ToDictionary(function => function.Name, StringComparer.Ordinal);
        foreach (var nativeMethod in GenerableMethods(nativeClass.StaticMethods))
        {
            if (!functions.TryGetValue(nativeMethod.Name, out var function))
            {
                Report("Native functions without documentation", $"{name}.{nativeMethod.Name}");
                continue;
            }

            AddIfBuilt(type, BuildMethod(name, nativeMethod, function, true));
        }

        ReportMissing(name, functions.Values.Where(function => function.IsAvailableOn(_side)).Select(function => function.Name), nativeClass.StaticMethods);
        type.Events.AddRange(BuildEvents(type.Name, docs.Events));
    }

    private void BuildEntityClass(NativeClassDump nativeClass, string name, DocsClass docs)
    {
        var mixins = docs.Inheritance.Where(parent => parent is not EntityBaseName and not ActorBaseName).Order(StringComparer.Ordinal).ToArray();
        var type = new TypeModel
        {
            Name = CSharpName(name),
            Kind = TypeKind.Entity,
            Namespace = EntitiesNamespace,
            ScriptClassName = nativeClass.Name,
            BaseType = docs.Inheritance.Contains(ActorBaseName) ? ActorBaseName : EntityBaseName,
            Interfaces = mixins.Select(mixin => $"I{mixin}").ToArray(),
            Description = docs.Description,
        };
        _types[type.Name] = type;

        var owners = CollectOwners(name, docs);
        foreach (var nativeMethod in GenerableMethods(nativeClass.Methods))
        {
            if (!owners.TryGetValue(nativeMethod.Name, out var owner))
            {
                var borrowed = _docs.FindBorrowedFunction(nativeMethod.Name, NativeParameterCount(nativeMethod));
                if (borrowed is null)
                {
                    Report("Native functions without documentation", $"{name}.{nativeMethod.Name}");
                    continue;
                }

                Report("Functions documented through other classes", $"{name}.{nativeMethod.Name}");
                owner = (borrowed, name);
            }

            var method = BuildMethod(name, nativeMethod, owner.Function, false);
            if (method is not null)
            {
                Assign(type, owner.Owner == name ? type.Name : owner.Owner, method);
            }
        }

        var ownFunctions = owners.Where(entry => entry.Value.Owner == name && entry.Value.Function.IsAvailableOn(_side)).Select(entry => entry.Key);
        ReportMissing(name, ownFunctions, nativeClass.Methods);

        var inheritedEvents = docs.Inheritance.Select(parent => _docs.FindClass(parent)).OfType<DocsClass>().SelectMany(parent => parent.Events);
        type.Events.AddRange(BuildEvents(type.Name, docs.Events.Concat(inheritedEvents)));

        var constructor = docs.Constructors.FirstOrDefault();
        if (nativeClass.Constructor is not null && constructor is not null)
        {
            type.Constructor = BuildConstructor(constructor, type.Name);
        }
    }

    private static int NativeParameterCount(NativeMethodDump nativeMethod)
    {
        try
        {
            return NativeSignature.Parse(nativeMethod.Signature).Parameters.Count;
        }
        catch (FormatException)
        {
            return -1;
        }
    }

    private static string CSharpName(string scriptName) => RenamedClasses.GetValueOrDefault(scriptName, scriptName);

    private Dictionary<string, (DocsFunction Function, string Owner)> CollectOwners(string name, DocsClass docs)
    {
        var owners = new Dictionary<string, (DocsFunction Function, string Owner)>(StringComparer.Ordinal);
        var sources = new[] { (Name: name, Class: docs) }
            .Concat(docs.Inheritance.Select(parent => (Name: parent, Class: _docs.FindClass(parent)!)).Where(source => source.Class is not null));
        foreach (var (owner, source) in sources)
        {
            foreach (var function in source.Functions)
            {
                if (!owners.TryGetValue(function.Name, out var existing) || IsBetterMatch(function, owner, existing))
                {
                    owners[function.Name] = (function, owner);
                }
            }
        }

        return owners;
    }

    private bool IsBetterMatch(DocsFunction function, string owner, (DocsFunction Function, string Owner) existing) =>
        existing.Owner == owner && !existing.Function.IsAvailableOn(_side) && function.IsAvailableOn(_side);

    private void Assign(TypeModel type, string owner, MethodModel method)
    {
        if (owner == type.Name)
        {
            type.Methods.Add(method);
            return;
        }

        AddShared(GetSharedType(owner), method);
        if (owner is not EntityBaseName and not ActorBaseName)
        {
            type.Methods.Add(method);
        }
    }

    private TypeModel GetSharedType(string owner)
    {
        var isBase = owner is EntityBaseName or ActorBaseName;
        var name = isBase ? owner : $"I{owner}";
        if (_types.TryGetValue(name, out var existing))
        {
            return existing;
        }

        var type = new TypeModel
        {
            Name = name,
            Kind = isBase ? TypeKind.EntityBase : TypeKind.Mixin,
            Namespace = EntitiesNamespace,
            BaseType = owner == ActorBaseName ? EntityBaseName : null,
            Description = _docs.FindClass(owner)?.Description,
        };
        _types[name] = type;
        return type;
    }

    private static void AddShared(TypeModel type, MethodModel method)
    {
        if (type.Methods.All(existing => existing.Name != method.Name))
        {
            type.Methods.Add(method);
        }
    }

    private static void AddIfBuilt(TypeModel type, MethodModel? method)
    {
        if (method is not null)
        {
            type.Methods.Add(method);
        }
    }

    private MethodModel? BuildMethod(string className, NativeMethodDump nativeMethod, DocsFunction function, bool isStatic)
    {
        try
        {
            var signature = NativeSignature.Parse(nativeMethod.Signature);
            var returnType = MapReturn(signature.ReturnType!, function.Return.FirstOrDefault()?.Type);
            var parameters = signature.Parameters
                .Select((parameter, index) => BuildParameter(parameter, function, index))
                .Select(parameter => RequireCompletionCallback(nativeMethod.Name, parameter))
                .ToList();
            if (signature.Parameters.Count != function.Parameters.Count)
            {
                Report("Parameter count differs from documentation",
                    $"{className}.{nativeMethod.Name}: native {signature.Parameters.Count}, documented {function.Parameters.Count}");
            }

            return new MethodModel(nativeMethod.Name, returnType, NormalizeParameters(parameters), isStatic, function.Description);
        }
        catch (Exception exception) when (exception is TypeMapException or FormatException)
        {
            Report("Functions with unsupported signatures", $"{className}.{nativeMethod.Name}: {exception.Message}");
            return null;
        }
    }

    private CSharpType MapReturn(CppType nativeType, string? docsType)
    {
        var mapped = _nativeTypes.Map(nativeType, false);
        var type = ApplyDocumentedEntity(ApplyDocumentedEnum(mapped.Type, docsType), docsType);
        return mapped.IsOptional ? type.AsNullable() : type;
    }

    private CSharpType ApplyDocumentedEntity(CSharpType type, string? docsType)
    {
        if (docsType is null || type != CSharpType.Object)
        {
            return type;
        }

        var documented = _docsTypes.Map(docsType);
        return _docsTypes.IsEntityType(documented.Name) ? documented.AsNullable() : type;
    }

    private ParameterModel BuildParameter(CppType nativeType, DocsFunction function, int index)
    {
        var mapped = _nativeTypes.Map(nativeType, true);
        var docsParameter = index < function.Parameters.Count ? function.Parameters[index] : null;
        var fallbackName = mapped.Type.Kind == CSharpTypeKind.Variadic ? "arguments" : $"argument{index + 1}";
        var name = docsParameter is null ? fallbackName : NameConverter.ToCamelCase(docsParameter.Name);
        var type = ApplyDocumentedEnum(mapped.Type, docsParameter?.Type);
        return new ParameterModel(name, mapped.IsOptional ? type.AsNullable() : type, mapped.IsOptional, docsParameter?.Description);
    }

    private static ParameterModel RequireCompletionCallback(string methodName, ParameterModel parameter) =>
        methodName.EndsWith(AsyncSuffix, StringComparison.Ordinal) && parameter.Type.Kind == CSharpTypeKind.Callback
            ? parameter with { Type = CSharpType.Callback, IsOptional = false }
            : parameter;

    private CSharpType ApplyDocumentedEnum(CSharpType type, string? docsType)
    {
        var enumName = docsType is null ? null : _docsTypes.FindEnum(docsType);
        return enumName is not null && NativeTypeMapper.IsIntegral(type) ? new CSharpType(enumName) : type;
    }

    private ConstructorModel BuildConstructor(DocsFunction constructor, string className)
    {
        var parameters = constructor.Parameters.Select(parameter =>
        {
            var isOptional = parameter.Default is not null || parameter.Type.EndsWith('?');
            var type = _docsTypes.Map(parameter.Type);
            return new ParameterModel(NameConverter.ToCamelCase(parameter.Name), isOptional ? type.AsNullable() : type, isOptional,
                parameter.Description);
        }).ToList();

        return new ConstructorModel(NormalizeParameters(parameters), $"Spawns a new {className}.");
    }

    private IEnumerable<EventModel> BuildEvents(string className, IEnumerable<DocsEvent> events) =>
        events
            .Where(docsEvent => docsEvent.IsAvailableOn(_side))
            .DistinctBy(docsEvent => docsEvent.Name)
            .OrderBy(docsEvent => docsEvent.Name, StringComparer.Ordinal)
            .Select(docsEvent => BuildEvent(className, docsEvent));

    private EventModel BuildEvent(string className, DocsEvent docsEvent)
    {
        var arguments = docsEvent.Arguments.Select((argument, index) =>
        {
            var isSelf = index == 0 && argument.Name == "self";
            var type = isSelf ? new CSharpType(className) : _docsTypes.Map(argument.Type);
            var nullableType = isSelf || type.Kind == CSharpTypeKind.Callback || IsValueType(type) ? type : type.AsNullable();
            return new ParameterModel(NameConverter.ToCamelCase(argument.Name), nullableType, false, argument.Description);
        }).ToList();

        var returnType = docsEvent.Return.FirstOrDefault()?.Type == "boolean" ? new CSharpType("bool") : CSharpType.Void;
        return new EventModel(NameConverter.ToPascalCase(docsEvent.Name), docsEvent.Description, NormalizeParameters(arguments), returnType);
    }

    private bool IsValueType(CSharpType type) => ValueTypeNames.Contains(type.Name) || _enumNames.Contains(type.Name);

    private static List<ParameterModel> NormalizeParameters(List<ParameterModel> parameters)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < parameters.Count; index++)
        {
            var parameter = parameters[index];
            var name = names.Add(parameter.Name) ? parameter.Name : $"{parameter.Name}{index + 1}";
            names.Add(name);
            parameters[index] = parameter with { Name = name };
        }

        var lastRequired = parameters.FindLastIndex(parameter => !parameter.IsOptional && parameter.Type.Kind != CSharpTypeKind.Variadic);
        for (var index = 0; index < lastRequired; index++)
        {
            parameters[index] = parameters[index] with { IsOptional = false };
        }

        return parameters;
    }

    private IReadOnlyList<EnumModel> BuildEnums() =>
        _docs.Enums.OrderBy(entry => entry.Key, StringComparer.Ordinal).Select(entry => new EnumModel(
            entry.Key,
            entry.Value.Description,
            entry.Value.Enums.Select(value => BuildEnumValue(entry.Key, value)).OfType<EnumValueModel>().ToArray(),
            entry.Value.Enums.Any(value => value.Value.Contains("<<", StringComparison.Ordinal)))).ToArray();

    private EnumValueModel? BuildEnumValue(string enumName, DocsEnumValue value)
    {
        if (!EnumValueEvaluator.TryEvaluate(value.Value, out var number))
        {
            Report("Enum values with unsupported values", $"{enumName}.{value.Key} = {value.Value}");
            return null;
        }

        return new EnumValueModel(NameConverter.ToPascalCase(value.Key), number, value.Description);
    }

    private void ReportMissing(string className, IEnumerable<string> documented, IEnumerable<NativeMethodDump> nativeMethods)
    {
        var nativeNames = nativeMethods.Select(method => method.Name).ToHashSet(StringComparer.Ordinal);
        foreach (var missing in documented.Where(name => !nativeNames.Contains(name)))
        {
            Report("Documented functions without native implementation", $"{className}.{missing}");
        }
    }

    private static IEnumerable<NativeMethodDump> GenerableMethods(IEnumerable<NativeMethodDump> methods) =>
        methods
            .Where(method => !ExcludedFunctions.Contains(method.Name))
            .Where(method => !method.Name.StartsWith("__", StringComparison.Ordinal))
            .Where(method => !method.Name.EndsWith("_I", StringComparison.Ordinal))
            .Where(method => !method.Name.EndsWith("_DEPRECATED", StringComparison.Ordinal))
            .OrderBy(method => method.Name, StringComparer.Ordinal);
}
