using System.Reflection;

namespace NanosSharp.Eventing;

internal sealed record EventHandlerRegistration(object? Listener, MethodInfo Method, ServerEventAttribute Options);
