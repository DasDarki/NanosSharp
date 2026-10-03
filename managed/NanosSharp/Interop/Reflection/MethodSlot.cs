namespace NanosSharp.Interop.Reflection;

internal sealed class MethodSlot(string name)
{
    private static int _nextId;

    public int Id { get; } = Interlocked.Increment(ref _nextId) - 1;

    public string Name { get; } = name;

    public static int Count => Volatile.Read(ref _nextId);
}
