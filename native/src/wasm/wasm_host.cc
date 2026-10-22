#include "wasm/wasm_host.h"

#include <cstdio>
#include <cstring>
#include <string>

namespace nanossharp::wasm {

namespace {

constexpr int32_t kValueTypeBool = 0;
constexpr int32_t kValueTypeInteger = 1;
constexpr int32_t kValueTypeDouble = 2;
constexpr int32_t kValueTypeString = 3;
constexpr int32_t kValueTypeEntity = 4;

wasm_valtype_t* ValType(char code) {
  switch (code) {
    case 'I':
      return wasm_valtype_new(WASM_I64);
    case 'f':
      return wasm_valtype_new(WASM_F32);
    case 'd':
      return wasm_valtype_new(WASM_F64);
    default:
      return wasm_valtype_new(WASM_I32);
  }
}

wasm_functype_t* FuncSig(const char* params, const char* results) {
  const size_t parameter_count = std::strlen(params);
  const size_t result_count = std::strlen(results);
  wasm_valtype_vec_t parameter_vec;
  wasm_valtype_vec_t result_vec;
  wasm_valtype_vec_new_uninitialized(&parameter_vec, parameter_count);
  for (size_t i = 0; i < parameter_count; ++i) {
    parameter_vec.data[i] = ValType(params[i]);
  }
  wasm_valtype_vec_new_uninitialized(&result_vec, result_count);
  for (size_t i = 0; i < result_count; ++i) {
    result_vec.data[i] = ValType(results[i]);
  }
  return wasm_functype_new(&parameter_vec, &result_vec);
}

wasm_trap_t* MakeTrap(const char* message) {
  return wasmtime_trap_new(message, std::strlen(message));
}

template <typename T>
void PutLittleEndian(std::vector<uint8_t>& blob, T value) {
  for (size_t i = 0; i < sizeof(T); ++i) {
    blob.push_back(static_cast<uint8_t>((value >> (i * 8)) & 0xff));
  }
}

void PutString(std::vector<uint8_t>& blob, const char* text) {
  const auto length = static_cast<uint16_t>(text != nullptr ? std::strlen(text) : 0);
  PutLittleEndian<uint16_t>(blob, length);
  for (uint16_t i = 0; i < length; ++i) {
    blob.push_back(static_cast<uint8_t>(text[i]));
  }
}

int64_t ReadI64(const uint8_t* data, size_t offset) {
  int64_t value = 0;
  std::memcpy(&value, data + offset, sizeof(value));
  return value;
}

int32_t ReadI32(const uint8_t* data, size_t offset) {
  int32_t value = 0;
  std::memcpy(&value, data + offset, sizeof(value));
  return value;
}

double ReadF64(const uint8_t* data, size_t offset) {
  double value = 0;
  std::memcpy(&value, data + offset, sizeof(value));
  return value;
}

}

WasmHost::WasmHost(const bridge::NativeApi& api) : api_(api) {}

WasmHost::~WasmHost() {
  if (module_ != nullptr) {
    wasmtime_module_delete(module_);
  }
  if (linker_ != nullptr) {
    wasmtime_linker_delete(linker_);
  }
  if (store_ != nullptr) {
    wasmtime_store_delete(store_);
  }
  if (engine_ != nullptr) {
    wasm_engine_delete(engine_);
  }
}

int32_t WasmHost::Intern(void* handle, std::vector<void*>& table,
                         std::unordered_map<void*, int32_t>& lookup) {
  if (handle == nullptr) {
    return 0;
  }

  const auto existing = lookup.find(handle);
  if (existing != lookup.end()) {
    return existing->second;
  }

  table.push_back(handle);
  const auto index = static_cast<int32_t>(table.size());
  lookup.emplace(handle, index);
  return index;
}

int32_t WasmHost::InternFunction(bridge::FunctionHandle handle) {
  return Intern(handle, functions_, function_lookup_);
}

int32_t WasmHost::InternProperty(bridge::PropertyHandle handle) {
  return Intern(handle, properties_, property_lookup_);
}

bridge::FunctionHandle WasmHost::ResolveFunction(int32_t index) const {
  if (index <= 0 || static_cast<size_t>(index) > functions_.size()) {
    return nullptr;
  }
  return static_cast<bridge::FunctionHandle>(functions_[index - 1]);
}

int32_t WasmHost::InternValue(bridge::ValueHandle handle) {
  if (handle == nullptr) {
    return 0;
  }
  if (!value_free_list_.empty()) {
    const int32_t index = value_free_list_.back();
    value_free_list_.pop_back();
    values_[index - 1] = handle;
    return index;
  }
  values_.push_back(handle);
  return static_cast<int32_t>(values_.size());
}

bridge::ValueHandle WasmHost::ResolveValue(int32_t index) const {
  if (index <= 0 || static_cast<size_t>(index) > values_.size()) {
    return nullptr;
  }
  return static_cast<bridge::ValueHandle>(values_[index - 1]);
}

bridge::PropertyHandle WasmHost::ResolveProperty(int32_t index) const {
  if (index <= 0 || static_cast<size_t>(index) > properties_.size()) {
    return nullptr;
  }
  return static_cast<bridge::PropertyHandle>(properties_[index - 1]);
}

int32_t WasmHost::InternInstance(void* instance) {
  return Intern(instance, instances_, instance_lookup_);
}

int32_t WasmHost::HostValueCreateEntity(int64_t class_id, int32_t instance, int32_t network_id) {
  return InternValue(api_.value_create_entity(static_cast<uint64_t>(class_id),
                                              ResolveInstance(instance),
                                              static_cast<uint32_t>(network_id)));
}

int32_t WasmHost::HostValueCreateString(int32_t text, int32_t length) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || text < 0 || length < 0 ||
      static_cast<size_t>(text) + static_cast<size_t>(length) > size) {
    return 0;
  }
  return InternValue(
      api_.value_create_string(reinterpret_cast<const char*>(data + text), length));
}

int32_t WasmHost::HostValueCreateVector(double x, double y, double z) {
  return InternValue(api_.value_create_vector(x, y, z));
}

int32_t WasmHost::HostValueGetString(int32_t value, int32_t length) {
  int32_t text_length = 0;
  const char* text = api_.value_get_string(ResolveValue(value), &text_length);

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || result_scratch_ == 0 || text == nullptr || text_length < 0 ||
      text_length > result_scratch_capacity_) {
    if (data != nullptr && length >= 0) {
      const int32_t zero = 0;
      std::memcpy(data + length, &zero, sizeof(int32_t));
    }
    return 0;
  }

  std::memcpy(data + result_scratch_, text, static_cast<size_t>(text_length));
  std::memcpy(data + length, &text_length, sizeof(int32_t));
  return result_scratch_;
}

void WasmHost::HostValueGetDoubles(int32_t value, int32_t output) {
  double buffer[3] = {0, 0, 0};
  api_.value_get_doubles(ResolveValue(value), buffer);

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data != nullptr && output >= 0 &&
      static_cast<size_t>(output) + sizeof(buffer) <= size) {
    std::memcpy(data + output, buffer, sizeof(buffer));
  }
}

int32_t WasmHost::InternEnvironment(bridge::EnvironmentHandle handle) {
  return Intern(handle, environments_, environment_lookup_);
}

bridge::EnvironmentHandle WasmHost::ResolveEnvironment(int32_t index) const {
  if (index <= 0 || static_cast<size_t>(index) > environments_.size()) {
    return nullptr;
  }
  return static_cast<bridge::EnvironmentHandle>(environments_[index - 1]);
}

int32_t WasmHost::WriteScratchString(const char* text) {
  if (result_scratch_ == 0 || text == nullptr) {
    return 0;
  }
  const size_t length = std::strlen(text);
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || length + 1 > static_cast<size_t>(result_scratch_capacity_)) {
    return 0;
  }
  std::memcpy(data + result_scratch_, text, length + 1);
  return result_scratch_;
}

int32_t WasmHost::HostValueCreateRotator(double pitch, double yaw, double roll) {
  return InternValue(api_.value_create_rotator(pitch, yaw, roll));
}

int32_t WasmHost::HostValueCreateColor(float r, float g, float b, float a) {
  return InternValue(api_.value_create_color(r, g, b, a));
}

int32_t WasmHost::HostValueCreateVector2D(double x, double y) {
  return InternValue(api_.value_create_vector2d(x, y));
}

void WasmHost::HostValueGetColor(int32_t value, int32_t output) {
  float buffer[4] = {0, 0, 0, 0};
  api_.value_get_color(ResolveValue(value), buffer);

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data != nullptr && output >= 0 &&
      static_cast<size_t>(output) + sizeof(buffer) <= size) {
    std::memcpy(data + output, buffer, sizeof(buffer));
  }
}

void WasmHost::HostValueGetEntity(int32_t value, int32_t class_id, int32_t instance,
                                  int32_t network_id) {
  uint64_t native_class_id = 0;
  void* native_instance = nullptr;
  uint32_t native_network_id = 0;
  api_.value_get_entity(ResolveValue(value), &native_class_id, &native_instance,
                        &native_network_id);

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return;
  }
  const int32_t instance_index = InternInstance(native_instance);
  std::memcpy(data + class_id, &native_class_id, sizeof(uint64_t));
  std::memcpy(data + instance, &instance_index, sizeof(int32_t));
  std::memcpy(data + network_id, &native_network_id, sizeof(uint32_t));
}

int32_t WasmHost::HostValueGetNetworkId(int32_t value) {
  return static_cast<int32_t>(api_.value_get_network_id(ResolveValue(value)));
}

int32_t WasmHost::HostValueResolveDeferred(int32_t value) {
  return InternValue(api_.value_resolve_deferred(ResolveValue(value)));
}

int32_t WasmHost::HostValueGetLength(int32_t value) {
  return api_.value_get_length(ResolveValue(value));
}

int32_t WasmHost::HostValueGetItem(int32_t value, int32_t index) {
  return InternValue(
      const_cast<bridge::ValueHandle>(api_.value_get_item(ResolveValue(value), index)));
}

void WasmHost::HostValueGetEntries(int32_t value, int32_t keys, int32_t values) {
  const int32_t length = api_.value_get_length(ResolveValue(value));
  if (length <= 0) {
    return;
  }
  std::vector<const game::ScriptValue*> native_keys(length);
  std::vector<const game::ScriptValue*> native_values(length);
  api_.value_get_entries(ResolveValue(value), native_keys.data(), native_values.data());

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return;
  }
  for (int32_t i = 0; i < length; ++i) {
    const int32_t key = InternValue(const_cast<bridge::ValueHandle>(native_keys[i]));
    const int32_t entry = InternValue(const_cast<bridge::ValueHandle>(native_values[i]));
    std::memcpy(data + keys + i * 4, &key, sizeof(int32_t));
    std::memcpy(data + values + i * 4, &entry, sizeof(int32_t));
  }
}

int32_t WasmHost::HostValueCreateArray(int32_t items, int32_t count) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || count < 0) {
    return 0;
  }
  std::vector<bridge::ValueHandle> native_items(count);
  for (int32_t i = 0; i < count; ++i) {
    native_items[i] = ResolveValue(ReadI32(data, static_cast<size_t>(items) + i * 4));
  }
  return InternValue(api_.value_create_array(native_items.data(), count));
}

int32_t WasmHost::HostValueCreateMap(int32_t keys, int32_t values, int32_t count) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || count < 0) {
    return 0;
  }
  std::vector<bridge::ValueHandle> native_keys(count);
  std::vector<bridge::ValueHandle> native_values(count);
  for (int32_t i = 0; i < count; ++i) {
    native_keys[i] = ResolveValue(ReadI32(data, static_cast<size_t>(keys) + i * 4));
    native_values[i] = ResolveValue(ReadI32(data, static_cast<size_t>(values) + i * 4));
  }
  return InternValue(api_.value_create_map(native_keys.data(), native_values.data(), count));
}

int32_t WasmHost::HostValueCreateFunction(int64_t function_id, int32_t environment) {
  return InternValue(api_.value_create_function(function_id, ResolveEnvironment(environment)));
}

int32_t WasmHost::HostValueInvoke(int32_t function, int32_t args, int32_t count, int32_t result,
                                  int32_t error) {
  (void)error;
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || count < 0) {
    return 0;
  }
  std::vector<bridge::ValueHandle> native_args(count);
  for (int32_t i = 0; i < count; ++i) {
    native_args[i] = ResolveValue(ReadI32(data, static_cast<size_t>(args) + i * 4));
  }
  bridge::ValueHandle value = nullptr;
  char* message = nullptr;
  const uint8_t ok =
      api_.value_invoke(ResolveValue(function), native_args.data(), count, &value, &message);
  if (ok == 0) {
    if (message != nullptr) {
      api_.free_string(message);
    }
    return 0;
  }
  const int32_t index = InternValue(value);
  std::memcpy(data + result, &index, sizeof(int32_t));
  return 1;
}

int32_t WasmHost::HostFunctionInvoke(int32_t handle, int32_t environment, int32_t args,
                                     int32_t count, int32_t result, int32_t error) {
  (void)error;
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || count < 0) {
    return 0;
  }
  std::vector<bridge::ValueHandle> native_args(count);
  for (int32_t i = 0; i < count; ++i) {
    native_args[i] = ResolveValue(ReadI32(data, static_cast<size_t>(args) + i * 4));
  }
  bridge::ValueHandle value = nullptr;
  char* message = nullptr;
  const uint8_t ok = api_.function_invoke(ResolveFunction(handle), ResolveEnvironment(environment),
                                          native_args.data(), count, &value, &message);
  if (ok == 0) {
    if (message != nullptr) {
      api_.free_string(message);
    }
    return 0;
  }
  const int32_t index = InternValue(value);
  std::memcpy(data + result, &index, sizeof(int32_t));
  return 1;
}

int32_t WasmHost::HostEnvironmentCreate(int32_t name, int32_t length) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return 0;
  }
  return InternEnvironment(
      api_.environment_create(reinterpret_cast<const char*>(data + name), length));
}

void WasmHost::HostEnvironmentDestroy(int32_t name, int32_t length) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data != nullptr) {
    api_.environment_destroy(reinterpret_cast<const char*>(data + name), length);
  }
}

int32_t WasmHost::HostLuaGetGlobal(int32_t environment, int32_t name, int32_t length,
                                   int32_t error) {
  (void)error;
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return 0;
  }
  char* message = nullptr;
  bridge::ValueHandle value = api_.lua_get_global(ResolveEnvironment(environment),
                                                  reinterpret_cast<const char*>(data + name),
                                                  length, &message);
  if (message != nullptr) {
    api_.free_string(message);
  }
  return InternValue(value);
}

int32_t WasmHost::HostLuaSetGlobal(int32_t environment, int32_t name, int32_t length,
                                   int32_t value, int32_t error) {
  (void)error;
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return 0;
  }
  char* message = nullptr;
  const uint8_t ok =
      api_.lua_set_global(ResolveEnvironment(environment),
                          reinterpret_cast<const char*>(data + name), length,
                          ResolveValue(value), &message);
  if (ok == 0 && message != nullptr) {
    api_.free_string(message);
  }
  return ok;
}

int32_t WasmHost::HostLibraryLoad(int32_t path, int32_t error) {
  (void)error;
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return 0;
  }
  char* message = nullptr;
  void* handle = api_.library_load(reinterpret_cast<const char*>(data + path), &message);
  if (message != nullptr) {
    api_.free_string(message);
  }
  return InternInstance(handle);
}

int32_t WasmHost::HostPackageReload(int32_t package, int32_t error) {
  (void)error;
  char* message = nullptr;
  const uint8_t ok =
      api_.package_reload(static_cast<game::Package*>(ResolveInstance(package)), &message);
  if (ok == 0 && message != nullptr) {
    api_.free_string(message);
  }
  return ok;
}

int32_t WasmHost::HostPackageGetEntity(int32_t package) {
  return InternInstance(
      api_.package_get_entity(static_cast<game::Package*>(ResolveInstance(package))));
}

int32_t WasmHost::HostFunctionGetSignature(int32_t handle) {
  char* signature = api_.function_get_signature(ResolveFunction(handle));
  const int32_t offset = WriteScratchString(signature);
  if (signature != nullptr) {
    api_.free_string(signature);
  }
  return offset;
}

int32_t WasmHost::HostPropertyGetSignature(int32_t handle) {
  char* signature = api_.property_get_signature(ResolveProperty(handle));
  const int32_t offset = WriteScratchString(signature);
  if (signature != nullptr) {
    api_.free_string(signature);
  }
  return offset;
}

int32_t WasmHost::HostPropertyGet(int32_t handle, int32_t self, int32_t result, int32_t error) {
  (void)error;
  bridge::ValueHandle value = nullptr;
  char* message = nullptr;
  const uint8_t ok = api_.property_get(ResolveProperty(handle), ResolveValue(self), &value, &message);
  if (ok == 0) {
    if (message != nullptr) {
      api_.free_string(message);
    }
    return 0;
  }

  size_t size = 0;
  uint8_t* data = GuestData(size);
  const int32_t index = InternValue(value);
  if (data != nullptr && result >= 0) {
    std::memcpy(data + result, &index, sizeof(int32_t));
  }
  return 1;
}

int32_t WasmHost::HostPropertySet(int32_t handle, int32_t self, int32_t value, int32_t error) {
  (void)error;
  char* message = nullptr;
  const uint8_t ok =
      api_.property_set(ResolveProperty(handle), ResolveValue(self), ResolveValue(value), &message);
  if (ok == 0 && message != nullptr) {
    api_.free_string(message);
  }
  return ok;
}

void* WasmHost::ResolveInstance(int32_t index) const {
  if (index <= 0 || static_cast<size_t>(index) > instances_.size()) {
    return nullptr;
  }
  return instances_[index - 1];
}

int32_t WasmHost::HostValueCreateNil() { return InternValue(api_.value_create_nil()); }

int32_t WasmHost::HostValueCreateBool(int32_t value) {
  return InternValue(api_.value_create_bool(value != 0 ? 1 : 0));
}

int32_t WasmHost::HostValueCreateInteger(int64_t value) {
  return InternValue(api_.value_create_integer(value));
}

int32_t WasmHost::HostValueCreateDouble(double value) {
  return InternValue(api_.value_create_double(value));
}

int32_t WasmHost::HostValueClone(int32_t value) {
  return InternValue(api_.value_clone(ResolveValue(value)));
}

void WasmHost::HostValueFree(int32_t value) {
  if (value <= 0 || static_cast<size_t>(value) > values_.size()) {
    return;
  }
  api_.value_free(static_cast<bridge::ValueHandle>(values_[value - 1]));
  values_[value - 1] = nullptr;
  value_free_list_.push_back(value);
}

int32_t WasmHost::HostValueGetType(int32_t value) {
  return api_.value_get_type(ResolveValue(value));
}

int32_t WasmHost::HostValueGetBool(int32_t value) {
  return api_.value_get_bool(ResolveValue(value));
}

int64_t WasmHost::HostValueGetInteger(int32_t value) {
  return api_.value_get_integer(ResolveValue(value));
}

double WasmHost::HostValueGetDouble(int32_t value) {
  return api_.value_get_double(ResolveValue(value));
}

uint8_t* WasmHost::GuestData(size_t& size) {
  if (!has_memory_) {
    size = 0;
    return nullptr;
  }
  size = wasmtime_memory_data_size(context_, &memory_);
  return wasmtime_memory_data(context_, &memory_);
}

std::string WasmHost::ReadGuestString(int32_t pointer, int64_t length) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || pointer < 0 || length < 0 ||
      static_cast<size_t>(pointer) + static_cast<size_t>(length) > size) {
    return {};
  }
  return std::string(reinterpret_cast<const char*>(data + pointer),
                     static_cast<size_t>(length));
}

void WasmHost::HostLog(int32_t type, int32_t pointer, int32_t length) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || pointer < 0 || length < 0 ||
      static_cast<size_t>(pointer) + static_cast<size_t>(length) > size) {
    return;
  }
  api_.log(type, reinterpret_cast<const char*>(data + pointer), length);
}

int32_t WasmHost::HostEngineIsAttached() { return api_.engine_is_attached(); }

void WasmHost::HostFreeString(int32_t) {}

void WasmHost::HostSetResultScratch(int32_t buffer, int32_t capacity) {
  result_scratch_ = buffer;
  result_scratch_capacity_ = capacity;
}

std::vector<uint8_t> WasmHost::BuildSchemaBlob() {
  std::vector<uint8_t> blob;

  const int32_t class_count = api_.class_count();
  PutLittleEndian<uint32_t>(blob, static_cast<uint32_t>(class_count));
  for (int32_t index = 0; index < class_count; ++index) {
    bridge::ClassHandle handle = api_.class_get(index);
    PutLittleEndian<uint64_t>(blob, api_.class_get_id(handle));
    PutString(blob, api_.class_get_name(handle));

    bridge::FunctionHandle constructor = api_.class_get_constructor(handle);
    PutLittleEndian<int32_t>(blob, InternFunction(constructor));
    if (constructor != nullptr) {
      PutString(blob, api_.function_get_name(constructor));
    }

    for (uint8_t is_static = 0; is_static <= 1; ++is_static) {
      const int32_t count = api_.class_get_function_count(handle, is_static);
      PutLittleEndian<uint16_t>(blob, static_cast<uint16_t>(count));
      for (int32_t i = 0; i < count; ++i) {
        bridge::FunctionHandle function = api_.class_get_function(handle, is_static, i);
        PutLittleEndian<int32_t>(blob, InternFunction(function));
        PutString(blob, api_.function_get_name(function));
      }
    }

    const int32_t property_count = api_.class_get_property_count(handle);
    PutLittleEndian<uint16_t>(blob, static_cast<uint16_t>(property_count));
    for (int32_t i = 0; i < property_count; ++i) {
      bridge::PropertyHandle property = api_.class_get_property(handle, i);
      PutLittleEndian<int32_t>(blob, InternProperty(property));
      PutString(blob, api_.property_get_name(property));
    }
  }

  const int32_t function_count = api_.global_function_count();
  PutLittleEndian<uint32_t>(blob, static_cast<uint32_t>(function_count));
  for (int32_t index = 0; index < function_count; ++index) {
    bridge::FunctionHandle function = api_.global_function_get(index);
    PutLittleEndian<int32_t>(blob, InternFunction(function));
    PutString(blob, api_.function_get_name(function));
  }

  return blob;
}

int32_t WasmHost::HostSchemaDump(int32_t buffer, int32_t capacity) {
  const std::vector<uint8_t> blob = BuildSchemaBlob();
  const auto needed = static_cast<int32_t>(blob.size());
  if (needed > capacity) {
    return needed;
  }

  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr || buffer < 0 ||
      static_cast<size_t>(buffer) + blob.size() > size) {
    return needed;
  }

  std::memcpy(data + buffer, blob.data(), blob.size());
  return needed;
}

int32_t WasmHost::HostFunctionCall(int32_t method, int32_t environment, int32_t args,
                                   int32_t count, int32_t result, int32_t error) {
  size_t size = 0;
  uint8_t* data = GuestData(size);
  if (data == nullptr) {
    return 0;
  }

  std::vector<bridge::NativeArgument> native_arguments(count);
  for (int32_t i = 0; i < count; ++i) {
    const size_t base = static_cast<size_t>(args) + static_cast<size_t>(i) * 32;
    bridge::NativeArgument& target = native_arguments[i];
    std::memset(&target, 0, sizeof(target));
    target.type = data[base];
    switch (static_cast<int32_t>(target.type)) {
      case kValueTypeBool:
        target.boolean = data[base + 8];
        break;
      case kValueTypeInteger:
        target.integer = ReadI64(data, base + 8);
        break;
      case kValueTypeDouble:
        target.number = ReadF64(data, base + 8);
        break;
      case kValueTypeString: {
        const int32_t pointer = ReadI32(data, base + 8);
        const int64_t length = ReadI64(data, base + 16);
        target.string.data = reinterpret_cast<const char*>(data + pointer);
        target.string.length = length;
        break;
      }
      case kValueTypeEntity: {
        std::memcpy(&target.entity.class_id, data + base + 8, sizeof(uint64_t));
        target.entity.instance = ResolveInstance(ReadI32(data, base + 16));
        std::memcpy(&target.entity.network_id, data + base + 24, sizeof(uint32_t));
        break;
      }
      default:
        std::memcpy(&target.integer, data + base + 8, 24);
        break;
    }
  }

  bridge::FunctionHandle function = ResolveFunction(method);
  auto environment_handle =
      reinterpret_cast<bridge::EnvironmentHandle>(static_cast<intptr_t>(environment));

  bridge::NativeArgument native_result;
  std::memset(&native_result, 0, sizeof(native_result));
  char* message = nullptr;
  const uint8_t succeeded =
      api_.function_call(function, environment_handle, native_arguments.data(), count,
                         &native_result, &message);

  if (succeeded == 0) {
    if (message != nullptr && error != 0) {
      (void)error;
      api_.free_string(message);
    }
    return 0;
  }

  const size_t result_base = static_cast<size_t>(result);
  std::memset(data + result_base, 0, 32);
  data[result_base] = native_result.type;
  switch (static_cast<int32_t>(native_result.type)) {
    case kValueTypeBool:
      data[result_base + 8] = native_result.boolean;
      break;
    case kValueTypeInteger:
      std::memcpy(data + result_base + 8, &native_result.integer, sizeof(int64_t));
      break;
    case kValueTypeDouble:
      std::memcpy(data + result_base + 8, &native_result.number, sizeof(double));
      break;
    case kValueTypeString: {
      const int64_t length = native_result.string.length;
      if (result_scratch_ != 0 && length >= 0 && length <= result_scratch_capacity_) {
        std::memcpy(data + result_scratch_, native_result.string.data,
                    static_cast<size_t>(length));
        std::memcpy(data + result_base + 8, &result_scratch_, sizeof(int32_t));
        std::memcpy(data + result_base + 16, &length, sizeof(int64_t));
      }
      break;
    }
    case kValueTypeEntity: {
      const int32_t instance_index = InternInstance(native_result.entity.instance);
      std::memcpy(data + result_base + 8, &native_result.entity.class_id, sizeof(uint64_t));
      std::memcpy(data + result_base + 16, &instance_index, sizeof(int32_t));
      std::memcpy(data + result_base + 24, &native_result.entity.network_id, sizeof(uint32_t));
      break;
    }
    default:
      std::memcpy(data + result_base + 8, &native_result.integer, 24);
      break;
  }

  return 1;
}

bool WasmHost::Start(const std::filesystem::path& core_module,
                     const std::filesystem::path& package_directory, std::string& error) {
  engine_ = wasm_engine_new();
  store_ = wasmtime_store_new(engine_, nullptr, nullptr);
  context_ = wasmtime_store_context(store_);

  std::FILE* file = std::fopen(core_module.c_str(), "rb");
  if (file == nullptr) {
    error = "unable to open core module " + core_module.string();
    return false;
  }
  std::fseek(file, 0, SEEK_END);
  const long length = std::ftell(file);
  std::fseek(file, 0, SEEK_SET);
  std::vector<uint8_t> bytes(static_cast<size_t>(length));
  const size_t read = std::fread(bytes.data(), 1, bytes.size(), file);
  std::fclose(file);
  if (read != bytes.size()) {
    error = "unable to read core module " + core_module.string();
    return false;
  }

  wasmtime_error_t* compile_error =
      wasmtime_module_new(engine_, bytes.data(), bytes.size(), &module_);
  if (compile_error != nullptr) {
    error = "failed to compile core module";
    wasmtime_error_delete(compile_error);
    return false;
  }

  linker_ = wasmtime_linker_new(engine_);
  wasmtime_error_t* wasi_error = wasmtime_linker_define_wasi(linker_);
  if (wasi_error != nullptr) {
    error = "failed to define wasi";
    wasmtime_error_delete(wasi_error);
    return false;
  }

  wasi_config_t* wasi_config = wasi_config_new();
  wasi_config_inherit_stdout(wasi_config);
  wasi_config_inherit_stderr(wasi_config);
  const char* argv[] = {"nanossharp"};
  wasi_config_set_argv(wasi_config, 1, argv);
  const std::string package_string = package_directory.string();
  wasi_config_preopen_dir(wasi_config, package_string.c_str(), "/package",
                          WASMTIME_WASI_DIR_PERMS_READ, WASMTIME_WASI_FILE_PERMS_READ);
  wasmtime_error_t* set_wasi_error = wasmtime_context_set_wasi(context_, wasi_config);
  if (set_wasi_error != nullptr) {
    error = "failed to set wasi context";
    wasmtime_error_delete(set_wasi_error);
    return false;
  }

  return DefineImports(error) && DefineTrapsForUnboundImports(error);
}

bool WasmHost::DefineImports(std::string& error) {
  struct Binding {
    const char* name;
    const char* params;
    const char* results;
    wasmtime_func_callback_t callback;
  };

  const Binding bindings[] = {
      {"log", "iii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostLog(args[0].of.i32, args[1].of.i32, args[2].of.i32);
         return nullptr;
       }},
      {"engine-is-attached", "", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t*, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostEngineIsAttached();
         return nullptr;
       }},
      {"free-string", "i", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostFreeString(args[0].of.i32);
         return nullptr;
       }},
      {"schema-dump", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostSchemaDump(args[0].of.i32, args[1].of.i32);
         return nullptr;
       }},
      {"set-result-scratch", "ii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostSetResultScratch(args[0].of.i32, args[1].of.i32);
         return nullptr;
       }},
      {"function-call", "iiiiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 =
             self->HostFunctionCall(args[0].of.i32, args[1].of.i32, args[2].of.i32,
                                    args[3].of.i32, args[4].of.i32, args[5].of.i32);
         return nullptr;
       }},
      {"value-create-nil", "", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t*, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateNil();
         return nullptr;
       }},
      {"value-create-bool", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateBool(args[0].of.i32);
         return nullptr;
       }},
      {"value-create-integer", "I", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateInteger(args[0].of.i64);
         return nullptr;
       }},
      {"value-create-double", "d", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateDouble(args[0].of.f64);
         return nullptr;
       }},
      {"value-clone", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueClone(args[0].of.i32);
         return nullptr;
       }},
      {"value-free", "i", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostValueFree(args[0].of.i32);
         return nullptr;
       }},
      {"value-get-type", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueGetType(args[0].of.i32);
         return nullptr;
       }},
      {"value-get-bool", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueGetBool(args[0].of.i32);
         return nullptr;
       }},
      {"value-get-integer", "i", "I",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I64;
         results[0].of.i64 = self->HostValueGetInteger(args[0].of.i32);
         return nullptr;
       }},
      {"value-get-double", "i", "d",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_F64;
         results[0].of.f64 = self->HostValueGetDouble(args[0].of.i32);
         return nullptr;
       }},
      {"value-create-entity", "Iii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 =
             self->HostValueCreateEntity(args[0].of.i64, args[1].of.i32, args[2].of.i32);
         return nullptr;
       }},
      {"property-get", "iiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostPropertyGet(args[0].of.i32, args[1].of.i32,
                                                    args[2].of.i32, args[3].of.i32);
         return nullptr;
       }},
      {"property-set", "iiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostPropertySet(args[0].of.i32, args[1].of.i32,
                                                   args[2].of.i32, args[3].of.i32);
         return nullptr;
       }},
      {"value-create-string", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateString(args[0].of.i32, args[1].of.i32);
         return nullptr;
       }},
      {"value-create-vector", "ddd", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueCreateVector(args[0].of.f64, args[1].of.f64,
                                                         args[2].of.f64);
         return nullptr;
       }},
      {"value-get-string", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t* results, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         results[0].kind = WASMTIME_I32;
         results[0].of.i32 = self->HostValueGetString(args[0].of.i32, args[1].of.i32);
         return nullptr;
       }},
      {"value-get-doubles", "ii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* args, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostValueGetDoubles(args[0].of.i32, args[1].of.i32);
         return nullptr;
       }},
      {"value-create-rotator", "ddd", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateRotator(a[0].of.f64, a[1].of.f64, a[2].of.f64);
         return nullptr;
       }},
      {"value-create-color", "ffff", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateColor(a[0].of.f32, a[1].of.f32, a[2].of.f32, a[3].of.f32);
         return nullptr;
       }},
      {"value-create-vector2-d", "dd", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateVector2D(a[0].of.f64, a[1].of.f64);
         return nullptr;
       }},
      {"value-get-color", "ii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostValueGetColor(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"value-get-entity", "iiii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostValueGetEntity(a[0].of.i32, a[1].of.i32, a[2].of.i32, a[3].of.i32);
         return nullptr;
       }},
      {"value-get-network-id", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueGetNetworkId(a[0].of.i32);
         return nullptr;
       }},
      {"value-resolve-deferred", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueResolveDeferred(a[0].of.i32);
         return nullptr;
       }},
      {"value-get-length", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueGetLength(a[0].of.i32);
         return nullptr;
       }},
      {"value-get-item", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueGetItem(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"value-get-entries", "iii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostValueGetEntries(a[0].of.i32, a[1].of.i32, a[2].of.i32);
         return nullptr;
       }},
      {"value-create-array", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateArray(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"value-create-map", "iii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateMap(a[0].of.i32, a[1].of.i32, a[2].of.i32);
         return nullptr;
       }},
      {"value-create-function", "Ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostValueCreateFunction(a[0].of.i64, a[1].of.i32);
         return nullptr;
       }},
      {"value-invoke", "iiiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 =
             self->HostValueInvoke(a[0].of.i32, a[1].of.i32, a[2].of.i32, a[3].of.i32, a[4].of.i32);
         return nullptr;
       }},
      {"function-invoke", "iiiiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostFunctionInvoke(a[0].of.i32, a[1].of.i32, a[2].of.i32,
                                                a[3].of.i32, a[4].of.i32, a[5].of.i32);
         return nullptr;
       }},
      {"environment-create", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostEnvironmentCreate(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"environment-destroy", "ii", "",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t*, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         self->HostEnvironmentDestroy(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"lua-get-global", "iiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostLuaGetGlobal(a[0].of.i32, a[1].of.i32, a[2].of.i32, a[3].of.i32);
         return nullptr;
       }},
      {"lua-set-global", "iiiii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 =
             self->HostLuaSetGlobal(a[0].of.i32, a[1].of.i32, a[2].of.i32, a[3].of.i32, a[4].of.i32);
         return nullptr;
       }},
      {"library-load", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostLibraryLoad(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"package-reload", "ii", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostPackageReload(a[0].of.i32, a[1].of.i32);
         return nullptr;
       }},
      {"package-get-entity", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostPackageGetEntity(a[0].of.i32);
         return nullptr;
       }},
      {"function-get-signature", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostFunctionGetSignature(a[0].of.i32);
         return nullptr;
       }},
      {"property-get-signature", "i", "i",
       [](void* env, wasmtime_caller_t* caller, const wasmtime_val_t* a, size_t,
          wasmtime_val_t* r, size_t) -> wasm_trap_t* {
         auto* self = static_cast<WasmHost*>(env);
         self->caller_ = caller;
         r[0].kind = WASMTIME_I32;
         r[0].of.i32 = self->HostPropertyGetSignature(a[0].of.i32);
         return nullptr;
       }},
  };

  for (const Binding& binding : bindings) {
    wasm_functype_t* type = FuncSig(binding.params, binding.results);
    wasmtime_error_t* define_error =
        wasmtime_linker_define_func(linker_, "nanos", 5, binding.name,
                                    std::strlen(binding.name), type, binding.callback, this,
                                    nullptr);
    wasm_functype_delete(type);
    if (define_error != nullptr) {
      error = std::string("failed to define nanos import ") + binding.name;
      wasmtime_error_delete(define_error);
      return false;
    }
  }

  wasm_functype_t* close_type = FuncSig("i", "i");
  wasmtime_error_t* close_error = wasmtime_linker_define_func(
      linker_, "wasi_snapshot_preview1", 22, "adapter_close_badfd", 19, close_type,
      [](void*, wasmtime_caller_t*, const wasmtime_val_t*, size_t, wasmtime_val_t* results,
         size_t) -> wasm_trap_t* {
        results[0].kind = WASMTIME_I32;
        results[0].of.i32 = 0;
        return nullptr;
      },
      this, nullptr);
  wasm_functype_delete(close_type);
  if (close_error != nullptr) {
    error = "failed to define adapter_close_badfd";
    wasmtime_error_delete(close_error);
    return false;
  }

  return true;
}

bool WasmHost::DefineTrapsForUnboundImports(std::string& error) {
  wasm_importtype_vec_t imports;
  wasmtime_module_imports(module_, &imports);

  for (size_t i = 0; i < imports.size; ++i) {
    const wasm_importtype_t* import = imports.data[i];
    const wasm_name_t* module_name = wasm_importtype_module(import);
    const wasm_name_t* field_name = wasm_importtype_name(import);

    wasmtime_extern_t item;
    if (wasmtime_linker_get(linker_, context_, module_name->data, module_name->size,
                            field_name->data, field_name->size, &item)) {
      continue;
    }

    const wasm_externtype_t* extern_type = wasm_importtype_type(import);
    const wasm_functype_t* function_type = wasm_externtype_as_functype_const(extern_type);
    if (function_type == nullptr) {
      continue;
    }

    wasmtime_error_t* define_error = wasmtime_linker_define_func(
        linker_, module_name->data, module_name->size, field_name->data, field_name->size,
        function_type,
        [](void*, wasmtime_caller_t*, const wasmtime_val_t*, size_t, wasmtime_val_t*,
           size_t) -> wasm_trap_t* { return MakeTrap("unbound import called"); },
        nullptr, nullptr);
    if (define_error != nullptr) {
      wasm_importtype_vec_delete(&imports);
      error = "failed to define trap for unbound import";
      wasmtime_error_delete(define_error);
      return false;
    }
  }

  wasm_importtype_vec_delete(&imports);
  return true;
}

bool WasmHost::RunStart(std::string& error) {
  wasm_trap_t* trap = nullptr;
  wasmtime_error_t* instantiate_error =
      wasmtime_linker_instantiate(linker_, context_, module_, &instance_, &trap);
  if (instantiate_error != nullptr) {
    error = "failed to instantiate core module";
    wasmtime_error_delete(instantiate_error);
    return false;
  }
  if (trap != nullptr) {
    error = "core module trapped during instantiation";
    wasm_trap_delete(trap);
    return false;
  }
  instantiated_ = true;

  wasmtime_extern_t memory_export;
  if (wasmtime_instance_export_get(context_, &instance_, "memory", 6, &memory_export) &&
      memory_export.kind == WASMTIME_EXTERN_MEMORY) {
    memory_ = memory_export.of.memory;
    has_memory_ = true;
  }

  return CallInit("_initialize", 0, false, error) &&
         CallInit("initialize_runtime", 1, false, error) &&
         CallInit("nanos_boot", 0, true, error);
}

bool WasmHost::CallInit(const char* name, size_t results, bool required, std::string& error) {
  wasmtime_extern_t function;
  if (!wasmtime_instance_export_get(context_, &instance_, name, std::strlen(name), &function) ||
      function.kind != WASMTIME_EXTERN_FUNC) {
    if (!required) {
      return true;
    }
    error = std::string("reactor module does not export ") + name;
    return false;
  }

  wasmtime_val_t result;
  wasm_trap_t* trap = nullptr;
  wasmtime_error_t* call_error =
      wasmtime_func_call(context_, &function.of.func, nullptr, 0, results > 0 ? &result : nullptr,
                         results, &trap);
  if (call_error != nullptr) {
    error = std::string("failed to call ") + name;
    wasmtime_error_delete(call_error);
    return false;
  }
  if (trap != nullptr) {
    wasm_message_t message;
    wasm_trap_message(trap, &message);
    error = std::string("guest trapped in ") + name + ": " +
            std::string(message.data, message.size > 0 ? message.size - 1 : 0);
    wasm_byte_vec_delete(&message);
    wasm_trap_delete(trap);
    return false;
  }

  return true;
}

bool WasmHost::CallExport(const char* name, const wasmtime_val_t* args, size_t count,
                          std::string& error) {
  if (!instantiated_) {
    error = "core module is not instantiated";
    return false;
  }

  wasmtime_extern_t function;
  if (!wasmtime_instance_export_get(context_, &instance_, name, std::strlen(name), &function) ||
      function.kind != WASMTIME_EXTERN_FUNC) {
    error = std::string("guest does not export ") + name;
    return false;
  }

  wasm_trap_t* trap = nullptr;
  wasmtime_error_t* call_error =
      wasmtime_func_call(context_, &function.of.func, args, count, nullptr, 0, &trap);
  if (call_error != nullptr) {
    error = std::string("failed to call ") + name;
    wasmtime_error_delete(call_error);
    return false;
  }
  if (trap != nullptr) {
    wasm_message_t message;
    wasm_trap_message(trap, &message);
    error = std::string("guest trapped in ") + name + ": " +
            std::string(message.data, message.size > 0 ? message.size - 1 : 0);
    wasm_byte_vec_delete(&message);
    wasm_trap_delete(trap);
    return false;
  }

  return true;
}

bool WasmHost::EngineAttached(std::string& error) {
  return CallExport("nanos_engine_attached", nullptr, 0, error);
}

bool WasmHost::EngineDetaching(std::string& error) {
  return CallExport("nanos_engine_detaching", nullptr, 0, error);
}

bool WasmHost::Tick(float delta_time, std::string& error) {
  wasmtime_val_t argument;
  argument.kind = WASMTIME_F32;
  argument.of.f32 = delta_time;
  return CallExport("nanos_tick", &argument, 1, error);
}

bool WasmHost::CallInt(const char* name, int32_t argument, int64_t* result, std::string& error) {
  wasmtime_extern_t function;
  if (!wasmtime_instance_export_get(context_, &instance_, name, std::strlen(name), &function) ||
      function.kind != WASMTIME_EXTERN_FUNC) {
    error = std::string("guest does not export ") + name;
    return false;
  }

  wasmtime_val_t argument_value;
  argument_value.kind = WASMTIME_I32;
  argument_value.of.i32 = argument;
  wasmtime_val_t result_value;
  wasm_trap_t* trap = nullptr;
  wasmtime_error_t* call_error =
      wasmtime_func_call(context_, &function.of.func, &argument_value, 1,
                         result != nullptr ? &result_value : nullptr, result != nullptr ? 1 : 0,
                         &trap);
  if (call_error != nullptr) {
    error = std::string("failed to call ") + name;
    wasmtime_error_delete(call_error);
    return false;
  }
  if (trap != nullptr) {
    error = std::string("guest trapped in ") + name;
    wasm_trap_delete(trap);
    return false;
  }

  if (result != nullptr) {
    *result = result_value.of.i64;
  }
  return true;
}

}
