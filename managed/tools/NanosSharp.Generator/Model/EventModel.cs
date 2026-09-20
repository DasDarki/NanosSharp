namespace NanosSharp.Generator.Model;

internal sealed record EventModel(string Name, string? Description, IReadOnlyList<ParameterModel> Arguments, CSharpType ReturnType)
{
    public string HandlerName => $"{Name}Handler";

    public bool IsCancellable => ReturnType.Name == "bool";
}
