using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using NanosSharp.Runtime.Native;

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
        Console.WriteLine("[Nanos#] Binding natives...");
        try
        {
            Natives.Initialize();
        } 
        catch (Exception e)
        {
            Console.WriteLine("[Nanos#] Failed to bind natives: " + e.Message + "\n" + e.StackTrace);
        }
        
        Console.WriteLine("[Nanos#] Initializing bridge...");
        _mainThreadId = Environment.CurrentManagedThreadId;
        Console.WriteLine("[Nanos#] Bridge initialized.");
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
    
    /// <summary>
    /// Frees the memory of the string allocated by the managed side.
    /// </summary>
    /// <param name="str">The pointer to the managed string.</param>
    internal static void FreeHGlobalUtf8FromManaged(IntPtr str)
    {
        Marshal.FreeHGlobal(str);
    }
    
    /// <summary>
    /// Frees the native string from the given native pointer.
    /// </summary>
    /// <param name="str">The pointer to the native string.</param>
    internal static void FreeHGlobalUtf8FromNative(IntPtr str)
    {
        unsafe
        {
            Natives.FreeString(str);
        }
    }

    /// <summary>
    /// Logs as script print message to the console.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    internal static void ScriptLog(string message)
    {
        AssertMainThreadInvocation();
        
        unsafe
        {
            var ptr = StringToHGlobalUtf8(message);
            Natives.ScriptLog(ptr);
            FreeHGlobalUtf8FromManaged(ptr);
        }
    }

    /// <summary>
    /// Logs as error print message to the console.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    internal static void ErrorLog(string message)
    {
        AssertMainThreadInvocation();
        
        unsafe
        {
            var ptr = StringToHGlobalUtf8(message);
            Natives.ErrorLog(ptr);
            FreeHGlobalUtf8FromManaged(ptr);
        }
    }
}