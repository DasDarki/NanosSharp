//
// Created by DasDarki on 18.09.2022.
//

#ifndef NANOSSHARP_RUNTIME_RUNTIME_H
#define NANOSSHARP_RUNTIME_RUNTIME_H

#include <filesystem>
#include <string>
#include <stdint.h>

#include <coreclr_delegates.h>
#include <hostfxr.h>

#if _MSC_VER
#include <Windows.h>
#endif

#define RUNTIME_SUCCESS 0
#define RUNTIME_ERROR_HOSTFXR_FAILED 1
#define RUNTIME_ERROR_NO_RUNTIME_CONFIG 2
#define RUNTIME_ERROR_NO_RUNTIME_LIBRARY 3
#define RUNTIME_ERROR_ASSEMBLY_FAILED 4
#define RUNTIME_ERROR_INIT_FAILED 5

#ifdef _MSC_VER
#define DLL_HANDLE HMODULE
#define DLL_LOAD LoadLibraryA
#define DLL_UNLOAD FreeLibrary
#define DLL_GETFUNCTION GetProcAddress
#define HOSTFXR_FILENAME "hostfxr.dll"
#else
#define DLL_HANDLE void*
#define DLL_LOAD(p) dlopen(p, RTLD_LAZY)
#define DLL_UNLOAD dlclose
#define DLL_GETFUNCTION dlsym
#define HOSTFXR_FILENAME "libhostfxr.so"
#endif

#if defined(_WIN32)
#define CORECLR_DELEGATE_CALLTYPE __stdcall
#else
#define CORECLR_DELEGATE_CALLTYPE
#endif

#define TO_WCHAR(s) std::wstring(s.begin(), s.end()).c_str()

typedef void (*LoadModule_fn)(void*,void*,int32_t);
typedef int (*CallManagedDelegate_fn)(void*,void*);
typedef LoadModule_fn (CORECLR_DELEGATE_CALLTYPE* StartRuntime_fn)(void*, CallManagedDelegate_fn*);

LoadModule_fn load_module = nullptr;
CallManagedDelegate_fn call_managed_delegate = nullptr;

class Runtime {

private:
    DLL_HANDLE m_HostFxrPtr = nullptr;
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    hostfxr_initialize_for_runtime_config_fn init_fptr = nullptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
    hostfxr_close_fn close_fptr = nullptr;

    std::string m_ApiDllPath;
    std::string m_FriendlyName;
    bool m_Loaded = false;

private:
    bool load_host_fxr(const std::filesystem::path &dotnet_path, const std::string &version) {
        const std::filesystem::path host_fxr_path = dotnet_path / "host" / "fxr" / version / HOSTFXR_FILENAME;
        m_HostFxrPtr = DLL_LOAD(host_fxr_path.string().c_str());

        if (!m_HostFxrPtr) {
            return false;
        }

        init_fptr           = (hostfxr_initialize_for_runtime_config_fn)  DLL_GETFUNCTION(m_HostFxrPtr, "hostfxr_initialize_for_runtime_config");
        get_delegate_fptr   = (hostfxr_get_runtime_delegate_fn)           DLL_GETFUNCTION(m_HostFxrPtr, "hostfxr_get_runtime_delegate");
        close_fptr          = (hostfxr_close_fn)                          DLL_GETFUNCTION(m_HostFxrPtr, "hostfxr_close");

        return (init_fptr && get_delegate_fptr && close_fptr);
    }

    void unload_host_fxr() {
        if (m_HostFxrPtr) {
            DLL_UNLOAD(m_HostFxrPtr);
        }
    }

    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const std::filesystem::path &config_path) {
        void *ptr = nullptr;
        hostfxr_handle ctx = nullptr;

        int result;

        const std::string config_path_str = config_path.string();
        result = init_fptr(TO_WCHAR(config_path_str), nullptr, &ctx);
        if (result != 0 || !ctx) {
            close_fptr(ctx);
            return nullptr;
        }

        result = get_delegate_fptr(ctx, hdt_load_assembly_and_get_function_pointer, &ptr);
        if (result != 0 || !ptr) {
            close_fptr(ctx);
            return nullptr;
        }

        close_fptr(ctx);
        return (load_assembly_and_get_function_pointer_fn) ptr;
    }

public:
    /**
     * Returns a singleton instance of the runtime or creates a new one.
     */
    static Runtime GetInstance() {
        static Runtime instance;
        return instance;
    }

    /**
     * Initializes the runtime components and starts the whole runtime.
     *
     * @param L             The lua state for this runtime.
     * @param version       The version of the .NET runtime to use.
     * @param dll_name      The name of the .NET assembly to load without the .dll extension.
     * @param friendly_name The friendly name of the .NET assembly to load.
     */
    int Start(void *L, const std::string &version, const std::string &dll_name, const std::string &friendly_name) {
        if (m_Loaded) {
            return RUNTIME_SUCCESS;
        }

        const std::filesystem::path dotnet_path = std::filesystem::current_path() / "dotnet";
        if (!load_host_fxr(dotnet_path, version)) {
            return RUNTIME_ERROR_HOSTFXR_FAILED;
        }

        const std::filesystem::path dotnet_api_path = dotnet_path / "api";
        const std::filesystem::path dotnet_api_dll = dotnet_api_path / (dll_name + ".dll");
        if (!std::filesystem::exists(dotnet_api_dll)) {
            return RUNTIME_ERROR_NO_RUNTIME_LIBRARY;
        }

        const std::filesystem::path dotnet_api_config = dotnet_api_path / (dll_name + ".runtimeconfig.json");
        if (!std::filesystem::exists(dotnet_api_config)) {
            return RUNTIME_ERROR_NO_RUNTIME_CONFIG;
        }

        load_assembly_and_get_function_pointer = get_dotnet_load_assembly(dotnet_api_config);
        if (!load_assembly_and_get_function_pointer) {
            return RUNTIME_ERROR_ASSEMBLY_FAILED;
        }

        m_ApiDllPath = dotnet_api_dll.string();
        m_FriendlyName = friendly_name;

        const std::string &func_name = "Start";

        StartRuntime_fn start = nullptr;
        int rc = load_assembly_and_get_function_pointer(
                TO_WCHAR(m_ApiDllPath),
                TO_WCHAR(m_FriendlyName),
                TO_WCHAR(func_name),
                UNMANAGEDCALLERSONLY_METHOD,
                nullptr,
                (void**)&start);

        if (rc != 0 || !start) {
            return RUNTIME_ERROR_INIT_FAILED;
        }

        load_module = start(L, &call_managed_delegate);

        m_Loaded = true;

        return RUNTIME_SUCCESS;
    }

    /**
     * Loads a module into the runtime.
     *
     * @param L     The state for which the module should be loaded.
     * @param name  The name of the module.
     */
    void LoadModule(void *L, const char *name) {
        if (load_module) {
            load_module(L, (void*) name, strlen(name));
        }
    }
};

#endif //NANOSSHARP_RUNTIME_RUNTIME_H
