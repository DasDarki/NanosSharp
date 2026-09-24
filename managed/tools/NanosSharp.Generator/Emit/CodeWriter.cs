using System.Text;

namespace NanosSharp.Generator.Emit;

internal sealed class CodeWriter
{
    private const string Indentation = "    ";

    private readonly StringBuilder _builder = new();
    private int _depth;
    private bool _pendingBlankLine;

    public void Line(string text)
    {
        if (_pendingBlankLine)
        {
            _builder.Append('\n');
            _pendingBlankLine = false;
        }

        for (var level = 0; level < _depth; level++)
        {
            _builder.Append(Indentation);
        }

        _builder.Append(text).Append('\n');
    }

    public void BlankLine() => _pendingBlankLine = _builder.Length > 0;

    public void OpenBlock()
    {
        Line("{");
        _depth++;
        _pendingBlankLine = false;
    }

    public void CloseBlock(string suffix = "")
    {
        _depth--;
        _pendingBlankLine = false;
        Line("}" + suffix);
    }

    public override string ToString() => _builder.ToString();
}
