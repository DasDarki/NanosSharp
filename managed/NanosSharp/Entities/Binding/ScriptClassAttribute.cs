namespace NanosSharp.Entities.Binding;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class ScriptClassAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
