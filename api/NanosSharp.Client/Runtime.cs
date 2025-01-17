using System.Reflection;
using System.Runtime.InteropServices;
using NanosSharp.Client.API;
using Serilog;

namespace NanosSharp.Client;

/// <summary>
/// The runtime is the entry point of the NanosSharp client. From here the local WebSocket server is started and
/// the client is being initialized.
/// </summary>
internal static class Runtime
{
    /// <summary>
    /// The logger of the NanosSharp client.
    /// </summary>
    internal static ILogger Logger { get; } = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console()
        .CreateLogger();
    
    /// <summary>
    /// The runtime configuration of the NanosSharp client.
    /// </summary>
    internal static RuntimeConfig Config { get; private set; } = null!;
    
    /// <summary>
    /// The package loader instance of this runtime.
    /// </summary>
    internal static PackageLoader PackageLoader { get; } = new();
    
    /// <summary>
    /// Gets called from the unmanaged side to start the clients runtime.
    /// </summary>
    internal static void Main()
    {
        try
        {
            Config = RuntimeConfig.Load();
            Console.Title = "NanosSharp Client v" + (typeof(Runtime).Assembly.GetName().Version?.ToString() ?? "UNKNOWN");

            if (!Config.IsDebug)
            {
                ShowWindow(GetConsoleWindow(), 0);
            }
        
            Logger.Information("Starting NanosSharp WebSocket Bridge server...");
            
            try
            {
                BridgeServer.Instance.Start();
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "An error occurred while starting the WebSocket server.");
            }
        
            Logger.Verbose("Starting NanosSharp client runtime...");
            Logger.Information("NanosSharp client runtime started.");

            BridgeServer.Instance.NanosEventReceived += (s, array) =>
            {
                if (s == "@bridge:package:load")
                {
                    var path = array[0]!.GetValue<string>();
                    if (path.StartsWith(@"\\?\"))
                    {
                        path = path[4..];
                    }
                    
                    var name = Path.GetFileName(Path.GetDirectoryName(path.Replace("Client\\", ""))) ?? "Unknown" + Guid.NewGuid().ToString("N");
                    var assembly = PackageLoader.CompilePackage(name, path);
                    
                    if (assembly != null)
                    {
                        var type = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(Package)));
                        if (type != null)
                        {
                            var instance = (Package) Activator.CreateInstance(type)!;
                            var nativesProperty = type.GetProperty("Natives", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (nativesProperty != null)
                            {
                                nativesProperty.SetValue(instance, new Natives());
                            }
                            
                            instance.OnStart();
                            
                            Logger.Information("Loaded package {Name} from {Path}", name, path);
                        }
                    }
                }
            };
            
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "An error occurred while starting the NanosSharp client runtime.");
        }
    }
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}