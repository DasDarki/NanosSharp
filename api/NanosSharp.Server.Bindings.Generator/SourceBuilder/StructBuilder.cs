﻿using System.Text;

namespace NanosSharp.Server.Bindings.Generator.SourceBuilder;

internal class StructBuilder : ISourceBuilder
{
    private readonly string _name;
    private readonly Dictionary<string, string> _fields;
    
    internal StructBuilder(string name)
    {
        _name = name;
        _fields = new Dictionary<string, string>();
    }
    
    internal void AddField(string name, string type)
    {
        _fields.Add(name, type);
    }

    public string Generate(int indent = 0)
    {
        var sb = new StringBuilder();

        sb.AppendLineWithIndent($"public struct {_name}", indent);
        sb.AppendLineWithIndent("{", indent);
        
        foreach (var (name, type) in _fields)
        {
            sb.AppendLineWithIndent($"public {type} {name};", indent + 1);
        }
        
        sb.AppendLineWithIndent("}", indent);
        return sb.ToString();
    }
}