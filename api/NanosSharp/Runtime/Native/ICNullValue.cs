namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is null or not existing.
/// </summary>
internal class ICNullValue : ICValue
{
    internal ICNullValue(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ICNullValue"/> class.
    /// </summary>
    internal static ICNullValue Create()
    {
        unsafe
        {
            return new ICNullValue(Natives.ICValue_CreateNull());
        }
    }
}