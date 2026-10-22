#ifndef NANOSSHARP_WASM_WASM_HOST_H_
#define NANOSSHARP_WASM_WASM_HOST_H_

#include <cstdint>
#include <filesystem>
#include <string>
#include <unordered_map>
#include <vector>

#include <wasm.h>
#include <wasmtime.h>

#include "bridge/native_api.h"

namespace nanossharp::wasm {

/// Hosts the sandboxed client runtime inside wasmtime and connects its `nanos`
/// imports to the real engine seam. This plays the role that the standalone
/// `devhost-core` plays during development: it boots the unbundled core module,
/// owns the handle tables that translate i32 guest handles to native pointers,
/// and marshals arguments across guest linear memory.
class WasmHost {
 public:
  explicit WasmHost(const bridge::NativeApi& api);
  ~WasmHost();

  WasmHost(const WasmHost&) = delete;
  WasmHost& operator=(const WasmHost&) = delete;

  /// Loads and instantiates the core module, preopening the package directory
  /// as `/package` for the sandbox. Returns false and fills error on failure.
  bool Start(const std::filesystem::path& core_module,
             const std::filesystem::path& package_directory, std::string& error);

  /// Runs the guest command entry point (`_start`), which boots the managed
  /// runtime and loads the delivered packages. The instance stays alive so the
  /// engine to guest callbacks below can be driven afterwards.
  bool RunStart(std::string& error);

  /// Engine to guest callbacks: the native host invokes these named guest
  /// exports instead of holding managed function pointers.
  bool EngineAttached(std::string& error);
  bool EngineDetaching(std::string& error);
  bool Tick(float delta_time, std::string& error);

  /// Calls a guest export taking one i32 argument, optionally reading an i64
  /// result. Used by the microbenchmark driver.
  bool CallInt(const char* name, int32_t argument, int64_t* result, std::string& error);

 private:
  bool CallExport(const char* name, const wasmtime_val_t* args, size_t count,
                  std::string& error);
  bool CallInit(const char* name, size_t results, bool required, std::string& error);

  int32_t Intern(void* handle, std::vector<void*>& table,
                 std::unordered_map<void*, int32_t>& lookup);
  int32_t InternFunction(bridge::FunctionHandle handle);
  int32_t InternProperty(bridge::PropertyHandle handle);
  int32_t InternValue(bridge::ValueHandle handle);
  int32_t InternInstance(void* instance);
  int32_t InternEnvironment(bridge::EnvironmentHandle handle);
  bridge::FunctionHandle ResolveFunction(int32_t index) const;
  bridge::ValueHandle ResolveValue(int32_t index) const;
  bridge::PropertyHandle ResolveProperty(int32_t index) const;
  bridge::EnvironmentHandle ResolveEnvironment(int32_t index) const;
  void* ResolveInstance(int32_t index) const;

  int32_t HostValueCreateNil();
  int32_t HostValueCreateBool(int32_t value);
  int32_t HostValueCreateInteger(int64_t value);
  int32_t HostValueCreateDouble(double value);
  int32_t HostValueClone(int32_t value);
  void HostValueFree(int32_t value);
  int32_t HostValueGetType(int32_t value);
  int32_t HostValueGetBool(int32_t value);
  int64_t HostValueGetInteger(int32_t value);
  double HostValueGetDouble(int32_t value);
  int32_t HostValueCreateEntity(int64_t class_id, int32_t instance, int32_t network_id);
  int32_t HostValueCreateString(int32_t text, int32_t length);
  int32_t HostValueCreateVector(double x, double y, double z);
  int32_t HostValueGetString(int32_t value, int32_t length);
  void HostValueGetDoubles(int32_t value, int32_t output);
  int32_t HostValueCreateRotator(double pitch, double yaw, double roll);
  int32_t HostValueCreateColor(float r, float g, float b, float a);
  int32_t HostValueCreateVector2D(double x, double y);
  void HostValueGetColor(int32_t value, int32_t output);
  void HostValueGetEntity(int32_t value, int32_t class_id, int32_t instance, int32_t network_id);
  int32_t HostValueGetNetworkId(int32_t value);
  int32_t HostValueResolveDeferred(int32_t value);
  int32_t HostValueGetLength(int32_t value);
  int32_t HostValueGetItem(int32_t value, int32_t index);
  void HostValueGetEntries(int32_t value, int32_t keys, int32_t values);
  int32_t HostValueCreateArray(int32_t items, int32_t count);
  int32_t HostValueCreateMap(int32_t keys, int32_t values, int32_t count);
  int32_t HostValueCreateFunction(int64_t function_id, int32_t environment);
  int32_t HostValueInvoke(int32_t function, int32_t args, int32_t count, int32_t result, int32_t error);
  int32_t HostFunctionInvoke(int32_t handle, int32_t environment, int32_t args, int32_t count,
                             int32_t result, int32_t error);
  int32_t HostEnvironmentCreate(int32_t name, int32_t length);
  void HostEnvironmentDestroy(int32_t name, int32_t length);
  int32_t HostLuaGetGlobal(int32_t environment, int32_t name, int32_t length, int32_t error);
  int32_t HostLuaSetGlobal(int32_t environment, int32_t name, int32_t length, int32_t value, int32_t error);
  int32_t HostLibraryLoad(int32_t path, int32_t error);
  int32_t HostPackageReload(int32_t package, int32_t error);
  int32_t HostPackageGetEntity(int32_t package);
  int32_t HostFunctionGetSignature(int32_t handle);
  int32_t HostPropertyGetSignature(int32_t handle);
  int32_t HostPropertyGet(int32_t handle, int32_t self, int32_t result, int32_t error);
  int32_t HostPropertySet(int32_t handle, int32_t self, int32_t value, int32_t error);
  int32_t WriteScratchString(const char* text);

  uint8_t* GuestData(size_t& size);
  std::string ReadGuestString(int32_t pointer, int64_t length);

  void HostLog(int32_t type, int32_t pointer, int32_t length);
  int32_t HostEngineIsAttached();
  void HostFreeString(int32_t pointer);
  void HostSetResultScratch(int32_t buffer, int32_t capacity);
  int32_t HostSchemaDump(int32_t buffer, int32_t capacity);
  int32_t HostFunctionCall(int32_t method, int32_t environment, int32_t args,
                           int32_t count, int32_t result, int32_t error);

  std::vector<uint8_t> BuildSchemaBlob();

  bool DefineImports(std::string& error);
  bool DefineTrapsForUnboundImports(std::string& error);

  const bridge::NativeApi& api_;

  wasm_engine_t* engine_ = nullptr;
  wasmtime_store_t* store_ = nullptr;
  wasmtime_context_t* context_ = nullptr;
  wasmtime_linker_t* linker_ = nullptr;
  wasmtime_module_t* module_ = nullptr;
  wasmtime_instance_t instance_{};
  bool instantiated_ = false;
  wasmtime_caller_t* caller_ = nullptr;
  wasmtime_memory_t memory_{};
  bool has_memory_ = false;

  std::vector<void*> functions_;
  std::unordered_map<void*, int32_t> function_lookup_;
  std::vector<void*> properties_;
  std::unordered_map<void*, int32_t> property_lookup_;
  std::vector<void*> values_;
  std::vector<int32_t> value_free_list_;
  std::vector<void*> instances_;
  std::unordered_map<void*, int32_t> instance_lookup_;
  std::vector<void*> environments_;
  std::unordered_map<void*, int32_t> environment_lookup_;

  int32_t result_scratch_ = 0;
  int32_t result_scratch_capacity_ = 0;
};

}

#endif
