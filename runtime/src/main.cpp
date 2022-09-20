#include <string>
#include <vector>

#include "runtime.h"
#include "icvalues.h"

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

// region Hardcoded API

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

EXPORT void ErrorLog(const char *message) {
    ASSERT_LUA;

    luaL_error(LUA, message);
}

EXPORT void *ICValue_CreateNull() {
    const auto ptr = new ICNullValue();
    return (void *) ptr;
}

EXPORT void *ICValue_CreateBoolean(bool value) {
    const auto ptr = new ICBooleanValue(value);
    return (void *) ptr;
}

EXPORT void *ICValue_CreateInteger(long long value) {
    const auto ptr = new ICIntegerValue(value);
    return (void *) ptr;
}

EXPORT void *ICValue_CreateDouble(double value) {
    const auto ptr = new ICDoubleValue(value);
    return (void *) ptr;
}

EXPORT void *ICValue_CreateString(const char *value) {
    const auto ptr = new ICStringValue(value);
    return (void *) ptr;
}

EXPORT void *ICValue_CreateArray() {
    std::vector<ICValue *> vec;
    const auto ptr = new ICArrayValue(vec);
    return (void *) ptr;
}

EXPORT void *ICValue_CreatePointer(void *val_ptr) {
    const auto ptr = new ICPointerValue(val_ptr);
    return (void *) ptr;
}

EXPORT uint8_t ICValue_GetType(void *value) {
    const auto val = (ICValue *) value;
    return val->GetType();
}

EXPORT void ICValue_Destroy(void *value) {
    const auto val = (ICValue *) value;
    delete val;
}

EXPORT bool ICValue_GetBoolean(void *value) {
    const auto val = (ICBooleanValue *) value;
    return val->Value;
}

EXPORT long long ICValue_GetInteger(void *value) {
    const auto val = (ICIntegerValue *) value;
    return val->Value;
}

EXPORT double ICValue_GetDouble(void *value) {
    const auto val = (ICDoubleValue *) value;
    return val->Value;
}

EXPORT const char *ICValue_GetString(void *value, int32_t& size) {
    const auto val = (ICStringValue *) value;
    return allocate_string(val->Value, size);
}

EXPORT uint32_t ICValue_GetArraySize(void *value) {
    const auto val = (ICArrayValue *) value;
    return val->Value.size();
}

EXPORT void *ICValue_GetArrayElement(void *value, uint32_t index) {
    const auto val = (ICArrayValue *) value;
    return (void *) val->Value[index];
}

EXPORT void *ICValue_GetPointer(void *value) {
    const auto val = (ICPointerValue *) value;
    return val->Value;
}

EXPORT void ICValue_AddArrayElement(void *value, void *element) {
    const auto val = (ICArrayValue *) value;
    val->Value.push_back((ICValue *) element);
}

// endregion

// <% AUTOGENERATE(EXPORT_NATIVES, 0)