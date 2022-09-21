using System.Runtime.InteropServices;
using System.Text;
using NanosSharp.Native;

namespace NanosSharp;

/// <summary>
/// The main entry point of NanosSharp.
/// </summary>
internal static class Runtime
{
    private const string ManagedFunctionIdentifier = "NanosSharp-ManagedDelegate-1748d9d8-7fb5-43ed-a4d2-fb7031c6b5a0";
    private delegate void LoadModuleDelegate(IntPtr luaStatePtr, IntPtr namePtr, int nameLength);
    
    private static readonly List<LuaVM> CreatedVMs = new();

    /// <summary>
    /// Gets called by the native runtime to initialize the managed runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    [UnmanagedCallersOnly]
    internal static unsafe IntPtr Start(IntPtr luaStatePtr)
    {
        Natives.Initialize();
        
        CreatedVMs.Add(new LuaVM(luaStatePtr));

        return Marshal.GetFunctionPointerForDelegate<LoadModuleDelegate>(LoadModule);
    }
    
    /// <summary>
    /// Loads the module of the given name into this runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <param name="namePtr">The pointer to the module name.</param>
    /// <param name="nameLength">The length of the module name.</param>
    private static unsafe void LoadModule(IntPtr luaStatePtr, IntPtr namePtr, int nameLength)
    {
        string name = Encoding.UTF8.GetString((byte*) namePtr.ToPointer(), nameLength);
    }
    
    /// <summary>
    /// Returns the managed luaVM from the pointer to the native luaVM. Or if no such luaVM exists, creates a new one.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <returns>The managed luaVM.</returns>
    private static LuaVM GetVM(IntPtr luaStatePtr)
    {
        var vm = CreatedVMs.FirstOrDefault(vm => vm.Handle == luaStatePtr);
        if (vm == null)
        {
            vm = new LuaVM(luaStatePtr);
            CreatedVMs.Add(vm);
        }
        
        return vm;
    }
}