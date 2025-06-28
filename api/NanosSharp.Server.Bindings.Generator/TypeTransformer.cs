namespace NanosSharp.Server.Bindings.Generator;

internal static class TypeTransformer
{
    internal static string Transform(string type, List<string> availableEnums)
    {
        if (type.Contains('|'))
        {
            type = type.Split('|')[0];
        }
        
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
        
        var enumType = availableEnums.FirstOrDefault(x => string.Equals(x, type, StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrEmpty(enumType))
        {
            return (isArray ? $"{enumType}[]" : enumType) + (isNullable ? "?" : "");
        }
        
        var basicType = type.ToLower() switch
        {
            "number" => "double",
            "integer" => "long",
            "string" => "string",
            "boolean" => "bool",
            "table" => "Dictionary<string, object>",
            "any" => "object",
            "function" => "ILuaVM.CFunction",
            _ => type.ToLower().EndsWith("path") ? "string" : "LuaRef"
        };
        
        return (isArray ? $"{basicType}[]" : basicType) + (isNullable ? "?" : "");
    }

    internal static string DeterminePush(string type, List<string> availableEnums)
    {
        if (type.Contains('|'))
        {
            type = type.Split('|')[0];
        }
        
        if (type.EndsWith("?"))
        {
            type = type[..^1];
        }
        
        if (type.EndsWith("[]"))
        {
            return "PushArray(%)";
        }
        
        var enumType = availableEnums.FirstOrDefault(x => string.Equals(x, type, StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrEmpty(enumType))
        {
            return "PushEnum(%)";
        }

        return type.ToLower() switch
        {
            "number" => "PushNumber(%)",
            "string" => "PushString(%)",
            "boolean" => "PushBoolean(%)",
            "table" => "PushTable(%)",
            "any" => "PushObject(%)",
            "function" => "PushManagedFunction(%)",
            _ => type.ToLower().EndsWith("path") ? "PushString(%)" : "RawGetI(ILuaVM.RegistryIndex, %)"
        };
    }

    internal static string DeterminePull(string type, List<string> availableEnums, out bool needPop)
    {
        needPop = true;

        if (type.Contains('|'))
        {
            type = type.Split('|')[0];
        }
        
        if (type.EndsWith("?"))
        {
            type = type[..^1];
        }
        
        if (type.EndsWith("[]"))
        {
            type = type[..^2];
            needPop = false;
            
            var enumType = availableEnums.FirstOrDefault(x => string.Equals(x, type, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrEmpty(enumType))
            {
                return $"ToArray<{enumType}[]>(-1)";
            }

            switch (type.ToLower())
            {
                case "number":
                    return "ToArray<double>(-1)";
                case "string":
                    return "ToArray<string>(-1)";
                case "boolean":
                    return "ToArray<boolean>(-1)";
                case "table":
                    return "ToArray<Dictionary<string, object>>(-1)";
                case "any":
                    return "ToArray(-1)";
                default:
                    if (type.ToLower().EndsWith("path"))
                    {
                        return "ToArray<string>(-1)";
                    }
                    
                    return "ToRefArray(-1)";
            }
        }
        
        var enumType2 = availableEnums.FirstOrDefault(x => string.Equals(x, type, StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrEmpty(enumType2))
        {
            return $"ToEnum<{enumType2}>(-1)";
        }

        switch (type.ToLower())
        {
            case "number":
                return "ToNumber(-1)";
            case "string":
                return "ToString(-1)";
            case "boolean":
                return "ToBoolean(-1)";
            case "table":
                return "ToTable(-1)";
            case "any":
                return "ToObject(-1)";
            case "function":
                return "ToCFunction(-1)";
            default:
                if (type.ToLower().EndsWith("path"))
                {
                    return "ToString(-1)";
                }
                
                needPop = false;
                return "Ref(ILuaVM.RegistryIndex)";
        }
    }
}