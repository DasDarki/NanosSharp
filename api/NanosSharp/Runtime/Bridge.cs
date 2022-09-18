using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NanosSharp.Runtime;

/// <summary>
/// The bridge manages the communication between the Nanos# runtime and nanos world server.
/// </summary>
[SuppressUnmanagedCodeSecurity]
internal static class Bridge
{
    /// <summary>
    /// The path to the current working directory which is the folder where the executable is located.
    /// </summary>
    internal static string CurrentWorkingDir { get; private set; } = "";
    
    /// <summary>
    /// The main thread is for assertion purposes.
    /// </summary>
    private static int _mainThreadId;

    /// <summary>
    /// Initializes the bridge and passes some arguments to the managed side.
    /// </summary>
    [UnmanagedCallersOnly]
    internal static void Initialize()
    {
        /*Console.WriteLine("[Nanos#] Binding natives...");
        //Natives.Initialize();
        
        Console.WriteLine("[Nanos#] Initializing bridge...");
        _mainThreadId = Environment.CurrentManagedThreadId;
        Console.WriteLine("[Nanos#] Bridge initialized.");*/
    }
    
    /// <summary>
    /// Checks if the current invocation thread is the main thread or not.
    /// </summary>
    /// <exception cref="MethodAccessException">If the current invocation thread is not the main thread</exception>
    internal static void AssertMainThreadInvocation()
    {
        try
        {
            if (Environment.CurrentManagedThreadId != _mainThreadId)
                throw new MethodAccessException(
                    "The current method must be invoked from the main thread, use {currently not added} to enforce main thread invocation.");
        }
        catch (Exception ex)
        {
            //TODO: Logger.Error(ex, "Main Thread Invocation Assertion:");
        }
    }

    /// <summary>
    /// Converts the managed string into a native string to make it passable to the native side.
    /// </summary>
    /// <param name="str">The string to be converted.</param>
    /// <returns>The pointer pointing to the native string.</returns>
    /// <remarks>Credits: https://github.com/Microsoft/xbox-live-unity-plugin/blob/master/CSharpSource/Source/api/System/MarshallingHelpers.cs</remarks>
    internal static IntPtr StringToHGlobalUtf8(string? str)
    {
        if (str == null)
        {
            return IntPtr.Zero;
        }
        
        var bytes = Encoding.UTF8.GetBytes(str);
        var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr, bytes.Length, 0);
        
        return ptr;
    }

    /// <summary>
    /// Converts the given native string into a managed string.
    /// </summary>
    /// <param name="str">The pointer to the native string.</param>
    /// <param name="size">The size of the native string.</param>
    /// <returns></returns>
    internal static string HGlobalUtf8ToString(IntPtr str, int size)
    {
        if (str == IntPtr.Zero)
        {
            return string.Empty;
        }

        unsafe
        {
            var stringResult = Marshal.PtrToStringUTF8(str, size);
            Natives.FreeString(str);
            return stringResult;
        }
    }
}