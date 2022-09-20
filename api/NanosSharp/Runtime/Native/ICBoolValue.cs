namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is either true or false.
/// </summary>
internal class ICBoolValue : ICValue
{
    /// <summary>
    /// The value.
    /// </summary>
    internal bool Value
    {
        get
        {
            if (_value == null)
            {
                unsafe
                {
                    _value = Natives.ICValue_GetBoolean(Handle);
                }
            }
            
            return _value.Value;
        }
    }
    
    private bool? _value;
    
    internal ICBoolValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICBoolValue"/> class.
    /// </summary>
    internal static ICBoolValue Create(bool b)
    {
        unsafe
        {
            return new ICBoolValue(Natives.ICValue_CreateBool(b));
        }
    }
}