using NanosSharp.Generator.Mapping;
using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Emit;

internal static class EventClassEmitter
{
    public const string Namespace = "NanosSharp.Eventing";

    private static readonly HashSet<string> ReservedNames = ["Cancel", "IsCancelled"];

    private static readonly string[] Usings = ["NanosSharp.Entities", "NanosSharp.Enums", "NanosSharp.Mathematics"];

    public static IEnumerable<(TypeModel Owner, EventModel Event)> Collect(ApiModel model) =>
        model.Types
            .Where(type => type.Kind is TypeKind.StaticClass or TypeKind.Entity)
            .SelectMany(type => type.Events.Select(eventModel => (type, eventModel)));

    public static string ClassName(TypeModel owner, EventModel eventModel) => $"{owner.Name}{eventModel.Name}Event";

    public static string EmitEvent(TypeModel owner, EventModel eventModel)
    {
        var writer = CreateWriter();
        WriteSummary(writer, eventModel.Description);
        var baseType = eventModel.IsCancellable ? "CancellableEvent" : "ScriptEvent";
        writer.Line($"public sealed class {ClassName(owner, eventModel)} : {baseType}");
        writer.OpenBlock();
        foreach (var (argument, property) in Properties(owner, eventModel))
        {
            WriteSummary(writer, argument.Description);
            writer.Line($"public {argument.Type} {property} {{ get; init; }}{DefaultInitializer(owner, argument.Type)}");
            writer.BlankLine();
        }

        writer.CloseBlock();
        return writer.ToString();
    }

    public static string EmitCatalog(ApiModel model, string methodName)
    {
        var writer = CreateWriter();
        writer.Line("internal static partial class ScriptEventCatalog");
        writer.OpenBlock();
        writer.Line($"static partial void {methodName}(Dictionary<Type, ScriptEventDescriptor> descriptors)");
        writer.OpenBlock();
        foreach (var (owner, eventModel) in Collect(model))
        {
            var className = ClassName(owner, eventModel);
            writer.Line($"descriptors[typeof({className})] = new(\"{owner.ScriptClassName}\", \"{eventModel.Name}\", arguments => new {className}");
            writer.OpenBlock();
            var index = 0;
            foreach (var (argument, property) in Properties(owner, eventModel))
            {
                var forgiving = argument.Type.IsNullable ? string.Empty : "!";
                writer.Line($"{property} = ScriptEventArguments.Get<{argument.Type}>(arguments, {index++}){forgiving},");
            }

            writer.CloseBlock(");");
        }

        writer.CloseBlock();
        writer.CloseBlock();
        return writer.ToString();
    }

    private static IEnumerable<(ParameterModel Argument, string Property)> Properties(TypeModel owner, EventModel eventModel)
    {
        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var argument in eventModel.Arguments)
        {
            var name = argument.Name == "self" ? owner.Name : NameConverter.ToPascalCase(argument.Name.TrimStart('@'));
            if (ReservedNames.Contains(name) || !used.Add(name))
            {
                name += "Value";
                used.Add(name);
            }

            yield return (argument, name);
        }
    }

    private static string DefaultInitializer(TypeModel owner, CSharpType type) => type.Name == owner.Name ? " = null!;" : string.Empty;

    private static CodeWriter CreateWriter()
    {
        var writer = new CodeWriter();
        writer.Line("#nullable enable");
        writer.BlankLine();
        foreach (var name in Usings)
        {
            writer.Line($"using {name};");
        }

        writer.BlankLine();
        writer.Line($"namespace {Namespace};");
        writer.BlankLine();
        return writer;
    }

    private static void WriteSummary(CodeWriter writer, string? description)
    {
        var summary = DocumentationFormatter.Summary(description);
        if (summary is not null)
        {
            writer.Line($"/// <summary>{summary}</summary>");
        }
    }
}
