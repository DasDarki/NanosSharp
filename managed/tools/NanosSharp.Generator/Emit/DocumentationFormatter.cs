using System.Net;
using System.Text.RegularExpressions;

namespace NanosSharp.Generator.Emit;

internal static partial class DocumentationFormatter
{
    public static string? Summary(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var text = WebUtility.HtmlDecode(description);
        text = CodeTag().Replace(text, match => $"\u0001{match.Groups[1].Value}\u0002");
        text = AnyTag().Replace(text, string.Empty);
        text = WebUtility.HtmlEncode(text).Replace("&#39;", "'").Replace("&quot;", "\"");
        text = text.Replace("\u0001", "<c>").Replace("\u0002", "</c>");
        text = Whitespace().Replace(text, " ").Trim();
        return text.Length == 0 ? null : text;
    }

    [GeneratedRegex("<code>(.*?)</code>", RegexOptions.Singleline)]
    private static partial Regex CodeTag();

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex AnyTag();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
