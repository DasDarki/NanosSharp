namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is a text.
/// </summary>
internal class ICStringValue : ICValue
{
    /// <summary>
    /// The value.
    /// </summary>
    internal string Value
    {
        get
        {
            if (_value == null)
            {
                unsafe
                {
                    int length = 0;
                    var strPtr = Natives.ICValue_GetString(Handle, &length);
                    _value = Bridge.HGlobalUtf8ToString(strPtr, length);
                    Bridge.FreeHGlobalUtf8FromNative(strPtr);
                }
            }
            
            return _value;
        }
    }
    
    private string? _value;
    
    internal ICStringValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICStringValue"/> class.
    /// </summary>
    internal static ICStringValue Create(string s)
    {
        unsafe
        {
            var strPtr = Bridge.StringToHGlobalUtf8(s);
            var strVal = new ICStringValue(Natives.ICValue_CreateString(strPtr));
            Bridge.FreeHGlobalUtf8FromManaged(strPtr);
            return strVal;
        }
    }
}