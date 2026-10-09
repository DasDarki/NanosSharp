#ifndef NANOSSHARP_BRIDGE_NATIVE_API_H_
#define NANOSSHARP_BRIDGE_NATIVE_API_H_

#include <cstdint>

#include "bridge/native_argument.h"
#include "game/package.h"
#include "game/scripting.h"

namespace nanossharp::bridge {

using ValueHandle = game::ScriptValue*;
using ClassHandle = game::IClass*;
using FunctionHandle = game::IFunction*;
using PropertyHandle = game::IProperty*;
using EnvironmentHandle = game::Environment*;

inline constexpr int32_t kNativeApiVersion = 3;

struct ManagedCallbacks {
  void (*engine_attached)();
  void (*engine_detaching)();
  void (*tick)(float delta_time);
  void (*entity_created)(uint64_t class_id, void* instance, uint32_t network_id);
  void (*entity_destroyed)(uint64_t class_id, void* instance);
  ValueHandle (*invoke_function)(int64_t function_id, const NativeArgument* args, int32_t count);
  void (*release_function)(int64_t function_id);
  void (*package_loaded)(game::Package* package, const char* name, const char* directory,
                         EnvironmentHandle environment);
  void (*package_unloading)(game::Package* package);
};

struct NativeApi {
  int32_t version;

  void (*log)(int32_t type, const char* message, int32_t length);
  void (*free_string)(char* value);

  ValueHandle (*value_create_nil)();
  ValueHandle (*value_create_bool)(uint8_t value);
  ValueHandle (*value_create_integer)(int64_t value);
  ValueHandle (*value_create_double)(double value);
  ValueHandle (*value_create_string)(const char* value, int32_t length);
  ValueHandle (*value_create_vector)(double x, double y, double z);
  ValueHandle (*value_create_rotator)(double pitch, double yaw, double roll);
  ValueHandle (*value_create_color)(float r, float g, float b, float a);
  ValueHandle (*value_create_vector2d)(double x, double y);
  ValueHandle (*value_create_entity)(uint64_t class_id, void* instance, uint32_t network_id);
  ValueHandle (*value_create_array)(const ValueHandle* items, int32_t count);
  ValueHandle (*value_create_map)(const ValueHandle* keys, const ValueHandle* values,
                                  int32_t count);
  ValueHandle (*value_create_function)(int64_t function_id, EnvironmentHandle environment);
  ValueHandle (*value_clone)(const game::ScriptValue* value);
  void (*value_free)(ValueHandle value);

  int32_t (*value_get_type)(const game::ScriptValue* value);
  uint8_t (*value_get_bool)(const game::ScriptValue* value);
  int64_t (*value_get_integer)(const game::ScriptValue* value);
  double (*value_get_double)(const game::ScriptValue* value);
  const char* (*value_get_string)(const game::ScriptValue* value, int32_t* length);
  void (*value_get_doubles)(const game::ScriptValue* value, double* output);
  void (*value_get_color)(const game::ScriptValue* value, float* output);
  void (*value_get_entity)(const game::ScriptValue* value, uint64_t* class_id,
                           void** instance, uint32_t* network_id);
  uint32_t (*value_get_network_id)(const game::ScriptValue* value);
  ValueHandle (*value_resolve_deferred)(const game::ScriptValue* value);
  int32_t (*value_get_length)(const game::ScriptValue* value);
  const game::ScriptValue* (*value_get_item)(const game::ScriptValue* value,
                                             int32_t index);
  void (*value_get_entries)(const game::ScriptValue* value,
                            const game::ScriptValue** keys,
                            const game::ScriptValue** values);
  uint8_t (*value_invoke)(const game::ScriptValue* function, const ValueHandle* args,
                          int32_t count, ValueHandle* result, char** error);

  int32_t (*engine_is_attached)();
  int32_t (*class_count)();
  ClassHandle (*class_get)(int32_t index);
  const char* (*class_get_name)(ClassHandle handle);
  uint64_t (*class_get_id)(ClassHandle handle);
  FunctionHandle (*class_get_constructor)(ClassHandle handle);
  int32_t (*class_get_function_count)(ClassHandle handle, uint8_t is_static);
  FunctionHandle (*class_get_function)(ClassHandle handle, uint8_t is_static,
                                       int32_t index);
  int32_t (*class_get_property_count)(ClassHandle handle);
  PropertyHandle (*class_get_property)(ClassHandle handle, int32_t index);
  int32_t (*global_function_count)();
  FunctionHandle (*global_function_get)(int32_t index);
  const char* (*function_get_name)(FunctionHandle handle);
  char* (*function_get_signature)(FunctionHandle handle);
  const char* (*property_get_name)(PropertyHandle handle);
  char* (*property_get_signature)(PropertyHandle handle);

  uint8_t (*function_invoke)(FunctionHandle handle, EnvironmentHandle environment,
                             const ValueHandle* args, int32_t count, ValueHandle* result,
                             char** error);
  uint8_t (*property_get)(PropertyHandle handle, ValueHandle self, ValueHandle* result,
                          char** error);
  uint8_t (*property_set)(PropertyHandle handle, ValueHandle self, ValueHandle value,
                          char** error);

  EnvironmentHandle (*environment_create)(const char* name, int32_t length);
  void (*environment_destroy)(const char* name, int32_t length);
  uint8_t (*package_reload)(game::Package* package, char** error);
  void* (*library_load)(const char* path, char** error);
  void* (*package_get_entity)(game::Package* package);
  uint8_t (*function_call)(FunctionHandle handle, EnvironmentHandle environment, const NativeArgument* args,
                           int32_t count, NativeArgument* result, char** error);
  ValueHandle (*lua_get_global)(EnvironmentHandle environment, const char* name, int32_t length,
                                char** error);
  uint8_t (*lua_set_global)(EnvironmentHandle environment, const char* name, int32_t length,
                            const game::ScriptValue* value, char** error);
};

const NativeApi& GetNativeApi();
ManagedCallbacks& GetManagedCallbacks();

}

#endif
