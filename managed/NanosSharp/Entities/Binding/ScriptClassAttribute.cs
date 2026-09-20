namespace NanosSharp.Entities.Binding;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
internal sealed class ScriptClassAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
