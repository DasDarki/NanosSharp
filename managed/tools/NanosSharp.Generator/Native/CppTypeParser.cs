using System.Text;

namespace NanosSharp.Generator.Native;

internal sealed class CppTypeParser
{
    private const string EnumQualifier = "enum";
    private const string PointerSizeQualifier = "__ptr64";

    private static readonly string[] IgnoredQualifiers = ["const", "volatile", "struct", "class", EnumQualifier];

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
        var (name, isEnum) = ReadName();
        var arguments = new List<CppType>();
        SkipWhitespace();
        if (Peek() == '<')
        {
            _position++;
            arguments.AddRange(ParseArguments());
        }

        var (isPointer, isReference) = ReadSuffix();
        return new CppType(name, arguments, isPointer, isReference, isEnum);
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

    private (string Name, bool IsEnum) ReadName()
    {
        var words = new List<string>();
        var isEnum = false;
        SkipWhitespace();
        while (_position < _text.Length && IsNameCharacter(_text[_position]))
        {
            var word = ReadWord();
            isEnum |= word == EnumQualifier;
            if (!IgnoredQualifiers.Contains(word))
            {
                words.Add(word);
            }

            SkipWhitespace();
        }

        return (string.Join(' ', words), isEnum);
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
            else if (_text.AsSpan(_position).StartsWith(PointerSizeQualifier))
            {
                _position += PointerSizeQualifier.Length - 1;
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
