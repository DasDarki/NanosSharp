namespace NanosSharp.Runtime.Native;

/// <summary>
/// IC values are values which are managed by the unmanaged c++ side of the runtime. They are created there and the
/// values are retrieved from there. Through this way we can pass values where the types is not known at compile time
/// and without fearing memory leak or corruption.
/// </summary>
internal class ICValue
{
    /// <summary>
    /// The type of the IC value.
    /// </summary>
    internal ICValueType Type
    {
        get
        {
            _type ??= GetICType();
            return _type.Value;
        }
    }
    
    /// <summary>
    /// The pointer to the native instance.
    /// </summary>
    internal IntPtr Handle { get; }
    
    private ICValueType? _type;
    
    internal ICValue(IntPtr handle)
    {
        Handle = handle;
    }

    private ICValueType GetICType()
    {
        unsafe
        {
            return Natives.ICValue_GetType(Handle);
        }
    }

    /// <summary>
    /// Destroys the unmanaged IC value and frees the memory.
    /// </summary>
    internal void Destroy()
    {
        unsafe
        {
            Natives.ICValue_Destroy(Handle);
        }
    }
}