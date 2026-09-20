namespace NanosSharp.Generator.Model;

internal sealed record CSharpType(string Name, CSharpTypeKind Kind = CSharpTypeKind.Value)
{
    public static CSharpType Void { get; } = new("void", CSharpTypeKind.Void);

    public static CSharpType Object { get; } = new("object?");

    public static CSharpType Callback { get; } = new("Delegate", CSharpTypeKind.Callback);

    public static CSharpType Variadic { get; } = new("object?[]", CSharpTypeKind.Variadic);

    public bool IsNullable => Name.EndsWith('?');

    public CSharpType AsNullable() => IsNullable || Kind is CSharpTypeKind.Void or CSharpTypeKind.Variadic ? this : this with { Name = Name + "?" };

    public override string ToString() => Name;
}
