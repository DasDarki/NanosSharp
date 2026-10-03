namespace NanosSharp.Interop.Reflection;

internal sealed class StaticMethodSlot(string className, string name)
{
    private NativeMethod? _method;
    private int _generation = -1;

    public NativeMethod Resolve()
    {
        if (_method is not null && _generation == NativeClassRegistry.Generation)
        {
            return _method;
        }

        _method = NativeClassRegistry.Get(className).GetStaticMethod(name);
        _generation = NativeClassRegistry.Generation;
        return _method;
    }
}
