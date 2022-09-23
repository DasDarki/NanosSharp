using System.Text;

namespace NanosSharp.Server.Bindings.Generator.SourceBuilder;

internal static class StringBuilderExtensions
{
    internal static StringBuilder AppendLineWithIndent(this StringBuilder builder, string? line, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            builder.Append("    ");
        }

        builder.AppendLine(line);
        return builder;
    }
    
    internal static StringBuilder AppendWithIndent(this StringBuilder builder, string? line, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            builder.Append("    ");
        }

        builder.Append(line);
        return builder;
    }
}