namespace NanosSharp.Server.Bindings.Generator;

internal static class TypeTransformer
{
    internal static string Transform(string type)
    {
        var isNullable = type.EndsWith("?");
        if (isNullable)
        {
            type = type[..^1];
        }
        
        var isArray = type.EndsWith("[]");
        if (isArray)
        {
            type = type[..^2];
        }
        
        var basicType = type.ToLower() switch
        {
            "number" => "double",
            "string" => "string",
            "boolean" => "bool",
            "table" => "Dictionary<string, object>",
            "any" => "object",
            _ => type.EndsWith("path") ? "string" : "int"
        };
        
        return (isArray ? $"{basicType}[]" : basicType) + (isNullable ? "?" : "");
    }

    internal static string DeterminePush(string type)
    {
        if (type.EndsWith("?"))
        {
            type = type[..^1];
        }
        
        if (type.EndsWith("[]"))
        {
            type = type[..^2];
        }

        return type.ToLower() switch
        {
            "number" => "PushNumber(%)",
            "string" => "PushString(%)",
            "boolean" => "PushBoolean(%)",
            "table" => "Dictionary<string, object>",
            _ => type.EndsWith("path") ? "PushString(%)" : "RawGetI(ILuaVM.RegistryIndex, %)"
        };
    }

    internal static string DeterminePull(string type, out bool needPop)
    {
        if (type.EndsWith("?"))
        {
            type = type[..^1];
        }
        
        if (type.EndsWith("[]"))
        {
            type = type[..^2];
        }
        
        needPop = true;

        switch (type.ToLower())
        {
            case "number":
                return "ToNumber(-1)";
            case "string":
                return "ToString(-1)";
            case "boolean":
                return "ToBoolean(-1)";
            case "table":
                return "Dictionary<string, object>";
            default:
                if (type.EndsWith("path"))
                {
                    return "ToString(-1)";
                }
                
                needPop = false;
                return "Ref(ILuaVM.RegistryIndex)";
        }
    }
}