namespace NanosSharp.Eventing;

/// <summary>Marks a method with a single typed event parameter as handler, it is registered automatically.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerEventAttribute : Attribute
{
    /// <summary>Handlers with a lower priority run first.</summary>
    public int Priority { get; init; }

    /// <summary>Whether the handler still runs after an earlier handler cancelled the event.</summary>
    public bool ReceiveCancelled { get; init; }
}
