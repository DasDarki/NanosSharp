using NanosSharp.Generator.Model;

namespace NanosSharp.Generator.Emit;

internal static class EnumEmitter
{
    public const string Namespace = "NanosSharp.Enums";

    public static string Emit(EnumModel model)
    {
        var writer = new CodeWriter();
        writer.Line("#nullable enable");
        writer.BlankLine();
        writer.Line($"namespace {Namespace};");
        writer.BlankLine();
        WriteSummary(writer, model.Description);
        if (model.IsFlags)
        {
            writer.Line("[Flags]");
        }

        var underlyingType = model.Values.Any(value => value.Value is > int.MaxValue or < int.MinValue) ? " : long" : string.Empty;
        writer.Line($"public enum {model.Name}{underlyingType}");
        writer.OpenBlock();
        foreach (var value in model.Values.DistinctBy(value => value.Name))
        {
            WriteSummary(writer, value.Description);
            writer.Line($"{value.Name} = {value.Value},");
        }

        writer.CloseBlock();
        return writer.ToString();
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
