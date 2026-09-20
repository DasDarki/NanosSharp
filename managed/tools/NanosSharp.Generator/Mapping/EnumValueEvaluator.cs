using System.Globalization;

namespace NanosSharp.Generator.Mapping;

internal sealed class EnumValueEvaluator
{
    private readonly string _text;
    private int _position;

    private EnumValueEvaluator(string text)
    {
        _text = text;
    }

    public static bool TryEvaluate(string expression, out long value)
    {
        var evaluator = new EnumValueEvaluator(expression.Replace(" ", string.Empty, StringComparison.Ordinal));
        try
        {
            value = evaluator.ParseOr();
            return evaluator._position == evaluator._text.Length;
        }
        catch (FormatException)
        {
            value = 0;
            return false;
        }
    }

    private long ParseOr()
    {
        var value = ParseAdditive();
        while (TryConsume("|"))
        {
            value |= ParseAdditive();
        }

        return value;
    }

    private long ParseAdditive()
    {
        var value = ParseShift();
        while (true)
        {
            if (TryConsume("+"))
            {
                value += ParseShift();
            }
            else if (TryConsume("-"))
            {
                value -= ParseShift();
            }
            else
            {
                return value;
            }
        }
    }

    private long ParseShift()
    {
        var value = ParsePrimary();
        while (TryConsume("<<"))
        {
            value <<= (int)ParsePrimary();
        }

        return value;
    }

    private long ParsePrimary()
    {
        if (TryConsume("("))
        {
            var value = ParseOr();
            return TryConsume(")") ? value : throw new FormatException("missing closing parenthesis");
        }

        if (TryConsume("-"))
        {
            return -ParsePrimary();
        }

        var start = _position;
        while (_position < _text.Length && (char.IsAsciiHexDigit(_text[_position]) || _text[_position] is 'x' or 'X'))
        {
            _position++;
        }

        var literal = _text[start.._position];
        if (literal.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return long.Parse(literal[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return long.TryParse(literal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            ? number
            : throw new FormatException($"invalid literal '{literal}'");
    }

    private bool TryConsume(string token)
    {
        if (!_text.AsSpan(_position).StartsWith(token, StringComparison.Ordinal))
        {
            return false;
        }

        _position += token.Length;
        return true;
    }
}
