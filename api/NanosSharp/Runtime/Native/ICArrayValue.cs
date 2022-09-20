using System.Collections;

namespace NanosSharp.Runtime.Native;

/// <summary>
/// The value is an array without a fixed size.
/// </summary>
internal class ICArrayValue : ICValue, IEnumerable<ICValue>
{
    /// <summary>
    /// The current amount of items in the array.
    /// </summary>
    internal uint Length
    {
        get
        {
            unsafe
            {
                return Natives.ICValue_GetArraySize(Handle);
            }
        }
    }
    
    internal ICArrayValue(IntPtr handle) : base(handle)
    {
    }
    
    /// <summary>
    /// Adds the given value to the array.
    /// </summary>
    /// <param name="value">The value to be added.</param>
    internal void Add(ICValue value)
    {
        unsafe
        {
            Natives.ICValue_AddArrayElement(Handle, value.Handle);
        }
    }

    /// <summary>
    /// Returns the value at the given index.
    /// </summary>
    internal ICValue Get(uint idx)
    {
        unsafe
        {
            return new ICValue(Natives.ICValue_GetArrayElement(Handle, idx));
        }
    }

    /// <summary>
    /// The enumerator for the array.
    /// </summary>
    public IEnumerator<ICValue> GetEnumerator()
    {
        for (uint i = 0; i < Length; i++)
        {
            yield return Get(i);
        }
    }

    /// <summary>
    /// Returns the enumerator for the array.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="ICArrayValue"/> class.
    /// </summary>
    internal static ICArrayValue Create()
    {
        unsafe
        {
            return new ICArrayValue(Natives.ICValue_CreateArray());
        }
    }
}