namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is a pointer to a native object.
/// </summary>
internal class ICPointerValue : ICValue
{
    /// <summary>
    /// The value.
    /// </summary>
    internal IntPtr Value
    {
        get
        {
            if (_value == null)
            {
                unsafe
                {
                    _value = Natives.ICValue_GetPointer(Handle);
                }
            }
            
            return _value.Value;
        }
    }
    
    private IntPtr? _value;
    
    internal ICPointerValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICPointerValue"/> class.
    /// </summary>
    internal static ICPointerValue Create(IntPtr ptr)
    {
        unsafe
        {
            return new ICPointerValue(Natives.ICValue_CreatePointer(ptr));
        }
    }
}