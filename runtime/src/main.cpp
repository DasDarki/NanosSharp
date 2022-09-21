#include "runtime.h"

#ifdef __cplusplus
#define EXTERN extern "C"
#else
#define EXTERN
#endif

#ifdef _WIN32
#define EXPORT EXTERN __declspec(dllexport)
#define IMPORT EXTERN __declspec(dllimport)
#else
#define EXPORT EXTERN __attribute__((visibility("default")))
#define IMPORT
#endif

EXTERN {
    #include "lua.h"
    #include "lauxlib.h"
}

int load_module_internally(lua_State *L) {
    const char *module_name = luaL_checkstring(L, 1);
    Runtime::GetInstance().LoadModule(L, module_name);
    return 0;
}

EXTERN int luaopen_nanossharp_runtime (lua_State *L) {
    int result = Runtime::GetInstance().Start(L, "6.0.6", "NanosSharp", "NanosSharp.Runtime, NanosSharp");
    switch (result) {
        case RUNTIME_SUCCESS:
            break;
        case RUNTIME_ERROR_HOSTFXR_FAILED:
            return luaL_error(L, "Failed to load hostfxr.");
        case RUNTIME_ERROR_NO_RUNTIME_CONFIG:
            return luaL_error(L, "Failed to find NanosSharp runtime config.");
        case RUNTIME_ERROR_NO_RUNTIME_LIBRARY:
            return luaL_error(L, "Failed to find NanosSharp runtime library.");
        case RUNTIME_ERROR_ASSEMBLY_FAILED:
            return luaL_error(L, "Failed to load NanosSharp assembly.");
        case RUNTIME_ERROR_INIT_FAILED:
            return luaL_error(L, "Failed to initialize NanosSharp runtime.");
        default:
            return luaL_error(L, "Unknown error.");
    }

    struct luaL_Reg functions[] = {
            {"LoadModule", load_module_internally},
            {NULL, NULL},
    };

    luaL_newlib(L, functions);
    return 0;
}

// <% AUTOGENERATE(EXPORT_NATIVES, 0)