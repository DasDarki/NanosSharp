#include "runtime.h"

extern "C" {
    #include "lua.h"
    #include "lauxlib.h"
}

lua_State *current_lua_state = nullptr;

void print(const char *message) {
    if (!current_lua_state) {
        return;
    }

    lua_getglobal(current_lua_state, "print");
    lua_pushstring(current_lua_state, message);
    lua_call(current_lua_state, 1, 0);
}

extern "C" int luaopen_nanossharp_runtime (lua_State *L) {
    if (current_lua_state) {
        return 0;
    }

    current_lua_state = L;

    print("Starting NanosSharp Runtime...");

    int result = Runtime::GetInstance().Start("6.0.6", "NanosSharp", "NanosSharp.Runtime.Bridge, NanosSharp", print);
    switch (result) {
        case RUNTIME_SUCCESS:
            print("NanosSharp Runtime preparation successful.");
            break;
        case RUNTIME_ERROR_HOSTFXR_FAILED:
            return luaL_error(L, "Failed to load hostfxr.");
        case RUNTIME_ERROR_NO_RUNTIME_CONFIG:
            return luaL_error(L, "Failed to find NanosSharp runtime config.");
        case RUNTIME_ERROR_NO_RUNTIME_LIBRARY:
            return luaL_error(L, "Failed to find NanosSharp runtime library.");
        case RUNTIME_ERROR_ASSEMBLY_FAILED:
            return luaL_error(L, "Failed to load NanosSharp assembly.");
        case RUNTIME_ERROR_NO_INITIALIZE:
            return luaL_error(L, "Failed to find NanosSharp initialize function.");
        default:
            return luaL_error(L, "Unknown error.");
    }

    print("Starting NanosSharp API...");
    Runtime::GetInstance().Initialize();
    print("NanosSharp started successfully.");

    return 0;
}