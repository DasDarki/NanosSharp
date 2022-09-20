namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is a number without a fractional part.
/// </summary>
internal class ICIntegerValue : ICValue
{
    /// <summary>
    /// The value.
    /// </summary>
    internal long Value
    {
        get
        {
            if (_value == null)
            {
                unsafe
                {
                    _value = Natives.ICValue_GetInteger(Handle);
                }
            }
            
            return _value.Value;
        }
    }
    
    private long? _value;
    
    internal ICIntegerValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICIntegerValue"/> class.
    /// </summary>
    internal static ICIntegerValue Create(long l)
    {
        unsafe
        {
            return new ICIntegerValue(Natives.ICValue_CreateInteger(l));
        }
    }
}