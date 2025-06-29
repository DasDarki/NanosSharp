using System.Reflection;
using System.Runtime.Loader;
using NanosSharp.API;
using IOFile = System.IO.File;

namespace NanosSharp.Server.Packages;

/// <summary>
/// The package loader is responsible for loading packages into the NanosSharp server. The loader uses a specific order
/// in which it loads the packages, ensuring that dependencies are resolved correctly:<br/>
/// 1. Check if there is a DLL with the exact name of the package in the working directory.<br/>
/// 2. If not, gather all *.cs files in the working directory and compile them into a DLL.<br/>
/// <br/>
/// Resolving libraries is done through the <see cref="AssemblyLoadContext"/>. The loader will create a new load context
/// for each package and override the <see cref="AssemblyLoadContext.Resolving"/> event to resolve the package's
/// dependencies. This allows the package to load its dependencies from the same directory as the package itself,
/// which is useful for packages that have their own dependencies that are not part of the NanosSharp server's
/// default set of assemblies. The resolving will run in a defined order, which is:<br/>
/// 1. Check if the assembly is already loaded in the current application domain.<br/>
/// 2. If not, check if the DLLs exist in the package's working directory (1st priority) or in the NanosSharp server's
/// cache directory (2nd priority).<br/>
/// 3. If neither of the above is true and the csproj file exists, we grab the references from there and load them from
/// NuGet and cache them in the NanosSharp server's cache directory.<br/>
/// 4. If this is still not successful, we will fetch the lastest stable version of the assembly from NuGet and
/// cache it in the NanosSharp server's cache directory.<br/>
/// 5. If this is still not successful, we will throw an exception.
/// </summary>
internal static class PackageLoader
{
    /// <summary>
    /// Loads a package from the specified path and returns the assembly and its load context.
    /// </summary>
    /// <param name="packagePath">The path to the working directory of the package.</param>
    /// <param name="debug">Whether to load the package in debug mode. This will enable debugging symbols and allow for debugging the package.</param>
    /// <returns>The loaded assembly and its load context. If the package could not be loaded, returns null.</returns>
    public static (Assembly Assembly, AssemblyLoadContext Context)? Load(string packagePath, bool debug = false)
    {
        var packageName = Path.GetFileName(packagePath);
        
        // 1.
        var dllPath = Path.Combine(packagePath, $"{packageName}.dll");
        if (IOFile.Exists(dllPath))
        {
            var context = CreateContext(packagePath);
            using var stream = new MemoryStream(IOFile.ReadAllBytes(dllPath));
            var asm = context.LoadFromStream(stream);
            return (asm, context);
        }
        
        // 2.
        var csFiles = Directory.GetFiles(packagePath, "*.cs", SearchOption.AllDirectories);
        if (csFiles.Length > 0)
        {
            var context = CreateContext(packagePath);
            var asm = PackageCompiler.Compile(context, packagePath, packageName, csFiles, debug);
            if (asm != null)
            {
                return (asm, context);
            }
        }

        return null;
    }
    
    private static AssemblyLoadContext CreateContext(string packagePath)
    {
        var context = new AssemblyLoadContext(packagePath, isCollectible: true);
        context.Resolving += (_, assemblyName) =>
        {
            if (string.IsNullOrEmpty(assemblyName.Name))
            {
                return null;
            }
            
            if (assemblyName.Name == "NanosSharp.Server")
            {
                return typeof(Package).Assembly;
            }

            if (assemblyName.Name == "NanosSharp.API")
            {
                return typeof(LuaRef).Assembly;
            }
            
            var path = PackageLibraryResolver.Resolve(packagePath, assemblyName.Name, assemblyName.Version?.ToString());
            if (path != null)
            {
                return AppDomain.CurrentDomain.Load(IOFile.ReadAllBytes(path));
            }
            
            return null;
        };
        return context;
    }
}