#include "bridge/native_api.h"

#include <cxxabi.h>

#include <cstdlib>
#include <cstring>
#include <exception>
#include <memory>
#include <string>
#include <typeinfo>
#include <utility>
#include <vector>

#include "bridge/managed_function_ref.h"
#include "host/isolated_libraries.h"
#include "runtime.h"

namespace nanossharp::bridge {

namespace {

using game::ScriptValue;
using game::ValueType;

constexpr game::CompatibilityVersion kEnvironmentVersion{1, 0, 0, 0};

char* CopyString(const std::string& value) {
  char* copy = static_cast<char*>(std::malloc(value.size() + 1));
  std::memcpy(copy, value.c_str(), value.size() + 1);
  return copy;
}

char* DescribeType(const void* polymorphic_object) {
  auto vtable = *static_cast<const std::type_info* const* const*>(polymorphic_object);
  const std::type_info* type = vtable[-1];
  int status = 0;
  char* demangled = abi::__cxa_demangle(type->name(), nullptr, nullptr, &status);
  if (status != 0 || demangled == nullptr) {
    return CopyString(type->name());
  }
  return demangled;
}

template <typename Function>
uint8_t Guard(char** error, Function&& function) {
  try {
    function();
    return 1;
  } catch (const std::exception& exception) {
    if (error != nullptr) {
      *error = CopyString(exception.what());
    }
  } catch (...) {
    if (error != nullptr) {
      *error = CopyString("unknown native exception");
    }
  }
  return 0;
}

ValueHandle NewValue(game::ScriptVariant value) {
  return new ScriptValue{std::move(value)};
}

const game::FunctionList& Functions(ClassHandle handle, uint8_t is_static) {
  return is_static != 0 ? handle->GetStaticFunctions() : handle->GetFunctions();
}

std::vector<ScriptValue> CollectArguments(const ValueHandle* args, int32_t count) {
  std::vector<ScriptValue> arguments;
  arguments.reserve(count);
  for (int32_t i = 0; i < count; ++i) {
    arguments.push_back(*args[i]);
  }
  return arguments;
}

void Log(int32_t type, const char* message, int32_t length) {
  Runtime::Instance().Log(static_cast<game::LogType>(type),
                          std::string(message, length));
}

void FreeString(char* value) { std::free(value); }

ValueHandle CreateNil() { return NewValue(std::monostate{}); }
ValueHandle CreateBool(uint8_t value) { return NewValue(value != 0); }
ValueHandle CreateInteger(int64_t value) { return NewValue(value); }
ValueHandle CreateDouble(double value) { return NewValue(value); }

ValueHandle CreateString(const char* value, int32_t length) {
  return NewValue(std::string(value, length));
}

ValueHandle CreateVector(double x, double y, double z) {
  return NewValue(game::Vector{x, y, z});
}

ValueHandle CreateRotator(double pitch, double yaw, double roll) {
  return NewValue(game::Rotator{static_cast<float>(pitch), static_cast<float>(yaw),
                                static_cast<float>(roll)});
}

ValueHandle CreateColor(float r, float g, float b, float a) {
  return NewValue(game::Color{r, g, b, a});
}

ValueHandle CreateVector2D(double x, double y) {
  return NewValue(game::Vector2D{static_cast<float>(x), static_cast<float>(y)});
}

ValueHandle CreateEntity(uint64_t class_id, void* instance, uint32_t network_id) {
  return NewValue(game::Entity{class_id, instance, network_id});
}

ValueHandle CreateArray(const ValueHandle* items, int32_t count) {
  auto array = std::make_shared<game::ScriptArray>();
  array->reserve(count);
  for (int32_t i = 0; i < count; ++i) {
    array->push_back(*items[i]);
  }
  return NewValue(std::move(array));
}

ValueHandle CreateMap(const ValueHandle* keys, const ValueHandle* values,
                      int32_t count) {
  auto map = std::make_shared<game::ScriptMap>();
  map->reserve(count);
  for (int32_t i = 0; i < count; ++i) {
    map->insert_or_assign(*keys[i], *values[i]);
  }
  return NewValue(std::move(map));
}

ValueHandle CreateFunction(int64_t function_id, EnvironmentHandle environment) {
  std::shared_ptr<game::IScriptFunctionRef> function =
      std::make_shared<ManagedFunctionRef>(function_id, environment);
  return NewValue(std::move(function));
}

ValueHandle Clone(const ScriptValue* value) { return new ScriptValue(*value); }

void Free(ValueHandle value) { delete value; }

int32_t GetType(const ScriptValue* value) {
  return static_cast<int32_t>(value->value.index());
}

uint8_t GetBool(const ScriptValue* value) {
  const bool* result = std::get_if<bool>(&value->value);
  return result != nullptr && *result ? 1 : 0;
}

int64_t GetInteger(const ScriptValue* value) {
  if (const int64_t* integer = std::get_if<int64_t>(&value->value)) {
    return *integer;
  }
  if (const double* number = std::get_if<double>(&value->value)) {
    return static_cast<int64_t>(*number);
  }
  return 0;
}

double GetDouble(const ScriptValue* value) {
  if (const double* number = std::get_if<double>(&value->value)) {
    return *number;
  }
  if (const int64_t* integer = std::get_if<int64_t>(&value->value)) {
    return static_cast<double>(*integer);
  }
  return 0;
}

const char* GetString(const ScriptValue* value, int32_t* length) {
  const std::string* string = std::get_if<std::string>(&value->value);
  if (string == nullptr) {
    *length = 0;
    return nullptr;
  }
  *length = static_cast<int32_t>(string->size());
  return string->data();
}

void GetDoubles(const ScriptValue* value, double* output) {
  if (const auto* vector = std::get_if<game::Vector>(&value->value)) {
    output[0] = vector->x;
    output[1] = vector->y;
    output[2] = vector->z;
  } else if (const auto* rotator = std::get_if<game::Rotator>(&value->value)) {
    output[0] = rotator->pitch;
    output[1] = rotator->yaw;
    output[2] = rotator->roll;
  } else if (const auto* vector2d = std::get_if<game::Vector2D>(&value->value)) {
    output[0] = vector2d->x;
    output[1] = vector2d->y;
  }
}

void GetColor(const ScriptValue* value, float* output) {
  if (const auto* color = std::get_if<game::Color>(&value->value)) {
    output[0] = color->r;
    output[1] = color->g;
    output[2] = color->b;
    output[3] = color->a;
  }
}

void GetEntity(const ScriptValue* value, uint64_t* class_id, void** instance, uint32_t* network_id) {
  const auto* entity = std::get_if<game::Entity>(&value->value);
  *class_id = entity != nullptr ? entity->class_id : 0;
  *instance = entity != nullptr ? entity->instance : nullptr;
  *network_id = entity != nullptr ? entity->network_id : 0;
}

uint32_t GetNetworkId(const ScriptValue* value) {
  const auto* entity = std::get_if<game::EntityDeferred>(&value->value);
  return entity != nullptr ? entity->network_id : 0;
}

const game::ScriptArray* GetSequence(const ScriptValue* value) {
  if (const auto* array = std::get_if<std::shared_ptr<game::ScriptArray>>(&value->value)) {
    return array->get();
  }
  if (const auto* tuple = std::get_if<game::Tuple>(&value->value)) {
    return &tuple->values;
  }
  return nullptr;
}

ValueHandle ResolveDeferred(const ScriptValue* value) {
  const auto* entity = std::get_if<game::EntityDeferred>(&value->value);
  game::EntityFetcher* fetcher = game::GetSymbols().entity_fetcher;
  if (entity == nullptr || fetcher == nullptr || !*fetcher) {
    return CreateNil();
  }
  try {
    return NewValue((*fetcher)(entity->network_id).value);
  } catch (...) {
    return CreateNil();
  }
}

int32_t GetLength(const ScriptValue* value) {
  if (const game::ScriptArray* sequence = GetSequence(value)) {
    return static_cast<int32_t>(sequence->size());
  }
  if (const auto* map = std::get_if<std::shared_ptr<game::ScriptMap>>(&value->value)) {
    return *map != nullptr ? static_cast<int32_t>((*map)->size()) : 0;
  }
  return 0;
}

const ScriptValue* GetItem(const ScriptValue* value, int32_t index) {
  const game::ScriptArray* sequence = GetSequence(value);
  if (sequence == nullptr || index < 0 ||
      static_cast<size_t>(index) >= sequence->size()) {
    return nullptr;
  }
  return &(*sequence)[index];
}

void GetEntries(const ScriptValue* value, const ScriptValue** keys,
                const ScriptValue** values) {
  const auto* map = std::get_if<std::shared_ptr<game::ScriptMap>>(&value->value);
  if (map == nullptr || *map == nullptr) {
    return;
  }
  int32_t index = 0;
  for (const auto& [key, entry] : **map) {
    keys[index] = &key;
    values[index] = &entry;
    ++index;
  }
}

uint8_t Invoke(const ScriptValue* function, const ValueHandle* args, int32_t count,
               ValueHandle* result, char** error) {
  const auto* reference =
      std::get_if<std::shared_ptr<game::IScriptFunctionRef>>(&function->value);
  if (reference == nullptr || *reference == nullptr) {
    *error = CopyString("value is not a function");
    return 0;
  }
  return Guard(error, [&] {
    *result = NewValue((*reference)->Call(CollectArguments(args, count)).value);
  });
}

int32_t EngineIsAttached() { return Runtime::Instance().engine() != nullptr ? 1 : 0; }

int32_t ClassCount() {
  game::Engine* engine = Runtime::Instance().engine();
  return engine != nullptr ? static_cast<int32_t>(engine->classes().size()) : 0;
}

ClassHandle ClassGet(int32_t index) {
  return Runtime::Instance().engine()->classes().at(index);
}

const char* ClassGetName(ClassHandle handle) { return handle->GetName().c_str(); }

uint64_t ClassGetId(ClassHandle handle) { return handle->GetId(); }

FunctionHandle ClassGetConstructor(ClassHandle handle) {
  return handle->GetConstructor().get();
}

int32_t ClassGetFunctionCount(ClassHandle handle, uint8_t is_static) {
  return static_cast<int32_t>(Functions(handle, is_static).size());
}

FunctionHandle ClassGetFunction(ClassHandle handle, uint8_t is_static, int32_t index) {
  return Functions(handle, is_static).at(index).get();
}

int32_t ClassGetPropertyCount(ClassHandle handle) {
  return static_cast<int32_t>(handle->GetProperties().size());
}

PropertyHandle ClassGetProperty(ClassHandle handle, int32_t index) {
  return handle->GetProperties().at(index).get();
}

int32_t GlobalFunctionCount() {
  game::Engine* engine = Runtime::Instance().engine();
  return engine != nullptr ? static_cast<int32_t>(engine->functions().size()) : 0;
}

FunctionHandle GlobalFunctionGet(int32_t index) {
  return Runtime::Instance().engine()->functions().at(index).get();
}

const char* FunctionGetName(FunctionHandle handle) { return handle->GetName(); }

char* FunctionGetSignature(FunctionHandle handle) { return DescribeType(handle); }

const char* PropertyGetName(PropertyHandle handle) {
  return handle->GetName().c_str();
}

char* PropertyGetSignature(PropertyHandle handle) { return DescribeType(handle); }

uint8_t FunctionInvoke(FunctionHandle handle, EnvironmentHandle environment,
                       const ValueHandle* args, int32_t count, ValueHandle* result,
                       char** error) {
  return Guard(error, [&] {
    *result =
        NewValue(handle->Call(CollectArguments(args, count), environment).value);
  });
}

uint8_t PropertyGet(PropertyHandle handle, ValueHandle self, ValueHandle* result,
                    char** error) {
  return Guard(error, [&] { *result = NewValue(handle->Get(*self).value); });
}

uint8_t PropertySet(PropertyHandle handle, ValueHandle self, ValueHandle value,
                    char** error) {
  return Guard(error, [&] { handle->Set(*self, *value); });
}

EnvironmentHandle EnvironmentCreate(const char* name, int32_t length) {
  game::Engine* engine = Runtime::Instance().engine();
  if (engine == nullptr) {
    return nullptr;
  }
  return &engine->CreateEnvironment(std::string(name, length), kEnvironmentVersion);
}

void EnvironmentDestroy(const char* name, int32_t length) {
  if (game::Engine* engine = Runtime::Instance().engine()) {
    engine->DestroyEnvironment(std::string(name, length));
  }
}

void* LibraryLoad(const char* path, char** error) {
  std::string message;
  void* handle = host::LoadIsolatedLibrary(path, message);
  if (handle == nullptr) {
    *error = CopyString(message);
  }
  return handle;
}

void* PackageGetEntity(game::Package* package) { return package->scripting_entity(); }

class ArgumentLease {
 public:
  ArgumentLease() : arguments_(Acquire()) {}
  ~ArgumentLease() {
    arguments_.clear();
    --depth_;
  }

  ArgumentLease(const ArgumentLease&) = delete;
  ArgumentLease& operator=(const ArgumentLease&) = delete;

  std::vector<ScriptValue>& arguments() { return arguments_; }

 private:
  static std::vector<ScriptValue>& Acquire() {
    if (depth_ == pool_.size()) {
      pool_.push_back(std::make_unique<std::vector<ScriptValue>>());
    }
    return *pool_[depth_++];
  }

  static std::vector<std::unique_ptr<std::vector<ScriptValue>>> pool_;
  static size_t depth_;

  std::vector<ScriptValue>& arguments_;
};

std::vector<std::unique_ptr<std::vector<ScriptValue>>> ArgumentLease::pool_;
size_t ArgumentLease::depth_ = 0;

uint8_t FunctionCall(FunctionHandle handle, EnvironmentHandle environment, const NativeArgument* args,
                     int32_t count, NativeArgument* result, char** error) {
  return Guard(error, [&] {
    ArgumentLease lease;
    std::vector<ScriptValue>& arguments = lease.arguments();
    for (int32_t i = 0; i < count; ++i) {
      arguments.push_back(FromArgument(args[i]));
    }
    ToArgument(handle->Call(arguments, environment), *result);
  });
}

uint8_t PackageReload(game::Package* package, char** error) {
  return Guard(error, [&] { game::GetSymbols().package_reload(package); });
}

}

const NativeApi& GetNativeApi() {
  static const NativeApi api{
      .version = kNativeApiVersion,
      .log = &Log,
      .free_string = &FreeString,
      .value_create_nil = &CreateNil,
      .value_create_bool = &CreateBool,
      .value_create_integer = &CreateInteger,
      .value_create_double = &CreateDouble,
      .value_create_string = &CreateString,
      .value_create_vector = &CreateVector,
      .value_create_rotator = &CreateRotator,
      .value_create_color = &CreateColor,
      .value_create_vector2d = &CreateVector2D,
      .value_create_entity = &CreateEntity,
      .value_create_array = &CreateArray,
      .value_create_map = &CreateMap,
      .value_create_function = &CreateFunction,
      .value_clone = &Clone,
      .value_free = &Free,
      .value_get_type = &GetType,
      .value_get_bool = &GetBool,
      .value_get_integer = &GetInteger,
      .value_get_double = &GetDouble,
      .value_get_string = &GetString,
      .value_get_doubles = &GetDoubles,
      .value_get_color = &GetColor,
      .value_get_entity = &GetEntity,
      .value_get_network_id = &GetNetworkId,
      .value_resolve_deferred = &ResolveDeferred,
      .value_get_length = &GetLength,
      .value_get_item = &GetItem,
      .value_get_entries = &GetEntries,
      .value_invoke = &Invoke,
      .engine_is_attached = &EngineIsAttached,
      .class_count = &ClassCount,
      .class_get = &ClassGet,
      .class_get_name = &ClassGetName,
      .class_get_id = &ClassGetId,
      .class_get_constructor = &ClassGetConstructor,
      .class_get_function_count = &ClassGetFunctionCount,
      .class_get_function = &ClassGetFunction,
      .class_get_property_count = &ClassGetPropertyCount,
      .class_get_property = &ClassGetProperty,
      .global_function_count = &GlobalFunctionCount,
      .global_function_get = &GlobalFunctionGet,
      .function_get_name = &FunctionGetName,
      .function_get_signature = &FunctionGetSignature,
      .property_get_name = &PropertyGetName,
      .property_get_signature = &PropertyGetSignature,
      .function_invoke = &FunctionInvoke,
      .property_get = &PropertyGet,
      .property_set = &PropertySet,
      .environment_create = &EnvironmentCreate,
      .environment_destroy = &EnvironmentDestroy,
      .package_reload = &PackageReload,
      .library_load = &LibraryLoad,
      .package_get_entity = &PackageGetEntity,
      .function_call = &FunctionCall,
  };
  return api;
}

ManagedCallbacks& GetManagedCallbacks() {
  static ManagedCallbacks callbacks{};
  return callbacks;
}

}
