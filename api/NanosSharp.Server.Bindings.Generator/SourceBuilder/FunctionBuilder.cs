using System.Text;

namespace NanosSharp.Server.Bindings.Generator.SourceBuilder;

internal class FunctionBuilder : ISourceBuilder
{
    private readonly string _name;
    private readonly List<string> _body;
    private readonly List<string> _params;
    private readonly List<string> _returns;

    internal FunctionBuilder(string name, bool isStatic)
    {
        _name = ISourceBuilder.MakeSafeName(name);
        _body = new List<string>();
        _params = new List<string> {"ILuaVM vm"};
        _returns = new List<string>();
        
        if (!isStatic)
        {
            _params.Add("LuaRef selfRef");
        }
    }

    internal FunctionBuilder Param(string type, string name)
    {
        _params.Add($"{type} {name}");
        return this;
    }
    
    internal FunctionBuilder Returns(string type)
    {
        _returns.Add(type);
        return this;
    }
    
    internal FunctionBuilder Body(Action<List<string>> body)
    {
        body(_body);
        return this;
    }
    
    public string Generate(int indent = 0)
    {
        var sb = new StringBuilder();

        string? returnType = null;
        switch (_returns.Count)
        {
            case > 1:
            {
                for (int i = 0; i < _returns.Count; i++)
                {
                    _params.Add($"out {_returns[i]} r{i}");
                }

                break;
            }
            case 1:
                returnType = _returns[0];
                break;
        }
        
        var paramIndex = _params.FindIndex(p => p.StartsWith("params"));
        if (paramIndex != -1)
        {
            var param = _params[paramIndex];
            _params.RemoveAt(paramIndex);
            _params.Add(param);
        }

        sb.AppendLineWithIndent($"public static {returnType ?? "void"} {_name}({string.Join(", ", _params)})", indent);
        sb.AppendLineWithIndent("{", indent);

        if (_returns.Count > 1)
        {
            for (int i = 0; i < _returns.Count; i++)
            {
                sb.AppendLineWithIndent($"r{i} = default;", indent + 1);
            }
        }

        foreach (var line in _body)
        {
            sb.AppendLineWithIndent(line, indent + 1);
        }
        
        sb.AppendLineWithIndent("}", indent);
        return sb.ToString();
    }
}