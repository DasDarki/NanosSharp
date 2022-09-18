#include <string>
#include <vector>

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

lua_State *current_lua_state = nullptr;
#define LUA current_lua_state
#define ASSERT_LUA_R(r) if (LUA == nullptr) { return r; }
#define ASSERT_LUA ASSERT_LUA_R()

EXTERN int luaopen_nanossharp_runtime (lua_State *L) {
    if (current_lua_state) {
        return 0;
    }

    current_lua_state = L;

    int result = Runtime::GetInstance().Start("6.0.6", "NanosSharp", "NanosSharp.Runtime.Bridge, NanosSharp");
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

    return 0;
}

const char* allocate_string(const std::string& str, int32_t& size) {
    size_t stringSize = str.size();
    size = (int32_t) stringSize;
    char* writable = new char[stringSize + 1];
    std::copy(str.begin(), str.end(), writable);
    writable[stringSize] = '\0';
    return writable;
}

const char** allocate_string_array(std::vector<std::string> arr, uint32_t& size) {
    size = arr.size();
    auto out = new const char*[size];
    for (auto i = 0; i < size; i++) {
        auto el = arr[i];
        auto elSize = el.size();
        auto str = el.c_str();
        auto allocStr = new char[elSize + 1];
        for (auto j = 0; j < elSize; j++) allocStr[j] = str[j];
        allocStr[elSize] = '\0';
        out[i] = allocStr;
    }

    return out;
}

EXPORT void FreeString(const char* string) {
    delete[] string;
}

EXPORT void FreeStringArray(const char** stringArray, uint32_t size) {
    for (int i = 0; i < size; i++) delete[] stringArray[i];
    delete[] stringArray;
}

EXPORT void ScriptLog(const char *message) {
    ASSERT_LUA;

    lua_getglobal(LUA, "print");
    lua_pushstring(LUA, message);
    lua_call(LUA, 1, 0);
}