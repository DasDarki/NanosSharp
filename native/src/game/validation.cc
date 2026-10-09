#include "game/validation.h"

#include <cstring>
#include <typeinfo>

namespace nanossharp::game {

namespace {

constexpr char kEngineTypeName[] = "N2nw9scripting3lua9LuaEngineE";
constexpr char kEnvironmentTypeName[] = "N2nw9scripting3lua14LuaEnvironmentE";
constexpr char kClassTypePrefix[] = "N2nw9scripting5ClassI";
constexpr size_t kMaximumClassCount = 512;
constexpr size_t kMaximumPackageNameLength = 256;

bool HasTypeName(const void* object, const char* expected) {
  const char* name = GetTypeName(object);
  return name != nullptr && std::strcmp(name, expected) == 0;
}

bool HasTypePrefix(const void* object, const char* prefix) {
  const char* name = GetTypeName(object);
  return name != nullptr && std::strncmp(name, prefix, std::strlen(prefix)) == 0;
}

}

const char* GetTypeName(const void* polymorphic_object) {
  if (polymorphic_object == nullptr) {
    return nullptr;
  }
  auto vtable = *static_cast<const std::type_info* const* const*>(polymorphic_object);
  if (vtable == nullptr || vtable[-1] == nullptr) {
    return nullptr;
  }
  return vtable[-1]->name();
}

bool ValidateEngine(const Engine* engine, std::string& error) {
  if (!HasTypeName(engine, kEngineTypeName)) {
    error = "the scripting engine is not the expected LuaEngine";
    return false;
  }

  const auto& classes = engine->classes();
  if (classes.empty() || classes.size() > kMaximumClassCount) {
    error = "the scripting class list has an unexpected size of " +
            std::to_string(classes.size());
    return false;
  }

  for (const IClass* scripting_class : classes) {
    if (!HasTypePrefix(scripting_class, kClassTypePrefix)) {
      error = "the scripting class list contains an unexpected object";
      return false;
    }
  }
  return true;
}

bool ValidatePackage(const Package* package, std::string& error) {
  if (!HasTypeName(package->environment(), kEnvironmentTypeName)) {
    error = "the package environment is not the expected LuaEnvironment";
    return false;
  }

  if (package->scripting_entity() == nullptr) {
    error = "the package has no scripting entity";
    return false;
  }

  const std::string& name = package->name();
  if (name.c_str() == nullptr || name.empty() || name.size() > kMaximumPackageNameLength) {
    error = "the package name has an unexpected layout";
    return false;
  }
  return true;
}

bool IsLuaEnvironment(const Environment* environment) {
  return HasTypeName(environment, kEnvironmentTypeName);
}

}
