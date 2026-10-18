using NanosSharp.Generator.Model;
using NanosSharp.Generator.Native;

namespace NanosSharp.Generator.Mapping;

internal sealed class NativeTypeMapper(IReadOnlyDictionary<string, string> entityTypes, IReadOnlySet<string> enumNames, bool isMsvc)
{
    private const string EntityPrefix = "S";
    private const string UnrealEnumType = "int";

    private static readonly Dictionary<string, string> PrimitiveTypes = new(StringComparer.Ordinal)
    {
        ["bool"] = "bool",
        ["char"] = "sbyte",
        ["signed char"] = "sbyte",
        ["unsigned char"] = "byte",
        ["short"] = "short",
        ["unsigned short"] = "ushort",
        ["int"] = "int",
        ["unsigned int"] = "uint",
        ["long"] = "long",
        ["unsigned long"] = "ulong",
        ["long long"] = "long",
        ["unsigned long long"] = "ulong",
        ["float"] = "float",
        ["double"] = "double",
        ["std::string"] = "string",
        ["std::basic_string"] = "string",
        ["std::basic_string_view"] = "string",
        ["nw::structs::Vector"] = "Vector",
        ["nw::structs::Rotator"] = "Rotator",
        ["nw::structs::Color"] = "Color",
        ["nw::structs::Vector2D"] = "Vector2D",
        ["nw::structs::Entity"] = "Entity",
        ["nw::structs::Tuple"] = "object?[]",
        ["nw::scripting::ScriptValue"] = "object?",
    };

    private static readonly Dictionary<string, string> MsvcPrimitiveTypes = new(StringComparer.Ordinal)
    {
        ["long"] = "int",
        ["unsigned long"] = "uint",
        ["__int64"] = "long",
        ["unsigned __int64"] = "ulong",
    };

    private static readonly Dictionary<string, string> EnumAliases = new(StringComparer.Ordinal)
    {
        ["EventsReliability"] = "Reliability",
        ["AssetsType"] = "AssetType",
    };

    private static readonly HashSet<string> IntegralTypes = ["sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong"];

    public MappedType Map(CppType type, bool isParameter) => type.Name switch
    {
        "void" => new MappedType(CSharpType.Void, false),
        "std::optional" => new MappedType(Map(type.Argument(0), isParameter).Type, true),
        "std::vector" => new MappedType(MapSequence(type.Argument(0), isParameter), false),
        "std::map" or "std::unordered_map" => new MappedType(MapDictionary(type, isParameter), false),
        "std::shared_ptr" => new MappedType(MapSharedPointer(type.Argument(0), isParameter), false),
        "nw::scripting::ScriptVariadicArray" => new MappedType(CSharpType.Variadic, false),
        "nw::structs::DeprecatedParam" => new MappedType(CSharpType.Object, false),
        _ => new MappedType(MapSimple(type), false),
    };

    public static bool IsIntegral(CSharpType type) => IntegralTypes.Contains(type.Name.TrimEnd('?'));

    private CSharpType MapSimple(CppType type)
    {
        if (isMsvc && MsvcPrimitiveTypes.TryGetValue(type.Name, out var msvcPrimitive))
        {
            return new CSharpType(msvcPrimitive);
        }

        if (PrimitiveTypes.TryGetValue(type.Name, out var primitive))
        {
            return new CSharpType(primitive);
        }

        var shortName = ShortName(type.Name);
        if (type.IsPointer && shortName.StartsWith(EntityPrefix, StringComparison.Ordinal))
        {
            return new CSharpType(entityTypes.GetValueOrDefault(shortName, "Entity"));
        }

        var enumName = EnumAliases.GetValueOrDefault(shortName, shortName);
        if (enumNames.Contains(enumName))
        {
            return new CSharpType(enumName);
        }

        if (type.IsEnum)
        {
            return new CSharpType(UnrealEnumType);
        }

        throw new TypeMapException($"unsupported native type {type}");
    }

    private static string ShortName(string name)
    {
        var scope = name.EndsWith("::Type", StringComparison.Ordinal) ? name[..^"::Type".Length] : name;
        return scope[(scope.LastIndexOf(':') + 1)..];
    }

    private CSharpType MapSequence(CppType element, bool isParameter)
    {
        var elementType = Map(element, isParameter).Type;
        return new CSharpType(isParameter ? $"IEnumerable<{elementType}>" : $"IReadOnlyList<{elementType}>");
    }

    private CSharpType MapDictionary(CppType type, bool isParameter)
    {
        var key = Map(type.Argument(0), isParameter).Type;
        var value = Map(type.Argument(1), isParameter).Type;
        return new CSharpType(isParameter ? $"IDictionary<{key}, {value}>" : $"IReadOnlyDictionary<{key}, {value}>");
    }

    private static CSharpType MapSharedPointer(CppType pointee, bool isParameter)
    {
        if (pointee.Name != "nw::scripting::impl::IScriptFunctionRef")
        {
            throw new TypeMapException($"unsupported shared pointer to {pointee}");
        }

        return isParameter ? CSharpType.Callback : new CSharpType("ScriptFunction");
    }
}
