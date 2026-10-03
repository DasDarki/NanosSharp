#include "bridge/native_argument.h"

#include <string>
#include <utility>

namespace nanossharp::bridge {

namespace {

using game::ScriptValue;
using game::ValueType;

ScriptValue result_storage;

}

ScriptValue FromArgument(const NativeArgument& argument) {
  switch (static_cast<ValueType>(argument.type)) {
    case ValueType::kBool:
      return {argument.boolean != 0};
    case ValueType::kInteger:
      return {argument.integer};
    case ValueType::kDouble:
      return {argument.number};
    case ValueType::kString:
      return {std::string(argument.string.data, argument.string.length)};
    case ValueType::kEntity:
      return {game::Entity{argument.entity.class_id, argument.entity.instance, argument.entity.network_id}};
    case ValueType::kVector:
      return {game::Vector{argument.components[0], argument.components[1], argument.components[2]}};
    case ValueType::kRotator:
      return {game::Rotator{static_cast<float>(argument.components[0]), static_cast<float>(argument.components[1]),
                            static_cast<float>(argument.components[2])}};
    case ValueType::kColor:
      return {game::Color{argument.color[0], argument.color[1], argument.color[2], argument.color[3]}};
    case ValueType::kVector2D:
      return {game::Vector2D{static_cast<float>(argument.components[0]), static_cast<float>(argument.components[1])}};
    default:
      break;
  }

  if (argument.type == kHandleArgument && argument.handle != nullptr) {
    return *argument.handle;
  }
  return {std::monostate{}};
}

void ToArgument(ScriptValue&& value, NativeArgument& argument) {
  argument.type = static_cast<uint8_t>(value.value.index());
  switch (value.type()) {
    case ValueType::kBool:
      argument.boolean = std::get<bool>(value.value) ? 1 : 0;
      return;
    case ValueType::kInteger:
      argument.integer = std::get<int64_t>(value.value);
      return;
    case ValueType::kDouble:
      argument.number = std::get<double>(value.value);
      return;
    case ValueType::kNil:
    case ValueType::kVoid:
      argument.type = static_cast<uint8_t>(ValueType::kNil);
      return;
    case ValueType::kString: {
      result_storage = std::move(value);
      const auto& text = std::get<std::string>(result_storage.value);
      argument.string.data = text.data();
      argument.string.length = static_cast<int64_t>(text.size());
      return;
    }
    case ValueType::kEntity: {
      const auto& entity = std::get<game::Entity>(value.value);
      argument.entity.class_id = entity.class_id;
      argument.entity.instance = entity.instance;
      argument.entity.network_id = entity.network_id;
      return;
    }
    case ValueType::kVector: {
      const auto& vector = std::get<game::Vector>(value.value);
      argument.components[0] = vector.x;
      argument.components[1] = vector.y;
      argument.components[2] = vector.z;
      return;
    }
    case ValueType::kRotator: {
      const auto& rotator = std::get<game::Rotator>(value.value);
      argument.components[0] = rotator.pitch;
      argument.components[1] = rotator.yaw;
      argument.components[2] = rotator.roll;
      return;
    }
    case ValueType::kColor: {
      const auto& color = std::get<game::Color>(value.value);
      argument.color[0] = color.r;
      argument.color[1] = color.g;
      argument.color[2] = color.b;
      argument.color[3] = color.a;
      return;
    }
    case ValueType::kVector2D: {
      const auto& vector = std::get<game::Vector2D>(value.value);
      argument.components[0] = vector.x;
      argument.components[1] = vector.y;
      return;
    }
    default:
      argument.type = kHandleArgument;
      argument.handle = new ScriptValue(std::move(value));
      return;
  }
}

void ToArgumentView(const ScriptValue& value, NativeArgument& argument) {
  argument.type = static_cast<uint8_t>(value.value.index());
  switch (value.type()) {
    case ValueType::kBool:
      argument.boolean = std::get<bool>(value.value) ? 1 : 0;
      return;
    case ValueType::kInteger:
      argument.integer = std::get<int64_t>(value.value);
      return;
    case ValueType::kDouble:
      argument.number = std::get<double>(value.value);
      return;
    case ValueType::kNil:
    case ValueType::kVoid:
      argument.type = static_cast<uint8_t>(ValueType::kNil);
      return;
    case ValueType::kString: {
      const auto& text = std::get<std::string>(value.value);
      argument.string.data = text.data();
      argument.string.length = static_cast<int64_t>(text.size());
      return;
    }
    case ValueType::kEntity: {
      const auto& entity = std::get<game::Entity>(value.value);
      argument.entity.class_id = entity.class_id;
      argument.entity.instance = entity.instance;
      argument.entity.network_id = entity.network_id;
      return;
    }
    case ValueType::kVector: {
      const auto& vector = std::get<game::Vector>(value.value);
      argument.components[0] = vector.x;
      argument.components[1] = vector.y;
      argument.components[2] = vector.z;
      return;
    }
    case ValueType::kRotator: {
      const auto& rotator = std::get<game::Rotator>(value.value);
      argument.components[0] = rotator.pitch;
      argument.components[1] = rotator.yaw;
      argument.components[2] = rotator.roll;
      return;
    }
    case ValueType::kColor: {
      const auto& color = std::get<game::Color>(value.value);
      argument.color[0] = color.r;
      argument.color[1] = color.g;
      argument.color[2] = color.b;
      argument.color[3] = color.a;
      return;
    }
    case ValueType::kVector2D: {
      const auto& vector = std::get<game::Vector2D>(value.value);
      argument.components[0] = vector.x;
      argument.components[1] = vector.y;
      return;
    }
    default:
      argument.type = kBorrowedHandleArgument;
      argument.handle = const_cast<ScriptValue*>(&value);
      return;
  }
}

}
