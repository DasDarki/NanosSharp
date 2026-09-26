#ifndef NANOSSHARP_GAME_PACKAGE_H_
#define NANOSSHARP_GAME_PACKAGE_H_

#include <cstddef>
#include <string>

namespace nanossharp::game {

class Environment;

class Package {
 public:
  Environment* environment() const { return Field<Environment*>(kEnvironmentOffset); }
  const std::string& name() const { return Field<std::string>(kNameOffset); }
  void* scripting_entity() const { return Field<void*>(kScriptingEntityOffset); }

 private:
  static constexpr size_t kScriptingEntityOffset = 0x08;
  static constexpr size_t kEnvironmentOffset = 0x1f0;
  static constexpr size_t kNameOffset = 0x1f8;

  template <typename T>
  const T& Field(size_t offset) const {
    return *reinterpret_cast<const T*>(reinterpret_cast<const char*>(this) + offset);
  }
};

}

#endif
