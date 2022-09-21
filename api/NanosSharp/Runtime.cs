using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using NanosSharp.API;

namespace NanosSharp;

/// <summary>
/// The main entry point of NanosSharp.
/// </summary>
internal static class Runtime
{
    internal static readonly string ModulesPath = Path.Combine(Environment.CurrentDirectory, "Modules_NanosSharp");

    private static readonly Type ModuleType = typeof(IModule);
    private static readonly List<LuaVM> CreatedVMs = new();
    
    private delegate void LoadModuleDelegate(IntPtr luaStatePtr, IntPtr namePtr, int nameLength);

    /// <summary>
    /// Gets called by the native runtime to initialize the managed runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <param name="callManagedDelegatePtr">The pointer to the managed delegate caller.</param>
    [UnmanagedCallersOnly]
    internal static unsafe IntPtr Start(IntPtr luaStatePtr, IntPtr* callManagedDelegatePtr)
    {
        Directory.CreateDirectory(ModulesPath);
        Natives.Initialize();
        
        CreatedVMs.Add(new LuaVM(luaStatePtr));
        
        *callManagedDelegatePtr = (IntPtr) (delegate* unmanaged<IntPtr, IntPtr, int>) &CallManagedDelegate;

        return Marshal.GetFunctionPointerForDelegate<LoadModuleDelegate>(LoadModule);
    }
    
    /// <summary>
    /// Returns the managed luaVM from the pointer to the native luaVM. Or if no such luaVM exists, creates a new one.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <returns>The managed luaVM.</returns>
    internal static LuaVM GetVM(IntPtr luaStatePtr)
    {
        var vm = CreatedVMs.FirstOrDefault(vm => vm.Handle == luaStatePtr);
        if (vm == null)
        {
            vm = new LuaVM(luaStatePtr);
            CreatedVMs.Add(vm);
        }
        
        return vm;
    }
    
    /// <summary>
    /// Loads the module of the given name into this runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <param name="namePtr">The pointer to the module name.</param>
    /// <param name="nameLength">The length of the module name.</param>
    private static unsafe void LoadModule(IntPtr luaStatePtr, IntPtr namePtr, int nameLength)
    {
        try
        {
            string name = Encoding.UTF8.GetString((byte*) namePtr.ToPointer(), nameLength);
            ILuaVM vm = GetVM(luaStatePtr);

            string path = Path.Combine(ModulesPath, name, name + ".dll");
            if (!File.Exists(path))
            {
                //TODO error
                return;
            }

            AssemblyLoadContext context = new AssemblyLoadContext(null, true);
            context.Resolving += (_, assemblyName) => Assembly.Load(assemblyName);
            Assembly assembly = LoadAssemblyInMemory(path, context);
            
            foreach (var type in assembly.GetExportedTypes())
            {
                if (ModuleType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    if (Activator.CreateInstance(type) is not IModule module) continue;
                    module.Initialize(vm);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private static Assembly LoadAssemblyInMemory(string path, AssemblyLoadContext context)
    {
        using MemoryStream stream = new MemoryStream(File.ReadAllBytes(path));
        return context.LoadFromStream(stream);
    }

    [UnmanagedCallersOnly]
    private static int CallManagedDelegate(IntPtr luaStatePtr, IntPtr gcHandlePtr)
    {
        GCHandle gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
        if (gcHandle.Target is ILuaVM.CFunction func)
        {
            ILuaVM vm = GetVM(luaStatePtr);
            return func(vm);
        }

        return 0;
    }
}