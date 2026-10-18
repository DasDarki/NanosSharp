namespace NanosSharp.Eventing;

internal static partial class ScriptEventCatalog
{
    private static readonly Dictionary<Type, ScriptEventDescriptor> Descriptors = CreateDescriptors();

    public static ScriptEventDescriptor? Find(Type eventType) => Descriptors.GetValueOrDefault(eventType);

    private static Dictionary<Type, ScriptEventDescriptor> CreateDescriptors()
    {
        var descriptors = new Dictionary<Type, ScriptEventDescriptor>();
        AddSharedDescriptors(descriptors);
        AddSideDescriptors(descriptors);
        return descriptors;
    }

    static partial void AddSharedDescriptors(Dictionary<Type, ScriptEventDescriptor> descriptors);

    static partial void AddSideDescriptors(Dictionary<Type, ScriptEventDescriptor> descriptors);
}
