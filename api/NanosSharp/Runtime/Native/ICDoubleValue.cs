namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is a number with a fractional part.
/// </summary>
internal class ICDoubleValue : ICValue
{
    /// <summary>
    /// The value.
    /// </summary>
    internal double Value
    {
        get
        {
            if (_value == null)
            {
                unsafe
                {
                    _value = Natives.ICValue_GetDouble(Handle);
                }
            }
            
            return _value.Value;
        }
    }
    
    private double? _value;
    
    internal ICDoubleValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICDoubleValue"/> class.
    /// </summary>
    internal static ICDoubleValue Create(double d)
    {
        unsafe
        {
            return new ICDoubleValue(Natives.ICValue_CreateDouble(d));
        }
    }
}