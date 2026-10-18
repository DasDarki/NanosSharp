#ifndef NANOSSHARP_GAME_SCRIPTING_H_
#define NANOSSHARP_GAME_SCRIPTING_H_

#include <cstdint>
#include <memory>
#include <string>
#include <vector>

#include "game/script_value.h"

namespace nanossharp::game {

class Engine;
class Environment;
class IScriptEntityRef;

struct CompatibilityVersion {
  uint8_t minor;
  uint8_t major;
  uint8_t patch;
  uint8_t reserved;
};

static_assert(sizeof(CompatibilityVersion) == 4);

class IScriptFunctionRef {
 public:
  virtual ~IScriptFunctionRef() = default;
  virtual ScriptValue Call(const std::vector<ScriptValue>& args) = 0;
  virtual void* GetRawPtr() const = 0;
  virtual void Invalidate() = 0;
};

class IFunction {
 public:
  virtual ~IFunction() = default;
  virtual const char* GetName() const = 0;
  virtual const char* GetOutputName() const = 0;
  virtual ScriptValue Call(const std::vector<ScriptValue>& args,
                           Environment* environment) = 0;
  virtual bool IsStaticFunction() const = 0;
  virtual bool WantsEnvironmentInfo() const = 0;
  virtual bool WantsEnvironmentInfoFirst() const = 0;
};

class IProperty {
 public:
  virtual ~IProperty() = default;
  virtual ScriptValue Get(const ScriptValue& self) = 0;
  virtual void Set(const ScriptValue& self, const ScriptValue& value) = 0;
  virtual const std::string& GetName() const = 0;
};

using FunctionList = std::vector<std::unique_ptr<IFunction>>;
using PropertyList = std::vector<std::unique_ptr<IProperty>>;

class IClass {
 public:
  virtual ~IClass() = default;
  virtual const std::string& GetName() const = 0;
  virtual uint64_t GetId() const = 0;
  virtual const std::unique_ptr<IFunction>& GetConstructor() const = 0;
  virtual const FunctionList& GetFunctions() const = 0;
  virtual const FunctionList& GetStaticFunctions() const = 0;
  virtual const PropertyList& GetProperties() const = 0;
  virtual void* GetDeprecatedEvents() = 0;
  virtual void* GetDefaultClass() = 0;
  virtual void AddInheritedClass() = 0;
  virtual void SetDefaultClass(uint64_t) = 0;
  virtual void ResetDefaultClass() = 0;
  virtual bool IsConstructable() const = 0;
  virtual bool IsInheritable() const = 0;
};

class Engine {
 public:
  virtual void EntityDestroyed(const Entity& entity, uint64_t class_id) = 0;
  virtual void EntityCreated(const Entity& entity,
                             const std::shared_ptr<IScriptEntityRef>& ref,
                             uint64_t class_id) = 0;
  virtual ~Engine() = default;
  virtual void Shutdown() = 0;
  virtual void Destroy() = 0;
  virtual void Initialize(bool) = 0;
  virtual Environment& CreateEnvironment(const std::string& name,
                                         CompatibilityVersion version) = 0;
  virtual void GetCurrentPath() = 0;
  virtual void GetCurrentDebugInfo() = 0;
  virtual void LoadScriptFromBuffer() = 0;
  virtual void LoadScriptFromBufferInEnvironment() = 0;
  virtual void ClearCachedFiles() = 0;
  virtual void CheckAndLoadCachedFile() = 0;
  virtual void CacheLoadedFile() = 0;
  virtual void LoadCModule() = 0;
  virtual void DestroyEnvironment(const std::string& name) = 0;

  const std::vector<IClass*>& classes() const { return classes_; }
  const FunctionList& functions() const { return functions_; }

 private:
  std::vector<IClass*> classes_;
  FunctionList functions_;
};

inline constexpr int kEngineEntityDestroyedSlot = 0;
inline constexpr int kEngineEntityCreatedSlot = 1;
inline constexpr int kEngineVirtualCount = 19;

}

#endif
