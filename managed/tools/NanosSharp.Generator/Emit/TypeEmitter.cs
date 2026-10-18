using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Emit;

internal static class TypeEmitter
{
    private const int MaximumLineLength = 116;
    private const int MaximumTypedArguments = 8;
    private const string SelfArgumentName = "self";
    private const string HookPrefix = "Handle";

    private static readonly string[] Usings =
    [
        "NanosSharp.Entities", "NanosSharp.Entities.Binding", "NanosSharp.Enums", "NanosSharp.Interop.Reflection",
        "NanosSharp.Mathematics", "NanosSharp.Scripting",
    ];

    public static string Emit(TypeModel type)
    {
        var writer = new CodeWriter();
        writer.Line("#nullable enable");
        writer.BlankLine();
        foreach (var name in Usings.Where(name => name != type.Namespace))
        {
            writer.Line($"using {name};");
        }

        writer.BlankLine();
        writer.Line($"namespace {type.Namespace};");
        writer.BlankLine();
        if (type.IsPrimary)
        {
            WriteSummary(writer, type.Description);
            WriteDeclaration(writer, type);
        }
        else
        {
            WritePartialDeclaration(writer, type);
        }

        writer.OpenBlock();
        WriteMembers(writer, type);
        writer.CloseBlock();
        return writer.ToString();
    }

    private static void WriteDeclaration(CodeWriter writer, TypeModel type)
    {
        var bases = new[] { type.BaseType }.Concat(type.Interfaces).OfType<string>().ToArray();
        var inheritance = bases.Length == 0 ? string.Empty : " : " + string.Join(", ", bases);
        switch (type.Kind)
        {
            case TypeKind.StaticClass:
                writer.Line($"public static partial class {type.Name}");
                break;
            case TypeKind.Mixin:
                writer.Line($"public partial interface {type.Name}");
                break;
            case TypeKind.EntityBase:
                writer.Line($"public abstract partial class {type.Name}{inheritance}");
                break;
            default:
                writer.Line($"[ScriptClass(\"{type.ScriptClassName}\")]");
                writer.Line($"public partial class {type.Name}{inheritance}");
                break;
        }
    }

    private static void WritePartialDeclaration(CodeWriter writer, TypeModel type)
    {
        var inheritance = type.Interfaces.Count == 0 ? string.Empty : " : " + string.Join(", ", type.Interfaces);
        var declaration = type.Kind switch
        {
            TypeKind.StaticClass => "public static partial class",
            TypeKind.Mixin => "public partial interface",
            _ => "public partial class",
        };
        writer.Line($"{declaration} {type.Name}{inheritance}");
    }

    private static void WriteMembers(CodeWriter writer, TypeModel type)
    {
        var hasEvents = type.Kind is TypeKind.StaticClass or TypeKind.Entity && type.Events.Count > 0;
        if (hasEvents)
        {
            WriteClassEvents(writer, type);
        }

        WritePrivateStaticFields(writer, type, hasEvents);

        foreach (var property in type.Properties)
        {
            WriteProperty(writer, type, property);
            writer.BlankLine();
        }

        if (type.Kind == TypeKind.Entity)
        {
            WriteConstructors(writer, type);
        }

        foreach (var method in type.Methods)
        {
            WriteMethod(writer, type, method);
            writer.BlankLine();
        }

        if (!hasEvents)
        {
            return;
        }

        if (type.Kind == TypeKind.Entity)
        {
            WriteInstanceSubscriptions(writer, type);
            WriteEventHooks(writer, type);
        }

        foreach (var eventModel in type.Events)
        {
            WriteSummary(writer, eventModel.Description);
            writer.Line($"public delegate {eventModel.ReturnType} {eventModel.HandlerName}({FormatParameters(eventModel.Arguments)});");
            writer.BlankLine();
        }
    }

    private static void WriteClassEvents(CodeWriter writer, TypeModel type)
    {
        foreach (var eventModel in type.Events)
        {
            WriteSummary(writer, eventModel.Description);
            writer.Line($"public static event {eventModel.HandlerName}? On{eventModel.Name}");
            writer.OpenBlock();
            writer.Line($"add => {eventModel.Name}Event.Add(value);");
            writer.Line($"remove => {eventModel.Name}Event.Remove(value);");
            writer.CloseBlock();
            writer.BlankLine();
        }

    }

    private static void WritePrivateStaticFields(CodeWriter writer, TypeModel type, bool hasEvents)
    {
        var events = hasEvents ? type.Events : [];
        foreach (var eventModel in events)
        {
            writer.Line($"private static readonly ClassEvent<{eventModel.HandlerName}> {eventModel.Name}Event = " +
                        $"new(\"{type.ScriptClassName}\", \"{eventModel.Name}\");");
        }

        var methods = type.Kind == TypeKind.Mixin ? [] : type.Methods;
        foreach (var method in methods)
        {
            writer.Line(type.Kind == TypeKind.StaticClass
                ? $"private static readonly StaticMethodSlot {SlotName(method)} = new(\"{type.ScriptClassName}\", \"{method.Name}\");"
                : $"private static readonly MethodSlot {SlotName(method)} = new(\"{method.Name}\");");
        }

        writer.BlankLine();
    }

    private static string SlotName(MethodModel method) => $"{method.CSharpName}Slot";

    private static void WriteInstanceSubscriptions(CodeWriter writer, TypeModel type)
    {
        foreach (var eventModel in type.Events)
        {
            writer.Line($"/// <summary>Subscribes to the {eventModel.Name} event of this entity only.</summary>");
            writer.Line($"public ScriptSubscription Subscribe{eventModel.Name}({eventModel.HandlerName} handler) => " +
                        $"Subscribe(\"{eventModel.Name}\", handler);");
            writer.BlankLine();
        }
    }

    private static void WriteEventHooks(CodeWriter writer, TypeModel type)
    {
        foreach (var eventModel in type.Events.Where(eventModel => eventModel.Arguments is [{ Name: SelfArgumentName }, ..]))
        {
            var parameters = FormatParameters(eventModel.Arguments.Skip(1));
            writer.Line($"/// <summary>Called for the {eventModel.Name} event of this entity when a subclass overrides it.</summary>");
            writer.Line($"[EventHook(\"{eventModel.Name}\")]");
            if (eventModel.IsCancellable)
            {
                writer.Line($"protected virtual bool {HookPrefix}{eventModel.Name}({parameters}) => true;");
                writer.BlankLine();
                continue;
            }

            writer.Line($"protected virtual void {HookPrefix}{eventModel.Name}({parameters})");
            writer.OpenBlock();
            writer.CloseBlock();
            writer.BlankLine();
        }
    }

    private static void WriteProperty(CodeWriter writer, TypeModel type, PropertyModel property)
    {
        WriteSummary(writer, property.Getter.Description);
        if (type.Kind == TypeKind.Mixin)
        {
            var accessors = property.Setter is null ? "{ get; }" : "{ get; set; }";
            writer.Line($"{property.Type} {property.Name} {accessors}");
            return;
        }

        var modifiers = type.Kind == TypeKind.StaticClass ? "public static" : "public";
        if (property.Setter is null)
        {
            writer.Line($"{modifiers} {property.Type} {property.Name} => {property.Getter.CSharpName}();");
            return;
        }

        writer.Line($"{modifiers} {property.Type} {property.Name}");
        writer.OpenBlock();
        writer.Line($"get => {property.Getter.CSharpName}();");
        writer.Line($"set => {property.Setter.CSharpName}(value);");
        writer.CloseBlock();
    }

    private static void WriteConstructors(CodeWriter writer, TypeModel type)
    {
        if (type.Constructor is { } constructor)
        {
            WriteSummary(writer, constructor.Description);
            WriteSignature(writer, $"public {type.Name}(", constructor.Parameters, ")");
            writer.OpenBlock();
            var arguments = constructor.Parameters.Count == 0
                ? "ReadOnlySpan<object?>.Empty"
                : constructor.Parameters.Any(parameter => parameter.IsOptional)
                    ? FormatArguments(constructor.Parameters, constructor.RequiredParameterCount)
                    : $"[{FormatArguments(constructor.Parameters, constructor.RequiredParameterCount)}]";
            writer.Line($"SpawnSelf(\"{type.ScriptClassName}\", {arguments});");
            writer.CloseBlock();
            writer.BlankLine();
        }

        if (!type.IsPrimary || type.Constructor is { Parameters.Count: 0 })
        {
            return;
        }

        writer.Line($"protected {type.Name}()");
        writer.OpenBlock();
        writer.CloseBlock();
        writer.BlankLine();
    }

    private static void WriteMethod(CodeWriter writer, TypeModel type, MethodModel method)
    {
        WriteSummary(writer, method.Description);
        var parameters = FormatParameters(method.Parameters);
        if (type.Kind == TypeKind.Mixin)
        {
            writer.Line($"{method.ReturnType} {method.CSharpName}({parameters});");
            return;
        }

        var modifiers = type.Kind == TypeKind.StaticClass ? "public static" : "public";
        var declaration = $"{modifiers} {method.ReturnType} {method.CSharpName}({parameters}) =>";
        var invocation = $"{FormatInvocation(type, method)};";
        if (declaration.Length + invocation.Length + 1 <= MaximumLineLength)
        {
            writer.Line($"{declaration} {invocation}");
            return;
        }

        WriteSignature(writer, $"{modifiers} {method.ReturnType} {method.CSharpName}(", method.Parameters, ") =>");
        writer.Line($"    {invocation}");
    }

    private static void WriteSignature(CodeWriter writer, string prefix, IReadOnlyList<ParameterModel> parameters, string suffix)
    {
        var singleLine = prefix + FormatParameters(parameters) + suffix;
        if (singleLine.Length <= MaximumLineLength || parameters.Count == 0)
        {
            writer.Line(singleLine);
            return;
        }

        writer.Line(prefix);
        for (var index = 0; index < parameters.Count; index++)
        {
            var separator = index == parameters.Count - 1 ? suffix : ",";
            writer.Line($"    {FormatParameter(parameters[index])}{separator}");
        }
    }

    private static string FormatInvocation(TypeModel type, MethodModel method)
    {
        var variadic = method.Parameters.FirstOrDefault(parameter => parameter.Type.Kind == CSharpTypeKind.Variadic);
        return variadic is null && method.Parameters.Count <= MaximumTypedArguments
            ? FormatTypedInvocation(type, method)
            : FormatDynamicInvocation(type, method, variadic);
    }

    private static string FormatTypedInvocation(TypeModel type, MethodModel method)
    {
        var owner = type.Kind == TypeKind.StaticClass ? "ScriptApi." : string.Empty;
        var arguments = method.Parameters.Select(FormatArgument).Prepend(method.RequiredParameterCount.ToString())
            .Prepend(SlotName(method));
        var invocation = string.Join(", ", arguments);
        if (method.ReturnType.Kind == CSharpTypeKind.Void)
        {
            return $"{owner}Call({invocation})";
        }

        var typeArguments = method.Parameters.Select(ArgumentType).Prepend(method.ReturnType.Name);
        var nullForgiving = method.ReturnType.IsNullable ? string.Empty : "!";
        return $"{owner}CallResult<{string.Join(", ", typeArguments)}>({invocation}){nullForgiving}";
    }

    private static string FormatDynamicInvocation(TypeModel type, MethodModel method, ParameterModel? variadic)
    {
        var owner = type.Kind == TypeKind.StaticClass ? "ScriptApi." : string.Empty;
        var name = variadic is null ? "Invoke" : "InvokeVariadic";
        var generic = method.ReturnType.Kind == CSharpTypeKind.Void ? string.Empty : $"<{method.ReturnType}>";
        var arguments = variadic is null
            ? FormatArguments(method.Parameters, method.RequiredParameterCount)
            : $"[{string.Join(", ", method.Parameters.Where(parameter => parameter != variadic).Select(FormatArgument))}], {variadic.Name}";
        var separator = arguments.Length == 0 ? string.Empty : ", ";
        var nullForgiving = method.ReturnType.Kind == CSharpTypeKind.Void || method.ReturnType.IsNullable ? string.Empty : "!";
        return $"{owner}{name}{generic}({SlotName(method)}{separator}{arguments}){nullForgiving}";
    }

    private static string ArgumentType(ParameterModel parameter) =>
        parameter.Type.Kind == CSharpTypeKind.Callback ? "ScriptCallback?" : parameter.Type.Name;

    private static string FormatParameters(IEnumerable<ParameterModel> parameters) =>
        string.Join(", ", parameters.Select(FormatParameter));

    private static string FormatParameter(ParameterModel parameter)
    {
        if (parameter.Type.Kind == CSharpTypeKind.Variadic)
        {
            return $"params {parameter.Type} {parameter.Name}";
        }

        return parameter.IsOptional ? $"{parameter.Type} {parameter.Name} = null" : $"{parameter.Type} {parameter.Name}";
    }

    private static string FormatArguments(IReadOnlyList<ParameterModel> parameters, int requiredCount)
    {
        if (parameters.Count == 0)
        {
            return "[]";
        }

        var values = parameters.Select(FormatArgument).ToArray();
        if (parameters.Any(parameter => parameter.IsOptional))
        {
            return $"ScriptArguments.Trim([{string.Join(", ", values)}], {requiredCount})";
        }

        return string.Join(", ", values);
    }

    private static string FormatArgument(ParameterModel parameter) => parameter.Type.Kind switch
    {
        CSharpTypeKind.Callback => $"ScriptCallback.From({parameter.Name})",
        _ => parameter.Name,
    };

    private static void WriteSummary(CodeWriter writer, string? description)
    {
        var summary = DocumentationFormatter.Summary(description);
        if (summary is not null)
        {
            writer.Line($"/// <summary>{summary}</summary>");
        }
    }
}
