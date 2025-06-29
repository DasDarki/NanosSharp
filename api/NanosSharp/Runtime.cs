using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using NanosSharp.API;
using Serilog;
using Serilog.Core;

namespace NanosSharp;

/// <summary>
/// The main entry point of NanosSharp.
/// </summary>
internal static class Runtime
{
    internal static readonly string ModulesPath = Path.Combine(Environment.CurrentDirectory, "Modules_NanosSharp");

    private static readonly Type ModuleType = typeof(IModule);
    private static readonly List<LuaVM> CreatedVMs = [];
    private static readonly ConcurrentDictionary<string, List<IModule>> LoadedModules = new();
    private static readonly ConcurrentDictionary<string, ILuaVM> ConnectedVMs = new();
#if DEBUG
    private static readonly Logger Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Verbose().CreateLogger();
#else
    private static readonly Logger Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();
#endif
    
    private delegate void LoadModuleDelegate(IntPtr luaStatePtr, IntPtr namePtr, int nameLength, IntPtr envPtr);

    /// <summary>
    /// Gets called by the native runtime to initialize the managed runtime.
    /// </summary>
    /// <param name="luaStatePtr">The pointer to the native luaVM.</param>
    /// <param name="callManagedDelegatePtr">The pointer to the managed delegate caller.</param>
    [UnmanagedCallersOnly]
    internal static unsafe IntPtr Start(IntPtr luaStatePtr, IntPtr* callManagedDelegatePtr)
    {
        Logger.Information("[NanosSharp] Starting NanosSharp...");
        Logger.Verbose("[NanosSharp] Modules Path: {ModulesPath}", ModulesPath);
        Logger.Verbose("[NanosSharp] Autoload Path: {AutoloadPath}", Path.Combine(ModulesPath, "@autoload"));
        
        Directory.CreateDirectory(ModulesPath);
        Directory.CreateDirectory(Path.Combine(ModulesPath, "@autoload"));
        
        Logger.Information("[NanosSharp] Initializing NanosSharp natives...");
        Natives.Initialize();
        Logger.Information("[NanosSharp] NanosSharp natives found and bound.");
        
        var vm = new LuaVM(luaStatePtr);
        CreatedVMs.Add(vm);
        
        *callManagedDelegatePtr = (IntPtr) (delegate* unmanaged<IntPtr, IntPtr, int>) &CallManagedDelegate;
        
        Logger.Information("[NanosSharp] NanosSharp started.");

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
    /// <param name="envPtr">The pointer to the environment table.</param>
    private static unsafe void LoadModule(IntPtr luaStatePtr, IntPtr namePtr, int nameLength, IntPtr envPtr)
    {
        try
        {
            Logger.Information("[NanosSharp] Loading module...");
            
            var modName = Encoding.UTF8.GetString((byte*) namePtr.ToPointer(), nameLength);
            var vm = GetVM(luaStatePtr);
            
            Logger.Verbose("[NanosSharp] Module Name: {ModuleName}", modName);

            if (modName == "@autoload")
            {
                Logger.Verbose("[NanosSharp] Loading all modules in autoload...");
                
                foreach (var dir in Directory.GetDirectories(Path.Combine(ModulesPath, "@autoload")))
                {
                    Logger.Verbose("[NanosSharp] Loading module in directory: {Dir}", dir);
                    
                    var dirName = Path.GetFileName(dir);
                    var path = Path.Combine(dir, dirName + ".dll");
                    if (!File.Exists(path))
                    {
                        Logger.Error("[NanosSharp] Managed DLL not found at path: {Path}", path);
                        continue;
                    }
                    
                    LoadForVM(path);
                }
                
                foreach (var file in Directory.GetFiles(Path.Combine(ModulesPath, "@autoload"), "*.ref"))
                {
                    Logger.Verbose("[NanosSharp] Loading module from reference file: {File}", file);
                    
                    var path = File.ReadAllText(file);
                    if (!File.Exists(path))
                    {
                        Logger.Error("[NanosSharp] Managed DLL not found at path: {Path}", path);
                        continue;
                    }
                    
                    LoadForVM(path);
                }
            }
            else
            {
                Logger.Verbose("[NanosSharp] Loading module at explicit path...");
                if (envPtr == IntPtr.Zero)
                {
                    Logger.Warning("[NanosSharp] Lua Env is null, pushing C#-managed functions are not supported in this module.");
                }
                else
                {
                    vm.LuaEnv = envPtr;
                }
                
                var path = Path.Combine(ModulesPath, modName, modName + ".dll");
                if (!File.Exists(path))
                {
                    var refPath = Path.Combine(ModulesPath, modName + ".ref");
                    if (!File.Exists(refPath))
                    {
                        Logger.Error("[NanosSharp] Managed DLL not found at path: {Path}", path);
                        return;
                    }
                    
                    path = File.ReadAllText(refPath);
                    if (!File.Exists(path))
                    {
                        Logger.Error("[NanosSharp] Managed DLL not found at path: {Path}", path);
                        return;
                    }
                }
                
                LoadForVM(path);
            }

            void LoadForVM(string path)
            {
                Logger.Verbose("[NanosSharp] Loading module for VM at path: {Path}", path);
                
                if (LoadedModules.TryGetValue(path, out var moduleList))
                {
                    Logger.Verbose("[NanosSharp] Module already loaded, initializing...");
                    
                    if (ConnectedVMs.TryGetValue(path, out var connectedVM) && connectedVM == vm)
                    {
                        Logger.Warning("[NanosSharp] Module already connected to this VM. Skipping...");
                        return;
                    }
                    
                    Logger.Information("[NanosSharp] Initializing loaded module...");
                    
                    foreach (var module in moduleList)
                    {
                        module.Initialize(vm);
                    }
                    
                    ConnectedVMs[path] = vm;
                    
                    Logger.Information("[NanosSharp] Module initialized.");
                }
                else
                {
                    Logger.Verbose("[NanosSharp] Module not loaded, loading...");
                    
                    var dirPath = Path.GetDirectoryName(path);
                    var context = new AssemblyLoadContext(null, true);
                    context.Resolving += (_, assemblyName) =>
                    {
                        Logger.Verbose("[NanosSharp] Resolving assembly: {AssemblyName} for path: {Path}", assemblyName, path);
                        
                        try
                        {
                            Logger.Verbose("[NanosSharp] Attempting to load assembly from context...");
                            return Assembly.Load(assemblyName);
                        }
                        catch (Exception ex)
                        {
                            Logger.Verbose("[NanosSharp] Failed to load assembly from context: {Error}", ex.Message);
                            
                            var dllPath = Path.Combine(dirPath ?? string.Empty, assemblyName.Name + ".dll");
                            Logger.Verbose("[NanosSharp] Attempting to load assembly from file: {DllPath}", dllPath);
                            if (File.Exists(dllPath))
                            {
                                using var stream = new MemoryStream(File.ReadAllBytes(dllPath));
                                return context.LoadFromStream(stream);
                            }

                            Logger.Error("[NanosSharp] Assembly not found in path {DllPath} nor in context.", dllPath);
                        }
                        
                        return null;
                    };
                    var assembly = LoadAssemblyInMemory(path, context);
                    
                    Logger.Verbose("[NanosSharp] Assembly loaded, searching for modules...");
                    
                    var newModuleList = new List<IModule>();
            
                    foreach (var type in assembly.GetTypes())
                    {
                        if (ModuleType.IsAssignableFrom(type) && !type.IsAbstract)
                        {
                            if (Activator.CreateInstance(type) is not IModule module)
                            {
                                continue;
                            }
                            
                            module.Initialize(vm);
                            
                            newModuleList.Add(module);
                        }
                    }
                    
                    if (newModuleList.Count > 0)
                    {
                        LoadedModules[path] = newModuleList;
                    }
                    
                    ConnectedVMs[path] = vm;
                    
                    Logger.Information("[NanosSharp] Module loaded and initialized. Found {ModuleCount} modules.", newModuleList.Count);
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
        return context.LoadFromAssemblyPath(path);
    }

    [UnmanagedCallersOnly]
    private static int CallManagedDelegate(IntPtr luaStatePtr, IntPtr gcHandlePtr)
    {
        var gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
        if (gcHandle.Target is ILuaVM.CFunction func)
        {
            ILuaVM vm = GetVM(luaStatePtr);
            return func(vm);
        }

        return 0;
    }
}