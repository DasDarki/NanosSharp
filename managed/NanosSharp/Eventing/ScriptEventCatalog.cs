namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    private static readonly Dictionary<Type, ScriptEventDescriptor> Descriptors = CreateDescriptors();

    public static ScriptEventDescriptor? Find(Type eventType) => Descriptors.GetValueOrDefault(eventType);

    private static partial Dictionary<Type, ScriptEventDescriptor> CreateDescriptors();
}
