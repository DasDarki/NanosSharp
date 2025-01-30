using System.Text;

namespace NanosSharp.Server.Bindings.Generator.SourceBuilder;

internal class ClassBuilder : ISourceBuilder
{
    private bool _isStatic;
    private readonly string _name;
    private readonly string? _inheritance;
    private readonly List<ISourceBuilder> _builders;

    internal ClassBuilder(bool isStatic, string name, string[] inheritances)
    {
        _isStatic = isStatic;
        _name = ISourceBuilder.MakeSafeName(name);
        _inheritance = GetMostValuableInheritance(inheritances);
        _builders = new List<ISourceBuilder>();
    }

    internal void AddStruct(string name, Action<StructBuilder> provider)
    {
        var builder = new StructBuilder(name);
        provider(builder);
        _builders.Add(builder);
    }
    
    internal void AddFunction(bool isStatic, string name, Action<FunctionBuilder> provider)
    {
        var builder = new FunctionBuilder(name, isStatic);
        provider(builder);
        _builders.Add(builder);
    }

    public string Generate(int indent = 0)
    {
        var sb = new StringBuilder();
        var access = _isStatic ? "static " : _name.StartsWith("Base") ? "abstract " : "";
        var extends = _inheritance != null ? $" : {_inheritance}" : "";
        sb.AppendLineWithIndent($"public {access}class {_name}{extends}", indent).AppendLineWithIndent("{", indent);

        foreach (var builder in _builders)
        {
            sb.AppendLineWithIndent(builder.Generate(indent + 1), indent);
        }

        sb.AppendLineWithIndent("}", indent);
        return sb.ToString();
    }

    private static string? GetMostValuableInheritance(string[] inheritances)
    {
        if (inheritances.Contains("Pickable")) 
        {
            return "Pickable";
        }
        
        if (inheritances.Contains("Paintable")) 
        {
            return "Paintable";
        }
        
        if (inheritances.Contains("Actor")) 
        {
            return "Actor";
        }
        
        return inheritances.Length > 0 ? inheritances[0] : null;
    }
}