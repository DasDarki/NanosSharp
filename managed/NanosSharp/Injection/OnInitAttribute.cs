namespace NanosSharp.Injection;

/// <summary>Marks a method to run after the instance was created and its injections resolved; it may be async and take a cancellation token.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class OnInitAttribute : Attribute
{
}
