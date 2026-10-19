using System.Text;

namespace NanosSharp.Installer.Client;

internal sealed class KeyValues
{
    public static KeyValues Parse(string text)
    {
        var tokens = Tokenize(text).GetEnumerator();
        var root = new KeyValues();
        ParseInto(root, tokens);
        return root;
    }

    public IReadOnlyDictionary<string, KeyValues> Children => _children;

    private readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, KeyValues> _children = new(StringComparer.OrdinalIgnoreCase);

    public string? Value(string key) => _values.GetValueOrDefault(key);

    public KeyValues? Child(string key) => _children.GetValueOrDefault(key);

    private static void ParseInto(KeyValues target, IEnumerator<string> tokens)
    {
        while (tokens.MoveNext())
        {
            var key = tokens.Current;
            if (key == "}")
            {
                return;
            }

            if (!tokens.MoveNext())
            {
                return;
            }

            if (tokens.Current == "{")
            {
                var child = new KeyValues();
                ParseInto(child, tokens);
                target._children[key] = child;
                continue;
            }

            target._values[key] = tokens.Current;
        }
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        var index = 0;
        while (index < text.Length)
        {
            var character = text[index];
            if (char.IsWhiteSpace(character))
            {
                index++;
                continue;
            }

            if (character is '{' or '}')
            {
                index++;
                yield return character.ToString();
                continue;
            }

            if (character == '/' && index + 1 < text.Length && text[index + 1] == '/')
            {
                index = SkipLine(text, index);
                continue;
            }

            yield return character == '"' ? ReadQuoted(text, ref index) : ReadBare(text, ref index);
        }
    }

    private static int SkipLine(string text, int index)
    {
        var end = text.IndexOf('\n', index);
        return end < 0 ? text.Length : end + 1;
    }

    private static string ReadQuoted(string text, ref int index)
    {
        var builder = new StringBuilder();
        index++;
        while (index < text.Length && text[index] != '"')
        {
            if (text[index] == '\\' && index + 1 < text.Length)
            {
                index++;
            }

            builder.Append(text[index]);
            index++;
        }

        index++;
        return builder.ToString();
    }

    private static string ReadBare(string text, ref int index)
    {
        var start = index;
        while (index < text.Length && !char.IsWhiteSpace(text[index]) && text[index] is not ('{' or '}' or '"'))
        {
            index++;
        }

        return text[start..index];
    }
}
