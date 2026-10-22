#include <chrono>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <filesystem>
#include <string>
#include <vector>

#include "bridge/native_api.h"
#include "wasm/wasm_host.h"

namespace {

using nanossharp::bridge::ClassHandle;
using nanossharp::bridge::EnvironmentHandle;
using nanossharp::bridge::FunctionHandle;
using nanossharp::bridge::NativeArgument;
using nanossharp::bridge::PropertyHandle;
using nanossharp::bridge::ValueHandle;
using nanossharp::game::ScriptValue;

constexpr intptr_t kPlayer = 100;
constexpr intptr_t kVehicle = 101;

template <typename T>
T Token(intptr_t value) {
  return reinterpret_cast<T>(value);
}

intptr_t Id(const void* handle) { return reinterpret_cast<intptr_t>(handle); }

void MockLog(int32_t type, const char* message, int32_t length) {
  std::fprintf(stdout, "[guest/%d] %.*s\n", type, length, message);
  std::fflush(stdout);
}

void MockFreeString(char* value) { std::free(value); }

int32_t MockEngineIsAttached() { return 0; }

int32_t MockClassCount() { return 2; }

ClassHandle MockClassGet(int32_t index) {
  return Token<ClassHandle>(index == 0 ? kPlayer : kVehicle);
}

const char* MockClassGetName(ClassHandle handle) {
  return Id(handle) == kPlayer ? "Player" : "Vehicle";
}

uint64_t MockClassGetId(ClassHandle handle) {
  return Id(handle) == kPlayer ? 1001 : 1002;
}

FunctionHandle MockClassGetConstructor(ClassHandle handle) {
  return Id(handle) == kPlayer ? Token<FunctionHandle>(10) : nullptr;
}

int32_t MockClassGetFunctionCount(ClassHandle handle, uint8_t is_static) {
  if (Id(handle) == kPlayer) {
    return is_static != 0 ? 1 : 2;
  }
  return is_static != 0 ? 0 : 1;
}

FunctionHandle MockClassGetFunction(ClassHandle handle, uint8_t is_static, int32_t index) {
  if (Id(handle) == kPlayer) {
    if (is_static != 0) {
      return Token<FunctionHandle>(13);
    }
    return Token<FunctionHandle>(index == 0 ? 11 : 12);
  }
  return Token<FunctionHandle>(21);
}

int32_t MockClassGetPropertyCount(ClassHandle handle) {
  return Id(handle) == kPlayer ? 1 : 0;
}

PropertyHandle MockClassGetProperty(ClassHandle, int32_t) {
  return Token<PropertyHandle>(14);
}

int32_t MockGlobalFunctionCount() { return 1; }

FunctionHandle MockGlobalFunctionGet(int32_t) { return Token<FunctionHandle>(31); }

const char* MockFunctionGetName(FunctionHandle handle) {
  switch (Id(handle)) {
    case 10:
      return "Constructor";
    case 11:
      return "GetName";
    case 12:
      return "SetHealth";
    case 13:
      return "GetAll";
    case 21:
      return "Honk";
    case 31:
      return "GetWorldTime";
    default:
      return "Unknown";
  }
}

const char* MockPropertyGetName(PropertyHandle) { return "Health"; }

uint8_t MockFunctionCall(FunctionHandle, EnvironmentHandle, const NativeArgument* args,
                         int32_t count, NativeArgument* result, char**) {
  std::memset(result, 0, sizeof(*result));
  result->type = 1;
  if (count == 0) {
    result->integer = 42;
    return 1;
  }

  if (count == 2) {
    static const char reply[] = "pong from host";
    result->type = 3;
    result->string.data = reply;
    result->string.length = static_cast<int64_t>(sizeof(reply) - 1);
    return 1;
  }

  if (count == 3) {
    result->type = 4;
    result->entity.class_id = 1001;
    result->entity.instance = reinterpret_cast<void*>(0xE0001);
    result->entity.network_id = 7;
    return 1;
  }

  const NativeArgument& first = args[0];
  if (first.type == 3) {
    std::fprintf(stdout, "[host] string arg length=%lld '%.*s'\n",
                 static_cast<long long>(first.string.length),
                 static_cast<int>(first.string.length), first.string.data);
    result->integer = first.string.length;
  } else {
    result->integer = static_cast<int64_t>(first.type);
  }
  std::fflush(stdout);
  return 1;
}

struct MockValue {
  int32_t type = 0;
  int64_t integer = 0;
  double number = 0;
  std::string text;
  double doubles[3] = {0, 0, 0};
  float colors[4] = {0, 0, 0, 0};
  std::vector<ValueHandle> items;
  std::vector<ValueHandle> keys;
};

ValueHandle MockValueCreateNil() {
  auto* value = new MockValue();
  value->type = 7;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateBool(uint8_t boolean) {
  auto* value = new MockValue();
  value->type = 0;
  value->integer = boolean != 0 ? 1 : 0;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateInteger(int64_t integer) {
  auto* value = new MockValue();
  value->type = 1;
  value->integer = integer;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateDouble(double number) {
  auto* value = new MockValue();
  value->type = 2;
  value->number = number;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateString(const char* text, int32_t length) {
  auto* value = new MockValue();
  value->type = 3;
  value->text.assign(text, static_cast<size_t>(length));
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateVector(double x, double y, double z) {
  auto* value = new MockValue();
  value->type = 10;
  value->doubles[0] = x;
  value->doubles[1] = y;
  value->doubles[2] = z;
  return reinterpret_cast<ValueHandle>(value);
}

const char* MockValueGetString(const ScriptValue* value, int32_t* length) {
  const auto* mock = reinterpret_cast<const MockValue*>(value);
  *length = static_cast<int32_t>(mock->text.size());
  return mock->text.c_str();
}

void MockValueGetDoubles(const ScriptValue* value, double* output) {
  const auto* mock = reinterpret_cast<const MockValue*>(value);
  output[0] = mock->doubles[0];
  output[1] = mock->doubles[1];
  output[2] = mock->doubles[2];
}

ValueHandle MockValueCreateRotator(double pitch, double yaw, double roll) {
  auto* value = new MockValue();
  value->type = 11;
  value->doubles[0] = pitch;
  value->doubles[1] = yaw;
  value->doubles[2] = roll;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateColor(float r, float g, float b, float a) {
  auto* value = new MockValue();
  value->type = 12;
  value->colors[0] = r;
  value->colors[1] = g;
  value->colors[2] = b;
  value->colors[3] = a;
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateVector2D(double x, double y) {
  auto* value = new MockValue();
  value->type = 13;
  value->doubles[0] = x;
  value->doubles[1] = y;
  return reinterpret_cast<ValueHandle>(value);
}

void MockValueGetColor(const ScriptValue* value, float* output) {
  const auto* mock = reinterpret_cast<const MockValue*>(value);
  output[0] = mock->colors[0];
  output[1] = mock->colors[1];
  output[2] = mock->colors[2];
  output[3] = mock->colors[3];
}

ValueHandle MockClone(ValueHandle value) {
  return reinterpret_cast<ValueHandle>(new MockValue(*reinterpret_cast<const MockValue*>(value)));
}

ValueHandle MockValueCreateArray(const ValueHandle* items, int32_t count) {
  auto* value = new MockValue();
  value->type = 5;
  for (int32_t i = 0; i < count; ++i) {
    value->items.push_back(MockClone(items[i]));
  }
  return reinterpret_cast<ValueHandle>(value);
}

ValueHandle MockValueCreateMap(const ValueHandle* keys, const ValueHandle* values, int32_t count) {
  auto* value = new MockValue();
  value->type = 6;
  for (int32_t i = 0; i < count; ++i) {
    value->keys.push_back(MockClone(keys[i]));
    value->items.push_back(MockClone(values[i]));
  }
  return reinterpret_cast<ValueHandle>(value);
}

int32_t MockValueGetLength(const ScriptValue* value) {
  return static_cast<int32_t>(reinterpret_cast<const MockValue*>(value)->items.size());
}

const ScriptValue* MockValueGetItem(const ScriptValue* value, int32_t index) {
  return reinterpret_cast<const ScriptValue*>(reinterpret_cast<const MockValue*>(value)->items.at(index));
}

void MockValueGetEntries(const ScriptValue* value, const ScriptValue** keys,
                         const ScriptValue** values) {
  const auto* mock = reinterpret_cast<const MockValue*>(value);
  for (size_t i = 0; i < mock->keys.size(); ++i) {
    keys[i] = reinterpret_cast<const ScriptValue*>(mock->keys[i]);
    values[i] = reinterpret_cast<const ScriptValue*>(mock->items[i]);
  }
}

ValueHandle MockValueClone(const ScriptValue* value) {
  return reinterpret_cast<ValueHandle>(
      new MockValue(*reinterpret_cast<const MockValue*>(value)));
}

void MockValueFree(ValueHandle value) { delete reinterpret_cast<MockValue*>(value); }

int32_t MockValueGetType(const ScriptValue* value) {
  return reinterpret_cast<const MockValue*>(value)->type;
}

uint8_t MockValueGetBool(const ScriptValue* value) {
  return reinterpret_cast<const MockValue*>(value)->integer != 0 ? 1 : 0;
}

int64_t MockValueGetInteger(const ScriptValue* value) {
  return reinterpret_cast<const MockValue*>(value)->integer;
}

double MockValueGetDouble(const ScriptValue* value) {
  return reinterpret_cast<const MockValue*>(value)->number;
}

ValueHandle MockValueCreateEntity(uint64_t, void*, uint32_t) {
  auto* value = new MockValue();
  value->type = 4;
  return reinterpret_cast<ValueHandle>(value);
}

uint8_t MockPropertyGet(PropertyHandle, ValueHandle, ValueHandle* result, char**) {
  auto* value = new MockValue();
  value->type = 1;
  value->integer = 150;
  *result = reinterpret_cast<ValueHandle>(value);
  return 1;
}

uint8_t MockPropertySet(PropertyHandle, ValueHandle, ValueHandle value, char**) {
  const auto* mock = reinterpret_cast<const MockValue*>(value);
  std::fprintf(stdout, "[host] property set: type=%d integer=%lld\n", mock->type,
               static_cast<long long>(mock->integer));
  std::fflush(stdout);
  return 1;
}

nanossharp::bridge::NativeApi BuildMockApi() {
  nanossharp::bridge::NativeApi api{};
  api.version = nanossharp::bridge::kNativeApiVersion;
  api.log = &MockLog;
  api.free_string = &MockFreeString;
  api.engine_is_attached = &MockEngineIsAttached;
  api.class_count = &MockClassCount;
  api.class_get = &MockClassGet;
  api.class_get_name = &MockClassGetName;
  api.class_get_id = &MockClassGetId;
  api.class_get_constructor = &MockClassGetConstructor;
  api.class_get_function_count = &MockClassGetFunctionCount;
  api.class_get_function = &MockClassGetFunction;
  api.class_get_property_count = &MockClassGetPropertyCount;
  api.class_get_property = &MockClassGetProperty;
  api.global_function_count = &MockGlobalFunctionCount;
  api.global_function_get = &MockGlobalFunctionGet;
  api.function_get_name = &MockFunctionGetName;
  api.property_get_name = &MockPropertyGetName;
  api.function_call = &MockFunctionCall;
  api.value_create_nil = &MockValueCreateNil;
  api.value_create_bool = &MockValueCreateBool;
  api.value_create_integer = &MockValueCreateInteger;
  api.value_create_double = &MockValueCreateDouble;
  api.value_clone = &MockValueClone;
  api.value_free = &MockValueFree;
  api.value_get_type = &MockValueGetType;
  api.value_get_bool = &MockValueGetBool;
  api.value_get_integer = &MockValueGetInteger;
  api.value_get_double = &MockValueGetDouble;
  api.value_create_entity = &MockValueCreateEntity;
  api.value_create_string = &MockValueCreateString;
  api.value_create_vector = &MockValueCreateVector;
  api.value_create_rotator = &MockValueCreateRotator;
  api.value_create_color = &MockValueCreateColor;
  api.value_create_vector2d = &MockValueCreateVector2D;
  api.value_create_array = &MockValueCreateArray;
  api.value_create_map = &MockValueCreateMap;
  api.value_get_string = &MockValueGetString;
  api.value_get_doubles = &MockValueGetDoubles;
  api.value_get_color = &MockValueGetColor;
  api.value_get_length = &MockValueGetLength;
  api.value_get_item = &MockValueGetItem;
  api.value_get_entries = &MockValueGetEntries;
  api.property_get = &MockPropertyGet;
  api.property_set = &MockPropertySet;
  return api;
}

}

int main(int argc, char** argv) {
  if (argc < 3) {
    std::fprintf(stderr, "usage: %s <core-module.wasm> <package-dir>\n", argv[0]);
    return 2;
  }

  const nanossharp::bridge::NativeApi api = BuildMockApi();
  nanossharp::wasm::WasmHost host(api);

  std::string error;
  if (!host.Start(argv[1], argv[2], error)) {
    std::fprintf(stderr, "start failed: %s\n", error.c_str());
    return 1;
  }

  if (!host.RunStart(error)) {
    std::fprintf(stderr, "run failed: %s\n", error.c_str());
    return 1;
  }

  if (argc >= 4 && std::strcmp(argv[3], "bench") == 0) {
    struct Bench {
      const char* name;
      const char* label;
      int iterations;
      int per_op;
      bool result;
    };
    const Bench benches[] = {
        {"bench_compute", "compute loop (interpreter)", 20000000, 1, true},
        {"bench_import", "import crossing (engine-is-attached)", 5000000, 1, false},
        {"bench_value", "value round trip (create+get+free)", 2000000, 3, false},
        {"bench_funccall", "function-call (0 args, int result)", 2000000, 1, false},
    };

    for (const Bench& b : benches) {
      int64_t warm = 0;
      host.CallInt(b.name, 10000, b.result ? &warm : nullptr, error);
    }

    for (const Bench& b : benches) {
      int64_t result = 0;
      const auto start = std::chrono::steady_clock::now();
      if (!host.CallInt(b.name, b.iterations, b.result ? &result : nullptr, error)) {
        std::fprintf(stderr, "bench %s failed: %s\n", b.name, error.c_str());
        return 1;
      }
      const auto end = std::chrono::steady_clock::now();
      const double ns =
          std::chrono::duration<double, std::nano>(end - start).count() / b.iterations / b.per_op;
      std::fprintf(stdout, "[bench] %-40s %8.2f ns/op\n", b.label, ns);
    }

    const int calls = 500000;
    const auto start = std::chrono::steady_clock::now();
    for (int i = 0; i < calls; ++i) {
      int64_t result = 0;
      if (!host.CallInt("bench_compute", 0, &result, error)) {
        std::fprintf(stderr, "bench host->guest failed: %s\n", error.c_str());
        return 1;
      }
    }
    const auto end = std::chrono::steady_clock::now();
    const double ns = std::chrono::duration<double, std::nano>(end - start).count() / calls;
    std::fprintf(stdout, "[bench] %-40s %8.2f ns/op\n", "host->guest export call", ns);

    std::fprintf(stdout, "wasm-host-test: benchmarks completed\n");
    return 0;
  }

  for (int i = 0; i < 3; ++i) {
    if (!host.Tick(0.016f, error)) {
      std::fprintf(stderr, "tick failed: %s\n", error.c_str());
      return 1;
    }
  }

  if (!host.EngineDetaching(error)) {
    std::fprintf(stderr, "detach failed: %s\n", error.c_str());
    return 1;
  }

  std::fprintf(stdout, "wasm-host-test: host-driven callbacks completed\n");
  return 0;
}
