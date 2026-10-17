#ifndef NANOSSHARP_GAME_SCRIPT_VALUE_H_
#define NANOSSHARP_GAME_SCRIPT_VALUE_H_

#include <cstdint>
#include <functional>
#include <memory>
#include <string>
#include <unordered_map>
#include <variant>
#include <vector>

namespace nanossharp::game {

class IScriptFunctionRef;
class OpaqueValue;
struct ScriptValue;

struct Entity {
  uint64_t class_id;
  void* instance;
  uint32_t network_id;

  bool operator==(const Entity& other) const {
    return class_id == other.class_id && instance == other.instance;
  }
};

struct Vector {
  double x;
  double y;
  double z;

  bool operator==(const Vector&) const = default;
};

struct Rotator {
  float pitch;
  float yaw;
  float roll;

  bool operator==(const Rotator&) const = default;
};

struct Color {
  float r;
  float g;
  float b;
  float a;

  bool operator==(const Color&) const = default;
};

struct Vector2D {
  float x;
  float y;

  bool operator==(const Vector2D&) const = default;
};

struct EntityDeferred {
  uint32_t network_id;

  bool operator==(const EntityDeferred&) const = default;
};

struct Void {
  bool operator==(const Void&) const = default;
};

struct Tuple {
  std::vector<ScriptValue> values;

  bool operator==(const Tuple& other) const;
};

struct WeakTable {
  std::weak_ptr<void> table;

  bool operator==(const WeakTable& other) const {
    return !table.owner_before(other.table) && !other.table.owner_before(table);
  }
};

enum class ValueType : uint8_t {
  kBool = 0,
  kInteger = 1,
  kDouble = 2,
  kString = 3,
  kEntity = 4,
  kArray = 5,
  kMap = 6,
  kNil = 7,
  kOpaque = 8,
  kFunction = 9,
  kVector = 10,
  kRotator = 11,
  kColor = 12,
  kVector2D = 13,
  kEntityDeferred = 14,
  kVoid = 15,
  kTuple = 16,
  kWeakTable = 17,
};

}

template <>
struct std::hash<nanossharp::game::Entity> {
  size_t operator()(const nanossharp::game::Entity& entity) const {
    return std::hash<void*>{}(entity.instance);
  }
};

template <>
struct std::hash<nanossharp::game::Vector> {
  size_t operator()(const nanossharp::game::Vector& vector) const {
    return std::hash<double>{}(vector.x) ^ std::hash<double>{}(vector.y) ^
           std::hash<double>{}(vector.z);
  }
};

template <>
struct std::hash<nanossharp::game::Rotator> {
  size_t operator()(const nanossharp::game::Rotator& rotator) const {
    return std::hash<float>{}(rotator.pitch) ^
           std::hash<float>{}(rotator.yaw) ^ std::hash<float>{}(rotator.roll);
  }
};

template <>
struct std::hash<nanossharp::game::Color> {
  size_t operator()(const nanossharp::game::Color& color) const {
    return std::hash<float>{}(color.r) ^ std::hash<float>{}(color.g) ^
           std::hash<float>{}(color.b) ^ std::hash<float>{}(color.a);
  }
};

template <>
struct std::hash<nanossharp::game::Vector2D> {
  size_t operator()(const nanossharp::game::Vector2D& vector) const {
    return std::hash<float>{}(vector.x) ^ std::hash<float>{}(vector.y);
  }
};

template <>
struct std::hash<nanossharp::game::EntityDeferred> {
  size_t operator()(const nanossharp::game::EntityDeferred& entity) const {
    return std::hash<uint32_t>{}(entity.network_id);
  }
};

template <>
struct std::hash<nanossharp::game::Void> {
  size_t operator()(const nanossharp::game::Void&) const { return 0; }
};

template <>
struct std::hash<nanossharp::game::Tuple> {
  size_t operator()(const nanossharp::game::Tuple& tuple) const;
};

template <>
struct std::hash<nanossharp::game::WeakTable> {
  size_t operator()(const nanossharp::game::WeakTable&) const { return 0; }
};

namespace nanossharp::game {

struct ScriptValueHash {
  size_t operator()(const ScriptValue& value) const;
};

using ScriptArray = std::vector<ScriptValue>;
using ScriptMap = std::unordered_map<ScriptValue, ScriptValue, ScriptValueHash>;

using ScriptVariant =
    std::variant<bool, int64_t, double, std::string, Entity,
                 std::shared_ptr<ScriptArray>, std::shared_ptr<ScriptMap>,
                 std::monostate, std::shared_ptr<OpaqueValue>,
                 std::shared_ptr<IScriptFunctionRef>, Vector, Rotator, Color,
                 Vector2D, EntityDeferred, Void, Tuple, WeakTable>;

struct ScriptValue {
  ScriptVariant value;

  ValueType type() const { return static_cast<ValueType>(value.index()); }

  bool operator==(const ScriptValue& other) const {
    return value == other.value;
  }
};

#if defined(_WIN32)
inline constexpr size_t kExpectedStringSize = 32;
inline constexpr size_t kExpectedScriptValueSize = 40;
#else
inline constexpr size_t kExpectedStringSize = 8;
inline constexpr size_t kExpectedScriptValueSize = 32;
#endif

static_assert(sizeof(std::string) == kExpectedStringSize,
              "the std::string ABI must match nanos world, compile with _GLIBCXX_USE_CXX11_ABI=0 "
              "on Linux and the MSVC STL on Windows");
static_assert(sizeof(Entity) == 24);
static_assert(sizeof(Vector) == 24);
static_assert(sizeof(Rotator) == 12);
static_assert(sizeof(Vector2D) == 8);
static_assert(sizeof(ScriptValue) == kExpectedScriptValueSize);
static_assert(sizeof(ScriptArray) == 24);
static_assert(alignof(ScriptValue) == 8);

inline bool Tuple::operator==(const Tuple& other) const {
  return values == other.values;
}

inline size_t ScriptValueHash::operator()(const ScriptValue& value) const {
  return std::hash<ScriptVariant>{}(value.value);
}

}

inline size_t std::hash<nanossharp::game::Tuple>::operator()(
    const nanossharp::game::Tuple& tuple) const {
  return tuple.values.size();
}

#endif
