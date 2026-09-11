using System.Runtime.InteropServices;

namespace NanosSharp.Interop.Values;

internal sealed unsafe class NativeValueList : IDisposable
{
    public int Count { get; }

    public nint* Pointer { get; private set; }

    public NativeValueList(IEnumerable<object?> values)
    {
        var handles = new List<nint>();
        try
        {
            foreach (var value in values)
            {
                handles.Add(ValueMarshaller.ToNative(value));
            }
        }
        catch
        {
            Free(handles);
            throw;
        }

        Count = handles.Count;
        Pointer = (nint*)NativeMemory.Alloc((nuint)Math.Max(Count, 1), (nuint)sizeof(nint));
        handles.CopyTo(new Span<nint>(Pointer, Count));
    }

    public NativeValueList(ReadOnlySpan<object?> values) : this((IEnumerable<object?>)values.ToArray())
    {
    }

    public void Dispose()
    {
        if (Pointer == null)
        {
            return;
        }

        Free(new ReadOnlySpan<nint>(Pointer, Count));
        NativeMemory.Free(Pointer);
        Pointer = null;
    }

    private static void Free(IEnumerable<nint> handles)
    {
        foreach (var handle in handles)
        {
            NativeRuntime.Api.ValueFree(handle);
        }
    }

    private static void Free(ReadOnlySpan<nint> handles)
    {
        foreach (var handle in handles)
        {
            NativeRuntime.Api.ValueFree(handle);
        }
    }
}
