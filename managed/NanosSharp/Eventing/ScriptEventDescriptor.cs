namespace NanosSharp.Eventing;

internal sealed record ScriptEventDescriptor(string ClassName, string EventName, Func<object?[], ScriptEvent> Create);
