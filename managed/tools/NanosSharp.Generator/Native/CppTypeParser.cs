using System.Text;

namespace NanosSharp.Generator.Native;

internal sealed class CppTypeParser
{
    private static readonly string[] IgnoredQualifiers = ["const", "volatile", "struct", "class", "enum"];

    private readonly string _text;
    private int _position;

    private CppTypeParser(string text)
    {
        _text = text;
    }

    public static CppType Parse(string text)
    {
        var parser = new CppTypeParser(text);
        var type = parser.ParseType();
        parser.SkipWhitespace();
        return parser._position == text.Length
            ? type
            : throw new FormatException($"Unexpected '{text[parser._position..]}' in '{text}'.");
    }

    private CppType ParseType()
    {
        var name = ReadName();
        var arguments = new List<CppType>();
        SkipWhitespace();
        if (Peek() == '<')
        {
            _position++;
            arguments.AddRange(ParseArguments());
        }

        var (isPointer, isReference) = ReadSuffix();
        return new CppType(name, arguments, isPointer, isReference);
    }

    private List<CppType> ParseArguments()
    {
        var arguments = new List<CppType>();
        SkipWhitespace();
        while (Peek() != '>')
        {
            arguments.Add(ParseType());
            SkipWhitespace();
            if (Peek() == ',')
            {
                _position++;
                SkipWhitespace();
            }
        }

        _position++;
        return arguments;
    }

    private string ReadName()
    {
        var words = new List<string>();
        SkipWhitespace();
        while (_position < _text.Length && IsNameCharacter(_text[_position]))
        {
            var word = ReadWord();
            if (!IgnoredQualifiers.Contains(word))
            {
                words.Add(word);
            }

            SkipWhitespace();
        }

        return string.Join(' ', words);
    }

    private string ReadWord()
    {
        var builder = new StringBuilder();
        while (_position < _text.Length && IsNameCharacter(_text[_position]))
        {
            builder.Append(_text[_position++]);
        }

        return builder.ToString();
    }

    private (bool IsPointer, bool IsReference) ReadSuffix()
    {
        var isPointer = false;
        var isReference = false;
        while (true)
        {
            SkipWhitespace();
            var current = Peek();
            if (current is '*')
            {
                isPointer = true;
            }
            else if (current is '&')
            {
                isReference = true;
            }
            else if (_text.AsSpan(_position).StartsWith("const"))
            {
                _position += "const".Length - 1;
            }
            else
            {
                return (isPointer, isReference);
            }

            _position++;
        }
    }

    private void SkipWhitespace()
    {
        while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
        {
            _position++;
        }
    }

    private char Peek() => _position < _text.Length ? _text[_position] : '\0';

    private static bool IsNameCharacter(char character) => char.IsLetterOrDigit(character) || character is '_' or ':';
}
