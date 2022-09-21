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
    /// <summary>
    /// Used for pushing managed functions and closures to the native stack.
    /// </summary>
    internal const string ManagedFunctionIdentifier = "NanosSharp-ManagedDelegate-1748d9d8-7fb5-43ed-a4d2-fb7031c6b5a0";
    
    internal static readonly string ModulesPath = Path.Combine(Environment.CurrentDirectory, "Modules_NanosSharp");

    private static readonly Type ModuleType = typeof(IModule);
    private static readonly List<LuaVM> CreatedVMs = new();
    
    private delegate void LoadModuleDelegate(IntPtr luaStatePtr, IntPtr namePtr, int nameLength);

    /// <summary>
    /// Gets called by the native runtime to initialize the managed runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    [UnmanagedCallersOnly]
    internal static IntPtr Start(IntPtr luaStatePtr)
    {
        Directory.CreateDirectory(ModulesPath);
        Natives.Initialize();
        
        CreatedVMs.Add(new LuaVM(luaStatePtr));

        return Marshal.GetFunctionPointerForDelegate<LoadModuleDelegate>(LoadModule);
    }

    internal static int ManagedDelegateExecutor(IntPtr luaState)
    {
        ILuaVM lua = GetVM(luaState);

        try
        {
            lua.PushGlobalTable();
            lua.GetField(-1, ManagedFunctionIdentifier);
            int delegateTypeId = (int) lua.ToNumber(-1);
            lua.Pop(2);

            IntPtr delegateHandle = lua.ToUserType(GetUpValueIndex(1, false), delegateTypeId);

            var managedDelegate = (ILuaVM.CFunction) GCHandle.FromIntPtr(delegateHandle).Target!;

            return Math.Max(0, managedDelegate(lua));
        }
        catch (Exception e)
        {
            lua.ClearStack();
            lua.PushString(".NET Exception: " + e);
            return -1;
        }
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
            
            int funcTypeId = vm.NewMetaTable("ManagedFunction");
            vm.PushGlobalTable();
            vm.PushNumber(funcTypeId);
            vm.SetField(-2, ManagedFunctionIdentifier);
            vm.Pop();
        }
        
        return vm;
    }
    
    internal static int GetUpValueIndex(byte upValue, bool managedOffset = true)
    {
        if (managedOffset)
        {
            return -10003 - upValue;
        }

        return -10002 - upValue;
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
}