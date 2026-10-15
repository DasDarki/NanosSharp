namespace NanosSharp.Entities.Binding;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class EventHookAttribute(string eventName) : Attribute
{
    public string EventName { get; } = eventName;
}
