namespace NanosSharp.Injection;

/// <summary>Injects a registered service into the marked field or property when the package starts.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
{
    /// <summary>When true a missing service fails the start, otherwise the member is left untouched.</summary>
    public bool IsRequired { get; set; } = true;
}
